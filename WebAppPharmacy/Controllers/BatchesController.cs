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
                   b.""RemainingQuantity"", b.""PurchasePrice"", s.""StatusName"" AS ""StatusName""
            FROM batches b
            JOIN products p ON p.""Id"" = b.""ProductId""
            JOIN batch_statuses s ON s.""Id"" = b.""StatusId""";

            var result = await _context.Set<BatchViewModel>().FromSqlRaw(sql).ToListAsync();
            return View(result);
        }

        // GET: Batches/Create
        public IActionResult Create()
        {
            ViewData["ProductId"] = new SelectList(_context.Products.ToList(), "Id", "Title");
            ViewData["StatusId"] = new SelectList(_context.BatchStatuses.ToList(), "Id", "StatusName");
            return View();
        }

        // POST: Batches/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductId,BatchNumber,SupplyDate,ExpirationDate,Quantity,RemainingQuantity,PurchasePrice,StorageConditions,StatusId")] Batch batch)
        {
            if (!ModelState.IsValid)
            {
                ViewData["ProductId"] = new SelectList(_context.Products.ToList(), "Id", "Title", batch.ProductId);
                ViewData["StatusId"] = new SelectList(_context.BatchStatuses.ToList(), "Id", "StatusName", batch.StatusId);
                return View(batch);
            }

            var sql = @"
            INSERT INTO batches 
            (""ProductId"", ""BatchNumber"", ""SupplyDate"", ""ExpirationDate"", 
             ""Quantity"", ""RemainingQuantity"", ""PurchasePrice"", ""StorageConditions"", ""StatusId"") 
            VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8})";

            await _context.Database.ExecuteSqlRawAsync(sql,
                batch.ProductId,
                batch.BatchNumber,
                batch.SupplyDate,
                batch.ExpirationDate,
                batch.Quantity,
                batch.RemainingQuantity,
                batch.PurchasePrice,
                batch.StorageConditions,
                batch.StatusId);

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
            ViewData["StatusId"] = new SelectList(_context.BatchStatuses.ToList(), "Id", "StatusName", batch.StatusId);
            return View(batch);
        }

        // POST: Batches/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,ProductId,BatchNumber,SupplyDate,ExpirationDate,Quantity,RemainingQuantity,PurchasePrice,StorageConditions,StatusId")] Batch batch)
        {
            if (id != batch.Id) return NotFound();

            if (!ModelState.IsValid)
            {
                ViewData["ProductId"] = new SelectList(_context.Products.ToList(), "Id", "Title", batch.ProductId);
                ViewData["StatusId"] = new SelectList(_context.BatchStatuses.ToList(), "Id", "StatusName", batch.StatusId);
                return View(batch);
            }

            var sql = @"
            UPDATE batches SET 
                ""ProductId"" = {0}, ""BatchNumber"" = {1}, ""SupplyDate"" = {2}, ""ExpirationDate"" = {3},
                ""Quantity"" = {4}, ""RemainingQuantity"" = {5}, ""PurchasePrice"" = {6}, 
                ""StorageConditions"" = {7}, ""StatusId"" = {8}
            WHERE ""Id"" = {9}";

            await _context.Database.ExecuteSqlRawAsync(sql,
                batch.ProductId,
                batch.BatchNumber,
                batch.SupplyDate,
                batch.ExpirationDate,
                batch.Quantity,
                batch.RemainingQuantity,
                batch.PurchasePrice,
                batch.StorageConditions,
                batch.StatusId,
                batch.Id);

            return RedirectToAction(nameof(Index));
        }

        // GET: Batches/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null) return NotFound();

            var batch = await _context.Batches
                .FromSqlRaw("SELECT * FROM batches WHERE \"Id\" = {0}", id)
                .FirstOrDefaultAsync();

            if (batch == null) return NotFound();

            batch.Product = await _context.Products.FirstOrDefaultAsync(p => p.Id == batch.ProductId);
            batch.Status = await _context.BatchStatuses.FirstOrDefaultAsync(s => s.Id == batch.StatusId);

            return View(batch);
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
            batch.Status = await _context.BatchStatuses.FirstOrDefaultAsync(s => s.Id == batch.StatusId);

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
