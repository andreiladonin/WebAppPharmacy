using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileSystemGlobbing;
using WebAppPharmacy;
using WebAppPharmacy.Models;
using WebAppPharmacy.Models.VM;

namespace WebAppPharmacy.Controllers
{
    public class SaleDetailsController : Controller
    {
        private readonly AppDbContext _context;

        public SaleDetailsController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var sql = "SELECT sd.\"Id\", sd.\"SaleId\", b.\"BatchNumber\" AS \"Batch\", sd.\"Quantity\", sd.\"Price\", sd.\"Discount\" FROM sale_details sd JOIN batches b ON b.\"Id\" = sd.\"BatchId\"";

            var result = await _context.Set<SaleDetailViewModel>().FromSqlRaw(sql).ToListAsync();
            return View(result);
        }

        public async Task<IActionResult> Details(long? id)
        {
            if (id == null) return NotFound();

            var item = await _context.SaleDetails
                .FromSqlRaw("SELECT * FROM sale_details WHERE \"Id\" = {0}", id)
                .FirstOrDefaultAsync();

            if (item == null) return NotFound();

            item.Sale = await _context.Sales.FindAsync(item.SaleId);
            item.Batch = await _context.Batches.FindAsync(item.BatchId);

            return View(item);
        }

        public IActionResult Create()
        {
            ViewData["SaleId"] = new SelectList(_context.Sales.FromSqlRaw("SELECT * FROM sales"), "Id", "Id");
            ViewData["BatchId"] = new SelectList(_context.Batches.FromSqlRaw("SELECT * FROM batches"), "Id", "BatchNumber");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SaleId,BatchId,Quantity,Price,Discount")] SaleDetail item)
        {
            if (ModelState.IsValid)
            {
                var sql = "INSERT INTO sale_details (\"SaleId\", \"BatchId\", \"Quantity\", \"Price\", \"Discount\") VALUES({0}, {1}, {2}, {3}, {4})";
    
            await _context.Database.ExecuteSqlRawAsync(sql,
                item.SaleId, item.BatchId, item.Quantity, item.Price, item.Discount);

                return RedirectToAction(nameof(Index));
            }

            ViewData["SaleId"] = new SelectList(_context.Sales.FromSqlRaw("SELECT * FROM sales"), "Id", "Id", item.SaleId);
            ViewData["BatchId"] = new SelectList(_context.Batches.FromSqlRaw("SELECT * FROM batches"), "Id", "BatchNumber", item.BatchId);
            return View(item);
        }

        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null) return NotFound();

            var item = await _context.SaleDetails
                .FromSqlRaw("SELECT * FROM sale_details WHERE \"Id\" = {0}", id)
                .FirstOrDefaultAsync();

            if (item == null) return NotFound();

            ViewData["SaleId"] = new SelectList(_context.Sales.FromSqlRaw("SELECT * FROM sales"), "Id", "Id", item.SaleId);
            ViewData["BatchId"] = new SelectList(_context.Batches.FromSqlRaw("SELECT * FROM batches"), "Id", "BatchNumber", item.BatchId);
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,SaleId,BatchId,Quantity,Price,Discount")] SaleDetail item)
        {
            if (id != item.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var sql = "UPDATE sale_details SET \"SaleId\" = {0}, \"BatchId\" = {1}, \"Quantity\" = {2}, \"Price\" = {3}, \"Discount\" = {4} WHERE \"Id\" = {5}";

                await _context.Database.ExecuteSqlRawAsync(sql,
                    item.SaleId, item.BatchId, item.Quantity, item.Price, item.Discount, item.Id);

                return RedirectToAction(nameof(Index));
            }

            ViewData["SaleId"] = new SelectList(_context.Sales.FromSqlRaw("SELECT * FROM sales"), "Id", "Id", item.SaleId);
            ViewData["BatchId"] = new SelectList(_context.Batches.FromSqlRaw("SELECT * FROM batches"), "Id", "BatchNumber", item.BatchId);
            return View(item);
        }

        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null) return NotFound();

            var item = await _context.SaleDetails
                .FromSqlRaw("SELECT * FROM sale_details WHERE \"Id\" = {0}", id)
                .FirstOrDefaultAsync();

            if (item == null) return NotFound();

            item.Sale = await _context.Sales.FindAsync(item.SaleId);
            item.Batch = await _context.Batches.FindAsync(item.BatchId);

            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            await _context.Database.ExecuteSqlRawAsync("DELETE FROM sale_details WHERE \"Id\" = {0}", id);
            return RedirectToAction(nameof(Index));
        }

        private bool SaleDetailExists(long id)
        {
            return _context.SaleDetails
                .FromSqlRaw("SELECT * FROM sale_details WHERE \"Id\" = {0}", id)
                .Any();
        }
    }


}
