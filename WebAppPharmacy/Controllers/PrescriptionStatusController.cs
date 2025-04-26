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
    public class PrescriptionStatusController : Controller
    {
        private readonly AppDbContext _context;

        public PrescriptionStatusController(AppDbContext context)
        {
            _context = context;
        }

        // GET: PrescriptionStatus
        public async Task<IActionResult> Index()
        {
            var statuses = await _context.PrescriptionStatuses
                .FromSqlRaw("SELECT * FROM prescription_statuses")
                .ToListAsync();
            return View(statuses);
        }

        // GET: PrescriptionStatus/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
                return NotFound();

            var status = await _context.PrescriptionStatuses
                .FromSqlRaw("SELECT * FROM prescription_statuses WHERE \"Id\" = {0}", id)
                .FirstOrDefaultAsync();

            if (status == null)
                return NotFound();

            return View(status);
        }

        // GET: PrescriptionStatus/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: PrescriptionStatus/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("StatusName")] PrescriptionStatus prescriptionStatus)
        {
            if (ModelState.IsValid)
            {
                var sql = "INSERT INTO prescription_statuses (\"StatusName\") VALUES ({0})";
                await _context.Database.ExecuteSqlRawAsync(sql, prescriptionStatus.StatusName);
                return RedirectToAction(nameof(Index));
            }
            return View(prescriptionStatus);
        }

        // GET: PrescriptionStatus/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
                return NotFound();

            var status = await _context.PrescriptionStatuses
                .FromSqlRaw("SELECT * FROM prescription_statuses WHERE \"Id\" = {0}", id)
                .FirstOrDefaultAsync();

            if (status == null)
                return NotFound();

            return View(status);
        }

        // POST: PrescriptionStatus/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,StatusName")] PrescriptionStatus prescriptionStatus)
        {
            if (id != prescriptionStatus.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var sql = "UPDATE prescription_statuses SET \"StatusName\" = {0} WHERE \"Id\" = {1}";
                    await _context.Database.ExecuteSqlRawAsync(sql, prescriptionStatus.StatusName, prescriptionStatus.Id);
                }
                catch (DbUpdateConcurrencyException)
                {
                    var exists = await _context.PrescriptionStatuses
                        .FromSqlRaw("SELECT * FROM prescription_statuses WHERE \"Id\" = {0}", prescriptionStatus.Id)
                        .AnyAsync();

                    if (!exists)
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(prescriptionStatus);
        }

        // GET: PrescriptionStatus/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
                return NotFound();

            var status = await _context.PrescriptionStatuses
                .FromSqlRaw("SELECT * FROM prescription_statuses WHERE \"Id\" = {0}", id)
                .FirstOrDefaultAsync();

            if (status == null)
                return NotFound();

            return View(status);
        }

        // POST: PrescriptionStatus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var sql = "DELETE FROM prescription_statuses WHERE \"Id\" = {0}";
            await _context.Database.ExecuteSqlRawAsync(sql, id);
            return RedirectToAction(nameof(Index));
        }

        private bool PrescriptionStatusExists(long id)
        {
            return _context.PrescriptionStatuses
                .FromSqlRaw("SELECT * FROM prescription_statuses WHERE \"Id\" = {0}", id)
                .Any();
        }
    }

}
