using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebAppPharmacy;
using WebAppPharmacy.Models.Dictionaries;

namespace WebAppPharmacy.Controllers
{
    public class OrderDetailStatusController : Controller
    {
        private readonly AppDbContext _context;

        public OrderDetailStatusController(AppDbContext context)
        {
            _context = context;
        }

        // GET: OrderDetailStatus
        public async Task<IActionResult> Index()
        {
            var statuses = await _context.OrderDetailStatuses
                .FromSqlRaw("SELECT * FROM order_detail_statuses")
                .ToListAsync();
            return View(statuses);
        }

        // GET: OrderDetailStatus/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
                return NotFound();

            var status = await _context.OrderDetailStatuses
                .FromSqlRaw("SELECT * FROM order_detail_statuses WHERE \"Id\" = {0}", id)
                .FirstOrDefaultAsync();

            if (status == null)
                return NotFound();

            return View(status);
        }

        // GET: OrderDetailStatus/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: OrderDetailStatus/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("StatusName")] OrderDetailStatus orderDetailStatus)
        {
            if (ModelState.IsValid)
            {
                var sql = "INSERT INTO order_detail_statuses (\"StatusName\") VALUES ({0})";
                await _context.Database.ExecuteSqlRawAsync(sql, orderDetailStatus.StatusName);
                return RedirectToAction(nameof(Index));
            }
            return View(orderDetailStatus);
        }

        // GET: OrderDetailStatus/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
                return NotFound();

            var status = await _context.OrderDetailStatuses
                .FromSqlRaw("SELECT * FROM order_detail_statuses WHERE \"Id\" = {0}", id)
                .FirstOrDefaultAsync();

            if (status == null)
                return NotFound();

            return View(status);
        }

        // POST: OrderDetailStatus/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,StatusName")] OrderDetailStatus orderDetailStatus)
        {
            if (id != orderDetailStatus.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var sql = "UPDATE order_detail_statuses SET \"StatusName\" = {0} WHERE \"Id\" = {1}";
                    await _context.Database.ExecuteSqlRawAsync(sql, orderDetailStatus.StatusName, orderDetailStatus.Id);
                }
                catch (DbUpdateConcurrencyException)
                {
                    var exists = await _context.OrderDetailStatuses
                        .FromSqlRaw("SELECT * FROM order_detail_statuses WHERE \"Id\" = {0}", orderDetailStatus.Id)
                        .AnyAsync();

                    if (!exists)
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(orderDetailStatus);
        }

        // GET: OrderDetailStatus/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
                return NotFound();

            var status = await _context.OrderDetailStatuses
                .FromSqlRaw("SELECT * FROM order_detail_statuses WHERE \"Id\" = {0}", id)
                .FirstOrDefaultAsync();

            if (status == null)
                return NotFound();

            return View(status);
        }

        // POST: OrderDetailStatus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var sql = "DELETE FROM order_detail_statuses WHERE \"Id\" = {0}";
            await _context.Database.ExecuteSqlRawAsync(sql, id);
            return RedirectToAction(nameof(Index));
        }

        private bool OrderDetailStatusExists(long id)
        {
            return _context.OrderDetailStatuses
                .FromSqlRaw("SELECT * FROM order_detail_statuses WHERE \"Id\" = {0}", id)
                .Any();
        }
    }
}
