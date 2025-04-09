using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebAppPharmacy;
using WebAppPharmacy.Models.Dictionaries;

namespace WebAppPharmacy.Controllers
{
    public class BatchStatusController : Controller
    {
        private readonly AppDbContext _context;

        public BatchStatusController(AppDbContext context)
        {
            _context = context;
        }

        // GET: BatchStatus
        public async Task<IActionResult> Index()
        {
            var statuses = await _context.BatchStatuses
                .FromSqlRaw("SELECT * FROM batch_statuses")
                .ToListAsync();
            return View(statuses);
        }

        // GET: BatchStatus/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
                return NotFound();

            var status = await _context.BatchStatuses
                .FromSqlRaw("SELECT * FROM batch_statuses WHERE \"Id\" = {0}", id)
                .FirstOrDefaultAsync();

            if (status == null)
                return NotFound();

            return View(status);
        }

        // GET: BatchStatus/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: BatchStatus/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("StatusName")] BatchStatus batchStatus)
        {
            if (ModelState.IsValid)
            {
                var sql = "INSERT INTO batch_statuses (\"StatusName\") VALUES ({0})";
                await _context.Database.ExecuteSqlRawAsync(sql, batchStatus.StatusName);
                return RedirectToAction(nameof(Index));
            }

            return View(batchStatus);
        }

        // GET: BatchStatus/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
                return NotFound();

            var status = await _context.BatchStatuses
                .FromSqlRaw("SELECT * FROM batch_statuses WHERE \"Id\" = {0}", id)
                .FirstOrDefaultAsync();

            if (status == null)
                return NotFound();

            return View(status);
        }

        // POST: BatchStatus/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,StatusName")] BatchStatus batchStatus)
        {
            if (id != batchStatus.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var sql = "UPDATE batch_statuses SET \"StatusName\" = {0} WHERE \"Id\" = {1}";
                    await _context.Database.ExecuteSqlRawAsync(sql, batchStatus.StatusName, batchStatus.Id);
                }
                catch (DbUpdateConcurrencyException)
                {
                    var exists = await _context.BatchStatuses
                        .FromSqlRaw("SELECT * FROM batch_statuses WHERE \"Id\" = {0}", batchStatus.Id)
                        .AnyAsync();

                    if (!exists)
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(batchStatus);
        }

        // GET: BatchStatus/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
                return NotFound();

            var status = await _context.BatchStatuses
                .FromSqlRaw("SELECT * FROM batch_statuses WHERE \"Id\" = {0}", id)
                .FirstOrDefaultAsync();

            if (status == null)
                return NotFound();

            return View(status);
        }

        // POST: BatchStatus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var sql = "DELETE FROM batch_statuses WHERE \"Id\" = {0}";
            await _context.Database.ExecuteSqlRawAsync(sql, id);
            return RedirectToAction(nameof(Index));
        }

        private bool BatchStatusExists(long id)
        {
            return _context.BatchStatuses
                .FromSqlRaw("SELECT * FROM batch_statuses WHERE \"Id\" = {0}", id)
                .Any();
        }
    }

}
