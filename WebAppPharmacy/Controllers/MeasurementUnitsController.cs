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
    public class MeasurementUnitsController : Controller
    {
        private readonly AppDbContext _context;

        public MeasurementUnitsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: MeasurementUnits
        public async Task<IActionResult> Index()
        {
            var units = await _context.MeasurementUnits
                .FromSqlRaw("SELECT * FROM measurement_units")
                .ToListAsync();
            return View(units);
        }

        // GET: MeasurementUnits/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
                return NotFound();

            var unit = await _context.MeasurementUnits
                .FromSqlRaw("SELECT * FROM measurement_units WHERE \"Id\" = {0}", id)
                .FirstOrDefaultAsync();

            if (unit == null)
                return NotFound();

            return View(unit);
        }

        // GET: MeasurementUnits/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: MeasurementUnits/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UnitName,Abbreviation")] MeasurementUnit measurementUnit)
        {
            if (ModelState.IsValid)
            {
                var sql = "INSERT INTO measurement_units (\"UnitName\", \"Abbreviation\") VALUES ({0}, {1})";
                await _context.Database.ExecuteSqlRawAsync(sql, measurementUnit.UnitName, measurementUnit.Abbreviation);
                return RedirectToAction(nameof(Index));
            }
            return View(measurementUnit);
        }

        // GET: MeasurementUnits/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
                return NotFound();

            var unit = await _context.MeasurementUnits
                .FromSqlRaw("SELECT * FROM measurement_units WHERE \"Id\" = {0}", id)
                .FirstOrDefaultAsync();

            if (unit == null)
                return NotFound();

            return View(unit);
        }

        // POST: MeasurementUnits/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,UnitName,Abbreviation")] MeasurementUnit measurementUnit)
        {
            if (id != measurementUnit.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var sql = "UPDATE measurement_units SET \"UnitName\" = {0}, \"Abbreviation\" = {1} WHERE \"Id\" = {2}";
                    await _context.Database.ExecuteSqlRawAsync(sql, measurementUnit.UnitName, measurementUnit.Abbreviation, measurementUnit.Id);
                }
                catch (DbUpdateConcurrencyException)
                {
                    var exists = await _context.MeasurementUnits
                        .FromSqlRaw("SELECT * FROM measurement_units WHERE \"Id\" = {0}", measurementUnit.Id)
                        .AnyAsync();

                    if (!exists)
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(measurementUnit);
        }

        // GET: MeasurementUnits/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
                return NotFound();

            var unit = await _context.MeasurementUnits
                .FromSqlRaw("SELECT * FROM measurement_units WHERE \"Id\" = {0}", id)
                .FirstOrDefaultAsync();

            if (unit == null)
                return NotFound();

            return View(unit);
        }

        // POST: MeasurementUnits/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var sql = "DELETE FROM measurement_units WHERE \"Id\" = {0}";
            await _context.Database.ExecuteSqlRawAsync(sql, id);
            return RedirectToAction(nameof(Index));
        }

        private bool MeasurementUnitExists(long id)
        {
            return _context.MeasurementUnits
                .FromSqlRaw("SELECT * FROM measurement_units WHERE \"Id\" = {0}", id)
                .Any();
        }
    }

}
