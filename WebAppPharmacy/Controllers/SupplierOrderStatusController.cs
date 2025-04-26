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
    public class SupplierOrderStatusController : Controller
    {
        private readonly AppDbContext _context;

        public SupplierOrderStatusController(AppDbContext context)
        {
            _context = context;
        }

        // GET: SupplierOrderStatus
        public async Task<IActionResult> Index()
        {
            return View(await _context.SupplierOrderStatuses.ToListAsync());
        }

        // GET: SupplierOrderStatus/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supplierOrderStatus = await _context.SupplierOrderStatuses
                .FirstOrDefaultAsync(m => m.Id == id);
            if (supplierOrderStatus == null)
            {
                return NotFound();
            }

            return View(supplierOrderStatus);
        }

        // GET: SupplierOrderStatus/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: SupplierOrderStatus/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,StatusName")] SupplierOrderStatus supplierOrderStatus)
        {
            if (ModelState.IsValid)
            {
                _context.Add(supplierOrderStatus);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(supplierOrderStatus);
        }

        // GET: SupplierOrderStatus/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supplierOrderStatus = await _context.SupplierOrderStatuses.FindAsync(id);
            if (supplierOrderStatus == null)
            {
                return NotFound();
            }
            return View(supplierOrderStatus);
        }

        // POST: SupplierOrderStatus/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,StatusName")] SupplierOrderStatus supplierOrderStatus)
        {
            if (id != supplierOrderStatus.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(supplierOrderStatus);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SupplierOrderStatusExists(supplierOrderStatus.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(supplierOrderStatus);
        }

        // GET: SupplierOrderStatus/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supplierOrderStatus = await _context.SupplierOrderStatuses
                .FirstOrDefaultAsync(m => m.Id == id);
            if (supplierOrderStatus == null)
            {
                return NotFound();
            }

            return View(supplierOrderStatus);
        }

        // POST: SupplierOrderStatus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var supplierOrderStatus = await _context.SupplierOrderStatuses.FindAsync(id);
            if (supplierOrderStatus != null)
            {
                _context.SupplierOrderStatuses.Remove(supplierOrderStatus);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SupplierOrderStatusExists(long id)
        {
            return _context.SupplierOrderStatuses.Any(e => e.Id == id);
        }
    }
}
