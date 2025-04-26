using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using WebAppPharmacy;
using WebAppPharmacy.Models;
using WebAppPharmacy.Models.DTO;
using WebAppPharmacy.Models.VM;

namespace WebAppPharmacy.Controllers
{
    public class BatchesController : Controller
    {
        private readonly AppDbContext _context;

        public BatchesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Batches
        public async Task<IActionResult> Index()
        {
            var sql = @"
            SELECT b.""Id"", p.""Title"" AS ""ProductTitle"", b.""BatchNumber"", 
                   b.""SupplyDate"", b.""ExpirationDate"", b.""Quantity"", 
                   b.""RemainingQuantity"", b.""PurchasePrice""
            FROM batches b
            JOIN products p ON p.""Id"" = b.""ProductId""";

            var result = await _context.Set<BatchViewModel>().FromSqlRaw(sql).ToListAsync();
            return View(result);
        }

        // GET: Batches/Create
        public IActionResult Create()
        {
            ViewData["ProductId"] = new SelectList(_context.Products.ToList(), "Id", "Title");
            return View();
        }

        // POST: Batches/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductId,BatchNumber,SupplyDate,ExpirationDate,Quantity,RemainingQuantity,PurchasePrice,StorageConditions")] Batch batch)
        {
            if (!ModelState.IsValid)
            {
                ViewData["ProductId"] = new SelectList(_context.Products.ToList(), "Id", "Title", batch.ProductId);
                return View(batch);
            }
            batch.SupplyDate = DateTime.SpecifyKind(batch.SupplyDate, DateTimeKind.Utc);
            batch.ExpirationDate = DateTime.SpecifyKind(batch.ExpirationDate, DateTimeKind.Utc);
            var sql = @"
            INSERT INTO batches 
            (""ProductId"", ""BatchNumber"", ""SupplyDate"", ""ExpirationDate"", 
             ""Quantity"", ""RemainingQuantity"", ""PurchasePrice"", ""StorageConditions"") 
            VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7})";

            await _context.Database.ExecuteSqlRawAsync(sql,
                batch.ProductId,
                batch.BatchNumber,
                batch.SupplyDate,
                batch.ExpirationDate,
                batch.Quantity,
                batch.RemainingQuantity,
                batch.PurchasePrice,
                batch.StorageConditions);

            return RedirectToAction(nameof(Index));
        }

        // GET: Batches/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null) return NotFound();

            var batch = await _context.Batches
                .FromSqlRaw("SELECT * FROM batches WHERE \"Id\" = {0}", id)
                .FirstOrDefaultAsync();

            if (batch == null) return NotFound();

            ViewData["ProductId"] = new SelectList(_context.Products.ToList(), "Id", "Title", batch.ProductId);

            return View(batch);
        }

        // POST: Batches/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,ProductId,BatchNumber,SupplyDate,ExpirationDate,Quantity,RemainingQuantity,PurchasePrice,StorageConditions")] Batch batch)
        {
            if (id != batch.Id) return NotFound();

            batch.SupplyDate = DateTime.SpecifyKind(batch.SupplyDate, DateTimeKind.Utc);
            batch.ExpirationDate = DateTime.SpecifyKind(batch.ExpirationDate, DateTimeKind.Utc);

            if (!ModelState.IsValid)
            {
                ViewData["ProductId"] = new SelectList(_context.Products.ToList(), "Id", "Title", batch.ProductId);

                return View(batch);
            }

            var sql = @"
            UPDATE batches SET 
                ""ProductId"" = {0}, ""BatchNumber"" = {1}, ""SupplyDate"" = {2}, ""ExpirationDate"" = {3},
                ""Quantity"" = {4}, ""RemainingQuantity"" = {5}, ""PurchasePrice"" = {6}, 
                ""StorageConditions"" = {7}
            WHERE ""Id"" = {8}";

            await _context.Database.ExecuteSqlRawAsync(sql,
                batch.ProductId,
                batch.BatchNumber,
                batch.SupplyDate,
                batch.ExpirationDate,
                batch.Quantity,
                batch.RemainingQuantity,
                batch.PurchasePrice,
                batch.StorageConditions,
                batch.Id);

            return RedirectToAction(nameof(Index));
        }

        // GET: Batches/Details/5
        public async Task<IActionResult> Details(long id)
        {
            try
            {
                var batch = await _context.Batches
                    .FromSqlRaw(
                        "SELECT b.\"Id\", b.\"BatchNumber\", b.\"RemainingQuantity\", b.\"ExpirationDate\", " +
                        "b.\"ProductId\", p.\"Title\", p.\"Price\", p.\"IsMarked\" " +
                        "FROM batches b " +
                        "JOIN products p ON b.\"ProductId\" = p.\"Id\" " +
                        "WHERE b.\"Id\" = {0}", id)
                    .Select(b => new
                    {
                        b.Id,
                        b.BatchNumber,
                        b.RemainingQuantity,
                        b.ExpirationDate,
                        ProductTitle = b.Product.Title,
                        ProductPrice = b.Product.Price,
                        IsMarked = b.Product.IsMarked
                    })
                    .FirstOrDefaultAsync();

                if (batch == null)
                {
                    TempData["Error"] = "Партия не найдена.";
                    return RedirectToAction(nameof(Index));
                }

                var unitItems = await _context.UnitItems
                    .FromSqlRaw(
                        "SELECT \"Id\", \"QrCode\", \"BatchId\", \"IsSold\" " +
                        "FROM unit_items " +
                        "WHERE \"BatchId\" = {0}", id)
                    .Select(u => new UnitItemDto
                    {
                        Id = u.Id,
                        QrCode = u.QrCode,
                        IsSold = u.IsSold,
                    })
                    .ToListAsync();

                var model = new BatchDetailsViewModel
                {
                    BatchId = id,
                    BatchNumber = batch.BatchNumber,
                    ProductTitle = batch.ProductTitle,
                    ProductPrice = batch.ProductPrice,
                    RemainingQuantity = batch.RemainingQuantity,
                    ExpirationDate = batch.ExpirationDate,
                    IsMarked = batch.IsMarked,
                    UnitItems = unitItems
                };

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Ошибка при загрузке данных: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Batches/AddUnitItem
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddUnitItem(long batchId, string qrCode)
        {
            try
            {
                var batchData = await _context.Batches
                    .FromSqlRaw(
                        "SELECT b.\"Id\", b.\"RemainingQuantity\", b.\"Quantity\", b.\"ProductId\", p.\"IsMarked\" " +
                        "FROM batches b " +
                        "JOIN products p ON b.\"ProductId\" = p.\"Id\" " +
                        "WHERE b.\"Id\" = {0}", batchId)
                    .Select(b => new
                    {
                        b.Id,
                        b.RemainingQuantity,
                        b.Quantity,
                        IsMarked = b.Product.IsMarked
                    })
                    .FirstOrDefaultAsync();

                if (batchData == null)
                {
                    TempData["Error"] = "Партия не найдена.";
                    return RedirectToAction(nameof(Details), new { id = batchId });
                }

                if (!batchData.IsMarked)
                {
                    TempData["Error"] = "Товар не маркированный, нельзя добавить единицу.";
                    return RedirectToAction(nameof(Details), new { id = batchId });
                }

                var currentUnitCount = await _context.UnitItems
                    .FromSqlRaw("SELECT COUNT(*) FROM unit_items WHERE \"BatchId\" = {0}", batchId)
                    .Select(u => u.Id)
                    .CountAsync();

                if (currentUnitCount >= batchData.Quantity)
                {
                    TempData["Error"] = "Количество единиц не может превышать исходное количество партии.";
                    return RedirectToAction(nameof(Details), new { id = batchId });
                }

                var qrExists = await _context.UnitItems
                    .FromSqlRaw("SELECT * FROM unit_items WHERE \"QrCode\" = {0}", qrCode)
                    .AnyAsync();

                if (qrExists)
                {
                    TempData["Error"] = "QR-код уже существует.";
                    return RedirectToAction(nameof(Details), new { id = batchId });
                }

                await _context.Database.ExecuteSqlRawAsync(
                    "INSERT INTO unit_items (\"QrCode\", \"BatchId\") VALUES ({0}, {1})",
                    qrCode, batchId);

                await _context.Database.ExecuteSqlRawAsync(
                    "UPDATE batches SET \"RemainingQuantity\" = \"RemainingQuantity\" + 1 WHERE \"Id\" = {0}",
                    batchId);

                TempData["Success"] = $"Упаковка с QR-кодом {qrCode} добавлена.";
                return RedirectToAction(nameof(Details), new { id = batchId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Ошибка при добавлении упаковки: {ex.Message}";
                return RedirectToAction(nameof(Details), new { id = batchId });
            }
        }

        // POST: Batches/EditUnitItem
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUnitItem(long id, long batchId, string qrCode)
        {
            try
            {
                var batchData = await _context.Batches
                    .FromSqlRaw(
                        "SELECT b.\"Id\", b.\"ProductId\", p.\"IsMarked\" " +
                        "FROM batches b " +
                        "JOIN products p ON b.\"ProductId\" = p.\"Id\" " +
                        "WHERE b.\"Id\" = {0}", batchId)
                    .Select(b => new
                    {
                        b.Id,
                        IsMarked = b.Product.IsMarked
                    })
                    .FirstOrDefaultAsync();

                if (batchData == null)
                {
                    TempData["Error"] = "Партия не найдена.";
                    return RedirectToAction(nameof(Details), new { id = batchId });
                }

                if (!batchData.IsMarked)
                {
                    TempData["Error"] = "Товар не маркированный.";
                    return RedirectToAction(nameof(Details), new { id = batchId });
                }

                var qrExists = await _context.UnitItems
                    .FromSqlRaw(
                        "SELECT * FROM unit_items WHERE \"QrCode\" = {0} AND \"Id\" != {1}",
                        qrCode, id)
                    .AnyAsync();

                if (qrExists)
                {
                    TempData["Error"] = "QR-код уже существует.";
                    return RedirectToAction(nameof(Details), new { id = batchId });
                }

                var rowsAffected = await _context.Database.ExecuteSqlRawAsync(
                    "UPDATE unit_items SET \"QrCode\" = {0} WHERE \"Id\" = {1} AND \"BatchId\" = {2}",
                    qrCode, id, batchId);

                if (rowsAffected == 0)
                {
                    TempData["Error"] = "Единица не найдена.";
                    return RedirectToAction(nameof(Details), new { id = batchId });
                }

                TempData["Success"] = $"QR-код обновлен на {qrCode}.";
                return RedirectToAction(nameof(Details), new { id = batchId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Ошибка при редактировании: {ex.Message}";
                return RedirectToAction(nameof(Details), new { id = batchId });
            }
        }

        // POST: Batches/DeleteUnitItem
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUnitItem(long id, long batchId)
        {
            try
            {
                var batchData = await _context.Batches
                    .FromSqlRaw(
                        "SELECT b.\"Id\", b.\"RemainingQuantity\", b.\"ProductId\", p.\"IsMarked\" " +
                        "FROM batches b " +
                        "JOIN products p ON b.\"ProductId\" = p.\"Id\" " +
                        "WHERE b.\"Id\" = {0}", batchId)
                    .Select(b => new
                    {
                        b.Id,
                        b.RemainingQuantity,
                        IsMarked = b.Product.IsMarked
                    })
                    .FirstOrDefaultAsync();

                if (batchData == null)
                {
                    TempData["Error"] = "Партия не найдена.";
                    return RedirectToAction(nameof(Details), new { id = batchId });
                }

                if (!batchData.IsMarked)
                {
                    TempData["Error"] = "Товар не маркированный.";
                    return RedirectToAction(nameof(Details), new { id = batchId });
                }

                var rowsAffected = await _context.Database.ExecuteSqlRawAsync(
                    "DELETE FROM unit_items WHERE \"Id\" = {0} AND \"BatchId\" = {1}",
                    id, batchId);

                if (rowsAffected == 0)
                {
                    TempData["Error"] = "Единица не найдена.";
                    return RedirectToAction(nameof(Details), new { id = batchId });
                }

                await _context.Database.ExecuteSqlRawAsync(
                    "UPDATE batches SET \"RemainingQuantity\" = \"RemainingQuantity\" - 1 WHERE \"Id\" = {0}",
                    batchId);

                TempData["Success"] = "Упаковка удалена.";
                return RedirectToAction(nameof(Details), new { id = batchId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Ошибка при удалении: {ex.Message}";
                return RedirectToAction(nameof(Details), new { id = batchId });
            }
        }

        // GET: Batches/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null) return NotFound();

            var batch = await _context.Batches
                .FromSqlRaw("SELECT * FROM batches WHERE \"Id\" = {0}", id)
                .FirstOrDefaultAsync();

            if (batch == null) return NotFound();

            batch.Product = await _context.Products.FirstOrDefaultAsync(p => p.Id == batch.ProductId);

            return View(batch);
        }

        // POST: Batches/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            await _context.Database.ExecuteSqlRawAsync("DELETE FROM batches WHERE \"Id\" = {0}", id);
            return RedirectToAction(nameof(Index));
        }

        private bool BatchExists(long id)
        {
            return _context.Batches
                .FromSqlRaw("SELECT * FROM batches WHERE \"Id\" = {0}", id)
                .Any();
        }
    }


}
