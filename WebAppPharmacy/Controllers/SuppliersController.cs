using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebAppPharmacy;
using WebAppPharmacy.Models;

namespace WebAppPharmacy.Controllers
{
    [Authorize]
    public class SuppliersController : Controller
    {
        private readonly AppDbContext _context;

        public SuppliersController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Suppliers
        public async Task<IActionResult> Index()
        {
            var suppliers = await _context.Suppliers
                .FromSqlRaw("SELECT * FROM suppliers")
                .ToListAsync();
            return View(suppliers);
        }

        // GET: Suppliers/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
                return NotFound();

            var supplier = await _context.Suppliers
                .FromSqlRaw("SELECT * FROM suppliers WHERE \"Id\" = {0}", id)
                .FirstOrDefaultAsync();

            if (supplier == null)
                return NotFound();

            return View(supplier);
        }

        // GET: Suppliers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Suppliers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Address,ContactPerson,Email,Phone")] Supplier supplier)
        {
            if (ModelState.IsValid)
            {
                var sql = "INSERT INTO suppliers (\"Title\", \"Address\", \"ContactPerson\", \"Email\", \"Phone\") VALUES ({0}, {1}, {2}, {3}, {4})";
                await _context.Database.ExecuteSqlRawAsync(sql, supplier.Title, supplier.Address, supplier.ContactPerson, supplier.Email, supplier.Phone);
                return RedirectToAction(nameof(Index));
            }
            return View(supplier);
        }

        // GET: Suppliers/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
                return NotFound();

            var supplier = await _context.Suppliers
                .FromSqlRaw("SELECT * FROM suppliers WHERE \"Id\" = {0}", id)
                .FirstOrDefaultAsync();

            if (supplier == null)
                return NotFound();

            return View(supplier);
        }

        // POST: Suppliers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,Title,Address,ContactPerson,Email,Phone")] Supplier supplier)
        {
            if (id != supplier.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var sql = "UPDATE suppliers SET \"Title\" = {0}, \"Address\" = {1}, \"ContactPerson\" = {2}, \"Email\" = {3}, \"Phone\" = {4} WHERE \"Id\" = {5}";
                    await _context.Database.ExecuteSqlRawAsync(sql, supplier.Title, supplier.Address, supplier.ContactPerson, supplier.Email, supplier.Phone, supplier.Id);
                }
                catch (DbUpdateConcurrencyException)
                {
                    var exists = await _context.Suppliers
                        .FromSqlRaw("SELECT * FROM suppliers WHERE \"Id\" = {0}", supplier.Id)
                        .AnyAsync();

                    if (!exists)
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(supplier);
        }

        // GET: Suppliers/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
                return NotFound();

            var supplier = await _context.Suppliers
                .FromSqlRaw("SELECT * FROM suppliers WHERE \"Id\" = {0}", id)
                .FirstOrDefaultAsync();

            if (supplier == null)
                return NotFound();

            return View(supplier);
        }

        // POST: Suppliers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var sql = "DELETE FROM suppliers WHERE \"Id\" = {0}";
            await _context.Database.ExecuteSqlRawAsync(sql, id);
            return RedirectToAction(nameof(Index));
        }

        private bool SupplierExists(long id)
        {
            return _context.Suppliers
                .FromSqlRaw("SELECT * FROM suppliers WHERE \"Id\" = {0}", id)
                .Any();
        }
    }
}
