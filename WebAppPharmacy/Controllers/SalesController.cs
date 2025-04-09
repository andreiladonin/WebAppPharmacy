using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebAppPharmacy;
using WebAppPharmacy.Models;
using WebAppPharmacy.Models.VM;

namespace WebAppPharmacy.Controllers
{
    public class SalesController : Controller
    {
        private readonly AppDbContext _context;

        public SalesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Sales
        public async Task<IActionResult> Index()
        {
            var sql = @"
            SELECT s.""Id"",
                   c.""FullName"" AS ""ClientName"",
                   e.""FullName"" AS ""EmployeeName"",
                   s.""SaleDate"",
                   s.""Total"",
                   st.""StatusName""
            FROM sales s
            JOIN clients c ON c.""Id"" = s.""ClientId""
            JOIN employees e ON e.""Id"" = s.""EmployeeId""
            JOIN sale_statuses st ON st.""Id"" = s.""StatusId""";

            var result = await _context.Set<SaleViewModel>().FromSqlRaw(sql).ToListAsync();
            return View(result);
        }

        // GET: Sales/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null) return NotFound();

            var sale = await _context.Sales
                .FromSqlRaw("SELECT * FROM sales WHERE \"Id\" = {0}", id)
                .FirstOrDefaultAsync();

            if (sale == null) return NotFound();

            sale.Client = await _context.Clients.FindAsync(sale.ClientId);
            sale.Employee = await _context.Employees.FindAsync(sale.EmployeeId);
            sale.Status = await _context.SaleStatuses.FindAsync(sale.StatusId);

            return View(sale);
        }

        // GET: Sales/Create
        public IActionResult Create()
        {
            ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "FullName");
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FullName");
            ViewData["StatusId"] = new SelectList(_context.SaleStatuses, "Id", "StatusName");
            return View();
        }

        // POST: Sales/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ClientId,EmployeeId,SaleDate,Total,StatusId")] Sale sale)
        {
            if (ModelState.IsValid)
            {
                var sql = @"INSERT INTO sales 
                        (""ClientId"", ""EmployeeId"", ""SaleDate"", ""Total"", ""StatusId"") 
                        VALUES ({0}, {1}, {2}, {3}, {4})";

                await _context.Database.ExecuteSqlRawAsync(sql,
                    sale.ClientId,
                    sale.EmployeeId,
                    sale.SaleDate,
                    sale.Total,
                    sale.StatusId
                );

                return RedirectToAction(nameof(Index));
            }

            ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "FullName", sale.ClientId);
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FullName", sale.EmployeeId);
            ViewData["StatusId"] = new SelectList(_context.SaleStatuses, "Id", "StatusName", sale.StatusId);
            return View(sale);
        }

        // GET: Sales/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null) return NotFound();

            var sale = await _context.Sales
                .FromSqlRaw("SELECT * FROM sales WHERE \"Id\" = {0}", id)
                .FirstOrDefaultAsync();

            if (sale == null) return NotFound();

            ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "FullName", sale.ClientId);
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FullName", sale.EmployeeId);
            ViewData["StatusId"] = new SelectList(_context.SaleStatuses, "Id", "StatusName", sale.StatusId);
            return View(sale);
        }

        // POST: Sales/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,ClientId,EmployeeId,SaleDate,Total,StatusId")] Sale sale)
        {
            if (id != sale.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var sql = @"UPDATE sales SET 
                        ""ClientId"" = {0}, 
                        ""EmployeeId"" = {1}, 
                        ""SaleDate"" = {2}, 
                        ""Total"" = {3}, 
                        ""StatusId"" = {4}
                        WHERE ""Id"" = {5}";

                await _context.Database.ExecuteSqlRawAsync(sql,
                    sale.ClientId,
                    sale.EmployeeId,
                    sale.SaleDate,
                    sale.Total,
                    sale.StatusId,
                    sale.Id
                );

                return RedirectToAction(nameof(Index));
            }

            ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "FullName", sale.ClientId);
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FullName", sale.EmployeeId);
            ViewData["StatusId"] = new SelectList(_context.SaleStatuses, "Id", "StatusName", sale.StatusId);
            return View(sale);
        }

        // GET: Sales/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null) return NotFound();

            var sale = await _context.Sales
                .FromSqlRaw("SELECT * FROM sales WHERE \"Id\" = {0}", id)
                .FirstOrDefaultAsync();

            if (sale == null) return NotFound();

            sale.Client = await _context.Clients.FindAsync(sale.ClientId);
            sale.Employee = await _context.Employees.FindAsync(sale.EmployeeId);
            sale.Status = await _context.SaleStatuses.FindAsync(sale.StatusId);

            return View(sale);
        }

        // POST: Sales/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var sql = "DELETE FROM sales WHERE \"Id\" = {0}";
            await _context.Database.ExecuteSqlRawAsync(sql, id);
            return RedirectToAction(nameof(Index));
        }

        private bool SaleExists(long id)
        {
            return _context.Sales
                .FromSqlRaw("SELECT * FROM sales WHERE \"Id\" = {0}", id)
                .Any();
        }
    }
}
