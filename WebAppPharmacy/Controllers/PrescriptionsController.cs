using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebAppPharmacy;
using WebAppPharmacy.Models;
using WebAppPharmacy.Models.VM;

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
            var sql = @"
            SELECT p.""Id"", p.""DoctorName"", p.""PrescriptionCode"", p.""IssueDate"", p.""ExpiryDate"",
                   c.""FullName"" AS ""ClientName"", s.""StatusName""
            FROM prescriptions p
            JOIN clients c ON c.""Id"" = p.""ClientId""
            JOIN prescription_statuses s ON s.""Id"" = p.""StatusId""";

            var result = await _context.Set<PrescriptionViewModel>().FromSqlRaw(sql).ToListAsync();
            return View(result);
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

        // GET: Prescriptions/Create
        public IActionResult Create()
        {
            ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "FullName");
            ViewData["StatusId"] = new SelectList(_context.PrescriptionStatuses, "Id", "StatusName");
            return View();
        }

        // POST: Prescriptions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DoctorName,PrescriptionCode,IssueDate,ExpiryDate,ClientId,StatusId")] Prescription prescription)
        {
            if (!ModelState.IsValid)
            {
                ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "FullName", prescription.ClientId);
                ViewData["StatusId"] = new SelectList(_context.PrescriptionStatuses, "Id", "StatusName", prescription.StatusId);
                return View(prescription);
            }
            prescription.IssueDate = DateTime.SpecifyKind(prescription.IssueDate, DateTimeKind.Utc);
            if (prescription.ExpiryDate != null)
                prescription.ExpiryDate = DateTime.SpecifyKind(prescription.ExpiryDate.Value, DateTimeKind.Utc);
            await _context.Database.ExecuteSqlRawAsync(@"
            INSERT INTO prescriptions 
            (""DoctorName"", ""PrescriptionCode"", ""IssueDate"", ""ExpiryDate"", ""ClientId"", ""StatusId"") 
            VALUES ({0}, {1}, {2}, {3}, {4}, {5})",
                prescription.DoctorName,
                prescription.PrescriptionCode,
                prescription.IssueDate,
                prescription.ExpiryDate,
                prescription.ClientId,
                prescription.StatusId);

            return RedirectToAction(nameof(Index));
        }

        // GET: Prescriptions/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null) return NotFound();

            var prescription = await _context.Prescriptions
                .FromSqlRaw("SELECT * FROM prescriptions WHERE \"Id\" = {0}", id)
                .FirstOrDefaultAsync();

            if (prescription == null) return NotFound();

            ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "FullName", prescription.ClientId);
            ViewData["StatusId"] = new SelectList(_context.PrescriptionStatuses, "Id", "StatusName", prescription.StatusId);
            return View(prescription);
        }

        // POST: Prescriptions/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,DoctorName,PrescriptionCode,IssueDate,ExpiryDate,ClientId,StatusId")] Prescription prescription)
        {
            if (id != prescription.Id) return NotFound();

            if (!ModelState.IsValid)
            {
                ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "FullName", prescription.ClientId);
                ViewData["StatusId"] = new SelectList(_context.PrescriptionStatuses, "Id", "StatusName", prescription.StatusId);
                return View(prescription);
            }

            prescription.IssueDate = DateTime.SpecifyKind(prescription.IssueDate, DateTimeKind.Utc);
            if (prescription.ExpiryDate != null)
                prescription.ExpiryDate = DateTime.SpecifyKind(prescription.ExpiryDate.Value, DateTimeKind.Utc);
            await _context.Database.ExecuteSqlRawAsync(@"
            UPDATE prescriptions SET 
            ""DoctorName"" = {0}, ""PrescriptionCode"" = {1}, ""IssueDate"" = {2}, ""ExpiryDate"" = {3}, 
            ""ClientId"" = {4}, ""StatusId"" = {5}
            WHERE ""Id"" = {6}",
                prescription.DoctorName,
                prescription.PrescriptionCode,
                prescription.IssueDate,
                prescription.ExpiryDate,
                prescription.ClientId,
                prescription.StatusId,
                prescription.Id);

            return RedirectToAction(nameof(Index));
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

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            try
            {
                // Начинаем транзакцию для атомарности
                using var transaction = await _context.Database.BeginTransactionAsync();

                // Удаляем все позиции рецепта из таблицы prescription_items
                await _context.Database.ExecuteSqlRawAsync(
                    "DELETE FROM prescription_items WHERE \"PrescriptionId\" = {0}", id);

                // Удаляем сам рецепт из таблицы prescriptions
                var rowsAffected = await _context.Database.ExecuteSqlRawAsync(
                    "DELETE FROM prescriptions WHERE \"Id\" = {0}", id);

                // Проверяем, был ли удален рецепт
                if (rowsAffected == 0)
                {
                    await transaction.RollbackAsync();
                    TempData["Error"] = "Рецепт не найден.";
                    return RedirectToAction("Details", new { id });
                }

                // Фиксируем транзакцию
                await transaction.CommitAsync();

                TempData["Success"] = "Рецепт и его позиции успешно удалены.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // В случае ошибки показываем сообщение
                TempData["Error"] = $"Ошибка при удалении рецепта: {ex.Message}";
                return RedirectToAction("Details", new { id });
            }
        }
    }

}