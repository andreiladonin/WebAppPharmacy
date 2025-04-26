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
using WebAppPharmacy.Models.VM;

namespace WebAppPharmacy.Controllers
{
    [Authorize]
    public class ProductsController : Controller
    {
        private readonly AppDbContext _context;

        public ProductsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            var sql = @"
    SELECT p.""Id"", p.""Title"", 
           c.""Title"" AS ""Category"", 
           pt.""TypeName"" AS ""Type"", 
           s.""Title"" AS ""Manufacturer"", 
           mu.""UnitName"" AS ""MeasurementUnit"",
           p.""Price"", p.""IsRecipe"", p.""IsMarked""
    FROM products p
    JOIN categories c ON c.""Id"" = p.""CategoryId""
    JOIN product_types pt ON pt.""Id"" = p.""TypeId""
    JOIN suppliers s ON s.""Id"" = p.""ManufacturerId""
    JOIN measurement_units mu ON mu.""Id"" = p.""MeasurementUnitId""";

            var result = await _context.Set<ProductViewModel>().FromSqlRaw(sql).ToListAsync();
            return View(result);
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products
                .FromSqlRaw("SELECT * FROM products WHERE \"Id\" = {0}", id)
                .FirstOrDefaultAsync();

            if (product == null) return NotFound();

            product.Category = await _context.Categories.FindAsync(product.CategoryId);
            product.Type = await _context.ProductTypes.FindAsync(product.TypeId);
            product.Manufacturer = await _context.Suppliers.FindAsync(product.ManufacturerId);
            product.MeasurementUnit = await _context.MeasurementUnits.FindAsync(product.MeasurementUnitId);

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Title");
            ViewData["TypeId"] = new SelectList(_context.ProductTypes, "Id", "TypeName");
            ViewData["ManufacturerId"] = new SelectList(_context.Suppliers, "Id", "Title");
            ViewData["MeasurementUnitId"] = new SelectList(_context.MeasurementUnits, "Id", "UnitName");
            return View();
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Description,DosageForm,Dosage,ManufacturerId,IsRecipe,IsMarked,CategoryId,TypeId,Price,MinQuantity,MeasurementUnitId")] Product product)
        {
            if (ModelState.IsValid)
            {
                var sql = @"
                    INSERT INTO products 
                    (""Title"", ""Description"", ""DosageForm"", ""Dosage"", ""ManufacturerId"", ""IsRecipe"", ""IsMarked"", ""CategoryId"", ""TypeId"", ""Price"", ""MinQuantity"", ""MeasurementUnitId"") 
                    VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11})";
                await _context.Database.ExecuteSqlRawAsync(sql,
                product.Title,
                product.Description,
                product.DosageForm,
                product.Dosage,
                product.ManufacturerId,
                product.IsRecipe,
                product.IsMarked,
                product.CategoryId,
                product.TypeId,
                product.Price,
                product.MinQuantity,
                product.MeasurementUnitId
            );

                return RedirectToAction(nameof(Index));
            }

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Title", product.CategoryId);
            ViewData["TypeId"] = new SelectList(_context.ProductTypes, "Id", "TypeName", product.TypeId);
            ViewData["ManufacturerId"] = new SelectList(_context.Suppliers, "Id", "Title", product.ManufacturerId);
            ViewData["MeasurementUnitId"] = new SelectList(_context.MeasurementUnits, "Id", "UnitName", product.MeasurementUnitId);
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products
                .FromSqlRaw("SELECT * FROM products WHERE \"Id\" = {0}", id)
                .FirstOrDefaultAsync();

            if (product == null) return NotFound();

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Title", product.CategoryId);
            ViewData["TypeId"] = new SelectList(_context.ProductTypes, "Id", "TypeName", product.TypeId);
            ViewData["ManufacturerId"] = new SelectList(_context.Suppliers, "Id", "Title", product.ManufacturerId);
            ViewData["MeasurementUnitId"] = new SelectList(_context.MeasurementUnits, "Id", "UnitName", product.MeasurementUnitId);
            return View(product);
        }

        // POST: Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,Title,Description,DosageForm,Dosage,ManufacturerId,IsRecipe,IsMarked,CategoryId,TypeId,Price,MinQuantity,MeasurementUnitId")] Product product)
        {
            if (id != product.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var sql = @"
                    UPDATE products SET 
                        ""Title"" = {0}, 
                        ""Description"" = {1}, 
                        ""DosageForm"" = {2}, 
                        ""Dosage"" = {3}, 
                        ""ManufacturerId"" = {4}, 
                        ""IsRecipe"" = {5}, 
                        ""IsMarked"" = {6}, 
                        ""CategoryId"" = {7}, 
                        ""TypeId"" = {8}, 
                        ""Price"" = {9}, 
                        ""MinQuantity"" = {10}, 
                        ""MeasurementUnitId"" = {11}
                    WHERE ""Id"" = {12}";

                await _context.Database.ExecuteSqlRawAsync(sql,
                    product.Title,
                    product.Description,
                    product.DosageForm,
                    product.Dosage,
                    product.ManufacturerId,
                    product.IsRecipe,
                    product.IsMarked,
                    product.CategoryId,
                    product.TypeId,
                    product.Price,
                    product.MinQuantity,
                    product.MeasurementUnitId,
                    product.Id
                );

                return RedirectToAction(nameof(Index));
            }

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Title", product.CategoryId);
            ViewData["TypeId"] = new SelectList(_context.ProductTypes, "Id", "TypeName", product.TypeId);
            ViewData["ManufacturerId"] = new SelectList(_context.Suppliers, "Id", "Title", product.ManufacturerId);
            ViewData["MeasurementUnitId"] = new SelectList(_context.MeasurementUnits, "Id", "UnitName", product.MeasurementUnitId);
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products
                .FromSqlRaw("SELECT * FROM products WHERE \"Id\" = {0}", id)
                .FirstOrDefaultAsync();

            if (product == null) return NotFound();

            product.Category = await _context.Categories.FindAsync(product.CategoryId);
            product.Type = await _context.ProductTypes.FindAsync(product.TypeId);
            product.Manufacturer = await _context.Suppliers.FindAsync(product.ManufacturerId);
            product.MeasurementUnit = await _context.MeasurementUnits.FindAsync(product.MeasurementUnitId);

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var sql = "DELETE FROM products WHERE \"Id\" = {0}";
            await _context.Database.ExecuteSqlRawAsync(sql, id);
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(long id)
        {
            return _context.Products
                .FromSqlRaw("SELECT * FROM products WHERE \"Id\" = {0}", id)
                .Any();
        }
    }

}
