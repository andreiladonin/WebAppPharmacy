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
    public class CategoriesController : Controller
    {
        private readonly AppDbContext _context;

        public CategoriesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Categories
        public async Task<IActionResult> Index()
        {
            var categories = await _context.Categories
                .FromSqlRaw("SELECT * FROM categories")
                .ToListAsync();
            return View(categories);
        }

        // GET: Categories/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
                return NotFound();

            var category = await _context.Categories
                .FromSqlRaw("SELECT * FROM categories WHERE \"Id\" = {0}", id)
                .FirstOrDefaultAsync();

            if (category == null)
                return NotFound();

            return View(category);
        }

        // GET: Categories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Categories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title")] Category category)
        {
            if (ModelState.IsValid)
            {
                var sql = "INSERT INTO categories (\"Title\") VALUES ({0})";
                await _context.Database.ExecuteSqlRawAsync(sql, category.Title);
                return RedirectToAction(nameof(Index));
            }

            return View(category);
        }

        // GET: Categories/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
                return NotFound();

            var category = await _context.Categories
                .FromSqlRaw("SELECT * FROM categories WHERE \"Id\" = {0}", id)
                .FirstOrDefaultAsync();

            if (category == null)
                return NotFound();

            return View(category);
        }

        // POST: Categories/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,Title")] Category category)
        {
            if (id != category.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var sql = "UPDATE categories SET \"Title\" = {0} WHERE \"Id\" = {1}";
                    await _context.Database.ExecuteSqlRawAsync(sql, category.Title, category.Id);
                }
                catch (DbUpdateConcurrencyException)
                {
                    var exists = await _context.Categories
                        .FromSqlRaw("SELECT * FROM categories WHERE \"Id\" = {0}", category.Id)
                        .AnyAsync();

                    if (!exists)
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(category);
        }

        // GET: Categories/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
                return NotFound();

            var category = await _context.Categories
                .FromSqlRaw("SELECT * FROM categories WHERE \"Id\" = {0}", id)
                .FirstOrDefaultAsync();

            if (category == null)
                return NotFound();

            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var sql = "DELETE FROM categories WHERE \"Id\" = {0}";
            await _context.Database.ExecuteSqlRawAsync(sql, id);
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(long id)
        {
            return _context.Categories
                .FromSqlRaw("SELECT * FROM categories WHERE \"Id\" = {0}", id)
                .Any();
        }
    }
}
