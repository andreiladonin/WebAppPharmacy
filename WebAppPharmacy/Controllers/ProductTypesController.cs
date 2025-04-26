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
    public class ProductTypesController : Controller
    {
        private readonly AppDbContext _context;

        public ProductTypesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: ProductTypes
        public async Task<IActionResult> Index()
        {
            var types = await _context.ProductTypes
                .FromSqlRaw("SELECT * FROM product_types")
                .ToListAsync();
            return View(types);
        }

        // GET: ProductTypes/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
                return NotFound();

            var type = await _context.ProductTypes
                .FromSqlRaw("SELECT * FROM product_types WHERE \"Id\" = {0}", id)
                .FirstOrDefaultAsync();

            if (type == null)
                return NotFound();

            return View(type);
        }

        // GET: ProductTypes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ProductTypes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TypeName")] ProductType productType)
        {
            if (ModelState.IsValid)
            {
                var sql = "INSERT INTO product_types (\"TypeName\") VALUES ({0})";
                await _context.Database.ExecuteSqlRawAsync(sql, productType.TypeName);
                return RedirectToAction(nameof(Index));
            }
            return View(productType);
        }

        // GET: ProductTypes/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
                return NotFound();

            var type = await _context.ProductTypes
                .FromSqlRaw("SELECT * FROM product_types WHERE \"Id\" = {0}", id)
                .FirstOrDefaultAsync();

            if (type == null)
                return NotFound();

            return View(type);
        }

        // POST: ProductTypes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,TypeName")] ProductType productType)
        {
            if (id != productType.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var sql = "UPDATE product_types SET \"TypeName\" = {0} WHERE \"Id\" = {1}";
                    await _context.Database.ExecuteSqlRawAsync(sql, productType.TypeName, productType.Id);
                }
                catch (DbUpdateConcurrencyException)
                {
                    var exists = await _context.ProductTypes
                        .FromSqlRaw("SELECT * FROM product_types WHERE \"Id\" = {0}", productType.Id)
                        .AnyAsync();

                    if (!exists)
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(productType);
        }

        // GET: ProductTypes/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
                return NotFound();

            var type = await _context.ProductTypes
                .FromSqlRaw("SELECT * FROM product_types WHERE \"Id\" = {0}", id)
                .FirstOrDefaultAsync();

            if (type == null)
                return NotFound();

            return View(type);
        }

        // POST: ProductTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var sql = "DELETE FROM product_types WHERE \"Id\" = {0}";
            await _context.Database.ExecuteSqlRawAsync(sql, id);
            return RedirectToAction(nameof(Index));
        }

        private bool ProductTypeExists(long id)
        {
            return _context.ProductTypes
                .FromSqlRaw("SELECT * FROM product_types WHERE \"Id\" = {0}", id)
                .Any();
        }
    }
}
