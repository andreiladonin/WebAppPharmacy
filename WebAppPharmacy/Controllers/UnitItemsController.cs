using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebAppPharmacy;
using WebAppPharmacy.Models;

namespace WebAppPharmacy.Controllers
{
    public class UnitItemsController : Controller
    {
        private readonly AppDbContext _context;

        public UnitItemsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: UnitItems
        public async Task<IActionResult> Index()
        {
            var items = await _context.UnitItems
                .FromSqlRaw("SELECT * FROM unit_items")
                .ToListAsync();

            foreach (var item in items)
            {
                item.Batch = await _context.Batches.FirstOrDefaultAsync(b => b.Id == item.BatchId);
            }

            return View(items);
        }

        // GET: UnitItems/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null) return NotFound();

            var item = await _context.UnitItems
                .FromSqlRaw("SELECT * FROM unit_items WHERE \"Id\" = {0}", id)
                .FirstOrDefaultAsync();

            if (item == null) return NotFound();

            item.Batch = await _context.Batches.FirstOrDefaultAsync(b => b.Id == item.BatchId);

            return View(item);
        }

        // GET: UnitItems/Create
        public IActionResult Create()
        {
            ViewData["BatchId"] = new SelectList(_context.Batches, "Id", "BatchNumber");
            return View();
        }

        // POST: UnitItems/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("QrCode,Status,BatchId")] UnitItem item)
        {
            if (ModelState.IsValid)
            {
                var sql = @"INSERT INTO unit_items 
                        (""QrCode"", ""Status"", ""BatchId"") 
                        VALUES ({0}, {1}, {2})";

                await _context.Database.ExecuteSqlRawAsync(sql,
                    item.QrCode,
                    item.Status,
                    item.BatchId
                );

                return RedirectToAction(nameof(Index));
            }

            ViewData["BatchId"] = new SelectList(_context.Batches, "Id", "BatchNumber", item.BatchId);
            return View(item);
        }

        // GET: UnitItems/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null) return NotFound();

            var item = await _context.UnitItems
                .FromSqlRaw("SELECT * FROM unit_items WHERE \"Id\" = {0}", id)
                .FirstOrDefaultAsync();

            if (item == null) return NotFound();

            ViewData["BatchId"] = new SelectList(_context.Batches, "Id", "BatchNumber", item.BatchId);
            return View(item);
        }

        // POST: UnitItems/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,QrCode,Status,BatchId")] UnitItem item)
        {
            if (id != item.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var sql = @"UPDATE unit_items SET 
                        ""QrCode"" = {0}, 
                        ""Status"" = {1}, 
                        ""BatchId"" = {2}
                        WHERE ""Id"" = {3}";

                await _context.Database.ExecuteSqlRawAsync(sql,
                    item.QrCode,
                    item.Status,
                    item.BatchId,
                    item.Id
                );

                return RedirectToAction(nameof(Index));
            }

            ViewData["BatchId"] = new SelectList(_context.Batches, "Id", "BatchNumber", item.BatchId);
            return View(item);
        }

        // GET: UnitItems/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null) return NotFound();

            var item = await _context.UnitItems
                .FromSqlRaw("SELECT * FROM unit_items WHERE \"Id\" = {0}", id)
                .FirstOrDefaultAsync();

            if (item == null) return NotFound();

            item.Batch = await _context.Batches.FirstOrDefaultAsync(b => b.Id == item.BatchId);

            return View(item);
        }

        // POST: UnitItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var sql = "DELETE FROM unit_items WHERE \"Id\" = {0}";
            await _context.Database.ExecuteSqlRawAsync(sql, id);
            return RedirectToAction(nameof(Index));
        }

        private bool UnitItemExists(long id)
        {
            return _context.UnitItems
                .FromSqlRaw("SELECT * FROM unit_items WHERE \"Id\" = {0}", id)
                .Any();
        }
    }
}
