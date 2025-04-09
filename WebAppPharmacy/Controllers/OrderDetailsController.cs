using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using WebAppPharmacy;
using WebAppPharmacy.Models;
using WebAppPharmacy.Models.VM;

namespace WebAppPharmacy.Controllers
{
    public class OrderDetailsController : Controller
    {
        private readonly AppDbContext _context;

        public OrderDetailsController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var sql = "SELECT od.\"Id\", p.\"Title\" AS \"ProductTitle\", od.\"OrderId\", od.\"Quantity\", od.\"Price\", s.\"StatusName\" AS \"Status\" FROM order_details od JOIN products p ON od.\"ProductId\" = p.\"Id\" JOIN order_detail_statuses s ON od.\"StatusId\" = s.\"Id\"";

            var result = await _context.Set<OrderDetailViewModel>().FromSqlRaw(sql).ToListAsync();
            return View(result);
        }

        public async Task<IActionResult> Details(long? id)
        {
            if (id == null) return NotFound();

            var item = await _context.OrderDetails
                .FromSqlRaw("SELECT * FROM order_details WHERE \"Id\" = {0}", id)
                .FirstOrDefaultAsync();

            if (item == null) return NotFound();

            item.Product = await _context.Products.FindAsync(item.ProductId);
            item.Order = await _context.SupplierOrders.FindAsync(item.OrderId);
            item.Status = await _context.OrderDetailStatuses.FindAsync(item.StatusId);

            return View(item);
        }

        public IActionResult Create()
        {
            ViewData["ProductId"] = new SelectList(_context.Products.FromSqlRaw("SELECT * FROM products"), "Id", "Title");
            ViewData["OrderId"] = new SelectList(_context.SupplierOrders.FromSqlRaw("SELECT * FROM supplier_orders"), "Id", "Id");
            ViewData["StatusId"] = new SelectList(_context.OrderDetailStatuses.FromSqlRaw("SELECT * FROM order_detail_statuses"), "Id", "StatusName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductId,OrderId,Quantity,Price,StatusId")] OrderDetail item)
        {
            if (ModelState.IsValid)
            {
                var sql = "INSERT INTO order_details (\"ProductId\", \"OrderId\", \"Quantity\", \"Price\", \"StatusId\") VALUES({0}, {1}, {2}, {3}, {4})";
    
            await _context.Database.ExecuteSqlRawAsync(sql,
                item.ProductId, item.OrderId, item.Quantity, item.Price, item.StatusId);

                return RedirectToAction(nameof(Index));
            }

            ViewData["ProductId"] = new SelectList(_context.Products.FromSqlRaw("SELECT * FROM products"), "Id", "Title", item.ProductId);
            ViewData["OrderId"] = new SelectList(_context.SupplierOrders.FromSqlRaw("SELECT * FROM supplier_orders"), "Id", "Id", item.OrderId);
            ViewData["StatusId"] = new SelectList(_context.OrderDetailStatuses.FromSqlRaw("SELECT * FROM order_detail_statuses"), "Id", "StatusName", item.StatusId);
            return View(item);
        }

        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null) return NotFound();

            var item = await _context.OrderDetails
                .FromSqlRaw("SELECT * FROM order_details WHERE \"Id\" = {0}", id)
                .FirstOrDefaultAsync();

            if (item == null) return NotFound();

            ViewData["ProductId"] = new SelectList(_context.Products.FromSqlRaw("SELECT * FROM products"), "Id", "Title", item.ProductId);
            ViewData["OrderId"] = new SelectList(_context.SupplierOrders.FromSqlRaw("SELECT * FROM supplier_orders"), "Id", "Id", item.OrderId);
            ViewData["StatusId"] = new SelectList(_context.OrderDetailStatuses.FromSqlRaw("SELECT * FROM order_detail_statuses"), "Id", "StatusName", item.StatusId);
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,ProductId,OrderId,Quantity,Price,StatusId")] OrderDetail item)
        {
            if (id != item.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var sql = "UPDATE order_details SET \"ProductId\" = {0}, \"OrderId\" = {1},  \"Quantity\" = {2}, \"Price\" = {3}, \"StatusId\" = {4} WHERE \"Id\" = {5}";
                
                await _context.Database.ExecuteSqlRawAsync(sql,
                    item.ProductId, item.OrderId, item.Quantity, item.Price, item.StatusId, item.Id);

                return RedirectToAction(nameof(Index));
            }

            ViewData["ProductId"] = new SelectList(_context.Products.FromSqlRaw("SELECT * FROM products"), "Id", "Title", item.ProductId);
            ViewData["OrderId"] = new SelectList(_context.SupplierOrders.FromSqlRaw("SELECT * FROM supplier_orders"), "Id", "Id", item.OrderId);
            ViewData["StatusId"] = new SelectList(_context.OrderDetailStatuses.FromSqlRaw("SELECT * FROM order_detail_statuses"), "Id", "StatusName", item.StatusId);
            return View(item);
        }

        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null) return NotFound();

            var item = await _context.OrderDetails
                .FromSqlRaw("SELECT * FROM order_details WHERE \"Id\" = {0}", id)
                .FirstOrDefaultAsync();

            if (item == null) return NotFound();

            item.Product = await _context.Products.FindAsync(item.ProductId);
            item.Order = await _context.SupplierOrders.FindAsync(item.OrderId);
            item.Status = await _context.OrderDetailStatuses.FindAsync(item.StatusId);

            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            await _context.Database.ExecuteSqlRawAsync("DELETE FROM order_details WHERE \"Id\" = {0}", id);
            return RedirectToAction(nameof(Index));
        }

        private bool OrderDetailExists(long id)
        {
            return _context.OrderDetails
                .FromSqlRaw("SELECT * FROM order_details WHERE \"Id\" = {0}", id)
                .Any();
        }
    }

}

