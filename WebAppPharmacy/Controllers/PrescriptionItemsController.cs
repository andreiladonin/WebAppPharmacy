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
    public class PrescriptionItemsController : Controller
    {
        private readonly AppDbContext _context;

        public PrescriptionItemsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: PrescriptionItems/Create
        public IActionResult Create(long? prescriptionId)
        {
            ViewData["PrescriptionId"] = new SelectList(_context.Prescriptions.FromSqlRaw("SELECT * FROM prescriptions"), "Id", "PrescriptionCode", prescriptionId);
            ViewData["ProductId"] = new SelectList(_context.Products.FromSqlRaw("SELECT * FROM products"), "Id", "Title");

            ViewBag.PrescriptionId = prescriptionId; // для скрытого поля в форме
            return View();
        }

        // POST: PrescriptionItems/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,PrescriptionId,ProductId,Quantity,DispensedQuantity")] PrescriptionItem item)
        {
            if (ModelState.IsValid)
            {
                await _context.Database.ExecuteSqlRawAsync(@"
                INSERT INTO prescription_items (""PrescriptionId"", ""ProductId"", ""Quantity"", ""DispensedQuantity"")
                VALUES ({0}, {1}, {2}, {3})",
                    item.PrescriptionId,
                    item.ProductId,
                    item.Quantity,
                    item.DispensedQuantity);

                return RedirectToAction("Details", "Prescriptions", new { id = item.PrescriptionId });
            }

            ViewData["PrescriptionId"] = new SelectList(_context.Prescriptions.FromSqlRaw("SELECT * FROM prescriptions"), "Id", "PrescriptionCode", item.PrescriptionId);
            ViewData["ProductId"] = new SelectList(_context.Products.FromSqlRaw("SELECT * FROM products"), "Id", "Title", item.ProductId);
            return View(item);
        }

        // GET: PrescriptionItems/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null) return NotFound();

            var item = await _context.PrescriptionItems
                .FromSqlRaw("SELECT * FROM prescription_items WHERE \"Id\" = {0}", id)
                .FirstOrDefaultAsync();

            if (item == null) return NotFound();

            ViewData["ProductId"] = new SelectList(_context.Products.FromSqlRaw("SELECT * FROM products"), "Id", "Title", item.ProductId);
            return View(item);
        }

        // POST: PrescriptionItems/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,PrescriptionId,ProductId,Quantity,DispensedQuantity")] PrescriptionItem item)
        {
            if (id != item.Id) return NotFound();

            if (ModelState.IsValid)
            {
                await _context.Database.ExecuteSqlRawAsync(@"
                    UPDATE prescription_items 
                    SET ""ProductId"" = {0}, ""Quantity"" = {1}, ""DispensedQuantity"" = {2}
                    WHERE ""Id"" = {3}",
                        item.ProductId,
                        item.Quantity,
                        item.DispensedQuantity,
                        item.Id);

                return RedirectToAction("Details", "Prescriptions", new { id = item.PrescriptionId });
            }

            ViewData["ProductId"] = new SelectList(_context.Products.FromSqlRaw("SELECT * FROM products"), "Id", "Title", item.ProductId);
            return View(item);
        }

        // POST: PrescriptionItems/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(long id, long prescriptionId)
        {
            await _context.Database.ExecuteSqlRawAsync("DELETE FROM prescription_items WHERE \"Id\" = {0}", id);
            return RedirectToAction("Details", "Prescriptions", new { id = prescriptionId });
        }
    }

}
