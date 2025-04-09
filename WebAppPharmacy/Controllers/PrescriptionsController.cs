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
    public class PrescriptionsController : Controller
    {
        private readonly AppDbContext _context;

        public PrescriptionsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Prescriptions
        public async Task<IActionResult> Index()
        {
            var prescriptions = await _context.Prescriptions
                .FromSqlRaw(@"SELECT * FROM prescriptions")
                .ToListAsync();

            foreach (var prescription in prescriptions)
            {
                prescription.Client = await _context.Clients
                    .FromSqlRaw("SELECT * FROM clients WHERE \"Id\" = {0}", prescription.ClientId)
                    .FirstOrDefaultAsync();

                prescription.Status = await _context.PrescriptionStatuses
                    .FromSqlRaw("SELECT * FROM prescription_statuses WHERE \"Id\" = {0}", prescription.StatusId)
                    .FirstOrDefaultAsync();
            }

            return View(prescriptions);
        }

        // GET: Prescriptions/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null) return NotFound();

            var prescription = await _context.Prescriptions
                .FromSqlRaw("SELECT * FROM prescriptions WHERE \"Id\" = {0}", id)
                .FirstOrDefaultAsync();

            if (prescription == null) return NotFound();

            prescription.Client = await _context.Clients
                .FromSqlRaw("SELECT * FROM clients WHERE \"Id\" = {0}", prescription.ClientId)
                .FirstOrDefaultAsync();

            prescription.Status = await _context.PrescriptionStatuses
                .FromSqlRaw("SELECT * FROM prescription_statuses WHERE \"Id\" = {0}", prescription.StatusId)
                .FirstOrDefaultAsync();

            prescription.Items = await _context.PrescriptionItems
                .FromSqlRaw("SELECT * FROM prescription_items WHERE \"PrescriptionId\" = {0}", prescription.Id)
                .ToListAsync();

            foreach (var item in prescription.Items)
            {
                item.Product = await _context.Products
                    .FromSqlRaw("SELECT * FROM products WHERE \"Id\" = {0}", item.ProductId)
                    .FirstOrDefaultAsync();
            }

            return View(prescription);
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
                INSERT INTO prescription_items (PrescriptionId, ProductId, Quantity, DispensedQuantity)
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


        // GET: Prescriptions/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null) return NotFound();

            var prescription = await _context.Prescriptions
                .FromSqlRaw("SELECT * FROM prescriptions WHERE \"Id\" = {0}", id)
                .FirstOrDefaultAsync();

            if (prescription == null) return NotFound();

            ViewData["ClientId"] = new SelectList(_context.Clients.FromSqlRaw("SELECT * FROM clients"), "Id", "FullName", prescription.ClientId);
            ViewData["StatusId"] = new SelectList(_context.PrescriptionStatuses.FromSqlRaw("SELECT * FROM prescription_statuses"), "Id", "StatusName", prescription.StatusId);
            return View(prescription);
        }

        // POST: Prescriptions/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,DoctorName,PrescriptionCode,IssueDate,ExpiryDate,ClientId,StatusId")] Prescription prescription)
        {
            if (id != prescription.Id) return NotFound();

            if (ModelState.IsValid)
            {
                await _context.Database.ExecuteSqlRawAsync(@"
                UPDATE prescriptions SET DoctorName = {0}, PrescriptionCode = {1}, IssueDate = {2}, ExpiryDate = {3}, ClientId = {4}, StatusId = {5} WHERE Id = {6}",
                    prescription.DoctorName,
                    prescription.PrescriptionCode,
                    prescription.IssueDate,
                    prescription.ExpiryDate,
                    prescription.ClientId,
                    prescription.StatusId,
                    prescription.Id);

                return RedirectToAction(nameof(Index));
            }

            ViewData["ClientId"] = new SelectList(_context.Clients.FromSqlRaw("SELECT * FROM clients"), "Id", "FullName", prescription.ClientId);
            ViewData["StatusId"] = new SelectList(_context.PrescriptionStatuses.FromSqlRaw("SELECT * FROM prescription_statuses"), "Id", "StatusName", prescription.StatusId);
            return View(prescription);
        }

        // GET: Prescriptions/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null) return NotFound();

            var prescription = await _context.Prescriptions
                .FromSqlRaw("SELECT * FROM prescriptions WHERE \"Id\" = {0}", id)
                .FirstOrDefaultAsync();

            if (prescription == null) return NotFound();

            prescription.Client = await _context.Clients
                .FromSqlRaw("SELECT * FROM clients WHERE \"Id\" = {0}", prescription.ClientId)
                .FirstOrDefaultAsync();

            prescription.Status = await _context.PrescriptionStatuses
                .FromSqlRaw("SELECT * FROM prescription_statuses WHERE \"Id\" = {0}", prescription.StatusId)
                .FirstOrDefaultAsync();

            return View(prescription);
        }


        // POST: Prescriptions/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(long id)
        {
            // Проверка: есть ли позиции рецепта
            var hasItems = await _context.PrescriptionItems
                .FromSqlRaw("SELECT * FROM prescription_items WHERE \"PrescriptionId\" = {0}", id)
                .AnyAsync();

            if (hasItems)
            {
                TempData["Error"] = "Нельзя удалить рецепт, у которого есть связанные позиции.";
                return RedirectToAction("Details", new { id });
            }

            await _context.Database.ExecuteSqlRawAsync("DELETE FROM prescriptions WHERE \"Id\" = {0}", id);
            return RedirectToAction("Index");
        }


        private bool PrescriptionExists(long id)
        {
            return _context.Prescriptions
                .FromSqlRaw("SELECT * FROM prescriptions WHERE \"Id\" = {0}", id)
                .Any();
        }
    }
}