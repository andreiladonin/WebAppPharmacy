using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using WebAppPharmacy;
using WebAppPharmacy.Models;
using WebAppPharmacy.Models.VM;

namespace WebAppPharmacy.Controllers
{
    [Authorize]
    public class SalesController : Controller
    {
        private readonly AppDbContext _context;

        public SalesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Sales
        public async Task<IActionResult> Index(string? search, string? sortOrder, int page = 1)
        {
            const int pageSize = 10;
            int offset = (page - 1) * pageSize;

            var whereSql = "";
            var orderSql = "ORDER BY s.\"SaleDate\" DESC";
            var parameters = new List<NpgsqlParameter>();

            if (!string.IsNullOrWhiteSpace(search))
            {
                whereSql = "WHERE c.\"FullName\" ILIKE @search OR e.\"FullName\" ILIKE @search";
                parameters.Add(new NpgsqlParameter("@search", $"%{search}%"));
            }

            orderSql = sortOrder switch
            {
                "client_asc" => "ORDER BY c.\"FullName\" ASC",
                "client_desc" => "ORDER BY c.\"FullName\" DESC",
                "date_asc" => "ORDER BY s.\"SaleDate\" ASC",
                "total_desc" => "ORDER BY s.\"Total\" DESC",
                "total_asc" => "ORDER BY s.\"Total\" ASC",
                _ => "ORDER BY s.\"SaleDate\" DESC"
            };

            string sql = $@"
                SELECT s.""Id"", s.""ClientId"", s.""EmployeeId"", s.""StatusId"", s.""SaleDate"", s.""Total"",
                       c.""FullName"" AS ""ClientName"",
                       e.""FullName"" AS ""EmployeeName"",
                       ss.""StatusName""
                FROM sales s
                JOIN clients c ON s.""ClientId"" = c.""Id""
                JOIN employees e ON s.""EmployeeId"" = e.""Id""
                JOIN sale_statuses ss ON s.""StatusId"" = ss.""Id""
                {whereSql}
                {orderSql}
                LIMIT @limit OFFSET @offset;
            ";

            parameters.Add(new NpgsqlParameter("@limit", pageSize));
            parameters.Add(new NpgsqlParameter("@offset", offset));

            var items = await _context.SaleListItemViewModel
                .FromSqlRaw(sql, parameters.ToArray())
                .ToListAsync();

            // Подсчёт общего количества записей
            string countSql = $@"
                SELECT COUNT(*) 
                FROM sales s
                JOIN clients c ON s.""ClientId"" = c.""Id""
                JOIN employees e ON s.""EmployeeId"" = e.""Id""
                {whereSql};
            ";

            using var cmd = _context.Database.GetDbConnection().CreateCommand();
            cmd.CommandText = countSql;
            if (parameters.FirstOrDefault(p => p.ParameterName == "@search") is var searchParam && searchParam != null)
                cmd.Parameters.Add(searchParam);

            await _context.Database.OpenConnectionAsync();
            var count = Convert.ToInt32(await cmd.ExecuteScalarAsync());
            await _context.Database.CloseConnectionAsync();

            var totalPages = (int)Math.Ceiling(count / (double)pageSize);

            var vm = new SaleIndexViewModel
            {
                Sales = items,
                CurrentPage = page,
                TotalPages = totalPages,
                Search = search,
                SortOrder = sortOrder
            };

            return View(vm);
        }
        public async Task<IActionResult> Details(long id)
        {
            var sale = await _context.SaleDetailViewModel
                .FromSqlRaw(@"
                    SELECT s.""Id"", s.""SaleDate"", s.""Total"",
                           c.""FullName"" AS ""ClientName"",
                           e.""FullName"" AS ""EmployeeName"",
                           ss.""StatusName""
                    FROM sales s
                    JOIN clients c ON s.""ClientId"" = c.""Id""
                    JOIN employees e ON s.""EmployeeId"" = e.""Id""
                    JOIN sale_statuses ss ON s.""StatusId"" = ss.""Id""
                    WHERE s.""Id"" = {0}
                ", id)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (sale == null)
                return NotFound();

            var items = await _context.SaleDetailItemViewModel
                .FromSqlRaw(@"
                    SELECT p.""Title"" AS ""ProductTitle"", pb.""BatchNumber"", pb.""ExpirationDate"",
                           si.""Quantity"", si.""Price"", si.""QrCode""
                    FROM sale_details si
                    JOIN batches pb ON si.""BatchId"" = pb.""Id""
                    JOIN products p ON pb.""ProductId"" = p.""Id""
                    WHERE si.""SaleId"" = {0}
                ", id)
                .ToListAsync();

            sale.Items = items;

            return View(sale);
        }

        // GET: Sales/Create
        public async Task<IActionResult> Create()
        {
            try
            {
                var clients = await _context.Clients
                    .FromSqlRaw("SELECT \"Id\", \"FullName\" FROM clients")
                    .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.FullName })
                    .ToListAsync();

                var employees = await _context.Employees
                    .FromSqlRaw("SELECT \"Id\", \"FullName\" FROM employees")
                    .Select(e => new SelectListItem { Value = e.Id.ToString(), Text = e.FullName })
                    .ToListAsync();

                var statuses = await _context.SaleStatuses
                    .FromSqlRaw("SELECT \"Id\", \"StatusName\" FROM sale_statuses")
                    .Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.StatusName })
                    .ToListAsync();

                var batches = await _context.Batches
                    .FromSqlRaw(@"
                        SELECT b.* 
                        FROM batches b
                        INNER JOIN products p ON p.""Id"" = b.""ProductId""
                        WHERE b.""RemainingQuantity"" > 0
                    ")
                    .Include(b => b.Product)
                    .ToListAsync();

                var batchIds = batches.Select(b => b.Id).ToList();

                var qrCodes = await _context.UnitItems
                    .FromSqlRaw("SELECT * FROM unit_items WHERE \"IsSold\" IS NULL OR \"IsSold\" = FALSE")
                    .Where(q => batchIds.Contains(q.BatchId))
                    .ToListAsync();

                var availableProducts = batches.Select(b => new ProductBatchViewModel
                {
                    BatchId = b.Id,
                    ProductId = b.ProductId,
                    ProductTitle = b.Product.Title,
                    BatchNumber = b.BatchNumber,
                    ExpirationDate = b.ExpirationDate,
                    Price = b.Product.Price,
                    RemainingQuantity = b.Product.IsMarked
                        ? qrCodes.Count(q => q.BatchId == b.Id)
                        : b.RemainingQuantity,
                    IsMarked = b.Product.IsMarked,
                    IsRecipe = b.Product.IsRecipe,
                    AvailableQrCodes = qrCodes
                        .Where(q => q.BatchId == b.Id)
                        .Select(q => q.QrCode)
                        .ToList()
                }).ToList();

                var model = new SaleCreateViewModel
                {
                    Clients = clients,
                    Employees = employees,
                    Statuses = statuses,
                    AvailableProducts = availableProducts
                };

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Ошибка при загрузке данных: {ex.Message}";
                return RedirectToAction("Index", "Batches");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SaleCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).ToList();
                foreach (var error in errors)
                {
                    // Логируем или выводим ошибку
                    Console.WriteLine(error.ErrorMessage);
                }
                return View(model);
            }

            decimal total = model.Items.Sum(item => item.Price * item.Quantity);
            DateTime now = DateTime.UtcNow;

            // Получаем ID продажи
            var saleIdResult = await _context.Sales
                .FromSqlRaw(@"
                    INSERT INTO sales (""SaleDate"", ""Total"", ""ClientId"", ""EmployeeId"", ""StatusId"")
                    VALUES (@p0, @p1, @p2, @p3, @p4)
                    RETURNING *;
                ", now, total, model.ClientId, model.EmployeeId, model.StatusId)
                        .ToListAsync();

            var saleIdFinal = saleIdResult.First().Id;

            foreach (var item in model.Items)
            {
                // Добавляем строку продажи
                await _context.Database.ExecuteSqlRawAsync(@"
                    INSERT INTO sale_details (""Quantity"", ""Price"", ""QrCode"", ""SaleId"", ""BatchId"")
                    VALUES (@p0, @p1, @p2, @p3, @p4);
                ", new object[] {
                    item.Quantity,
                    item.Price,
                    item.QrCode ?? "",
                    saleIdFinal,
                    item.BatchId
                });

                // Обновляем остаток
                await _context.Database.ExecuteSqlRawAsync(@"
                    UPDATE batches
                    SET ""RemainingQuantity"" = ""RemainingQuantity"" - @p0
                    WHERE ""Id"" = @p1;
                ", new object[] {
                    item.Quantity,
                    item.BatchId
                });

                // Обновляем флаг продаж по QR-коду
                if (!string.IsNullOrEmpty(item.QrCode))
                {
                    await _context.Database.ExecuteSqlRawAsync(@"
                        UPDATE unit_items
                        SET ""IsSold"" = TRUE
                        WHERE ""QrCode"" = @p0 AND ""BatchId"" = @p1;
                    ", new object[] {
                        item.QrCode,
                        item.BatchId
                    });
                }
            }

            return RedirectToAction("Index");
        }
    }
}

