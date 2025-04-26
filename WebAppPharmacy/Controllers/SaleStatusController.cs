using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebAppPharmacy;
using WebAppPharmacy.Models.Dictionaries;

namespace WebAppPharmacy.Controllers
{
    [Authorize]
    public class SaleStatusController : Controller
    {
        private readonly AppDbContext _context;

        public SaleStatusController(AppDbContext context)
        {
            _context = context;
        }

        // GET: SaleStatus
        public async Task<IActionResult> Index()
        {
            var statuses = await _context.SaleStatuses
                .FromSqlRaw("SELECT * FROM sale_statuses")
                .ToListAsync();
            return View(statuses);
        }

        // GET: SaleStatus/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
                return NotFound();

            var status = await _context.SaleStatuses
                .FromSqlRaw("SELECT * FROM sale_statuses WHERE \"Id\" = {0}", id)
                .FirstOrDefaultAsync();

            if (status == null)
                return NotFound();

            return View(status);
        }

        // GET: SaleStatus/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: SaleStatus/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("StatusName")] SaleStatus saleStatus)
        {
            if (ModelState.IsValid)
            {
                var sql = "INSERT INTO sale_statuses (\"StatusName\") VALUES ({0})";
                await _context.Database.ExecuteSqlRawAsync(sql, saleStatus.StatusName);
                return RedirectToAction(nameof(Index));
            }
            return View(saleStatus);
        }

        // GET: SaleStatus/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
                return NotFound();

            var status = await _context.SaleStatuses
                .FromSqlRaw("SELECT * FROM sale_statuses WHERE \"Id\" = {0}", id)
                .FirstOrDefaultAsync();

            if (status == null)
                return NotFound();

            return View(status);
        }

        // POST: SaleStatus/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,StatusName")] SaleStatus saleStatus)
        {
            if (id != saleStatus.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var sql = "UPDATE sale_statuses SET \"StatusName\" = {0} WHERE \"Id\" = {1}";
                    await _context.Database.ExecuteSqlRawAsync(sql, saleStatus.StatusName, saleStatus.Id);
                }
                catch (DbUpdateConcurrencyException)
                {
                    var exists = await _context.SaleStatuses
                        .FromSqlRaw("SELECT * FROM sale_statuses WHERE \"Id\" = {0}", saleStatus.Id)
                        .AnyAsync();

                    if (!exists)
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(saleStatus);
        }

        // GET: SaleStatus/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
                return NotFound();

            var status = await _context.SaleStatuses
                .FromSqlRaw("SELECT * FROM sale_statuses WHERE \"Id\" = {0}", id)
                .FirstOrDefaultAsync();

            if (status == null)
                return NotFound();

            return View(status);
        }

        // POST: SaleStatus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var sql = "DELETE FROM sale_statuses WHERE \"Id\" = {0}";
            await _context.Database.ExecuteSqlRawAsync(sql, id);
            return RedirectToAction(nameof(Index));
        }

        private bool SaleStatusExists(long id)
        {
            return _context.SaleStatuses
                .FromSqlRaw("SELECT * FROM sale_statuses WHERE \"Id\" = {0}", id)
                .Any();
        }
    }
}
