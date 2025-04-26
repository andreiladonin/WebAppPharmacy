using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebAppPharmacy.Models.Dictionaries;
using WebAppPharmacy.Models;
using WebAppPharmacy.Models.VM;
using WebAppPharmacy.Models.DTO;
using Microsoft.AspNetCore.Authorization;

namespace WebAppPharmacy.Controllers
{
    [Authorize]
    public class SupplierOrdersController : Controller
    {
        private AppDbContext _context;

        public SupplierOrdersController(AppDbContext context)
        {
            _context = context;
        }

        // GET: SupplierOrderController
        public async Task<IActionResult> Index()
        {
            var orders = await _context.Set<SupplierOrderListItemViewModel>()
                .FromSqlRaw(@"
                    SELECT o.""Id"", o.""OrderDate"", o.""DeliveryDate"", o.""TotalAmount"",
                           s.""Title"" AS SupplierTitle,
                           st.""StatusName"" AS StatusName
                    FROM supplier_orders o
                    JOIN suppliers s ON o.""SupplierId"" = s.""Id""
                    JOIN supplier_order_statuses st ON o.""StatusId"" = st.""Id""
                    ORDER BY o.""OrderDate"" DESC
                ")
                .ToListAsync();

            return View(orders);
        }
        public async Task<IActionResult> Create()
        {
            var suppliers = await _context.Suppliers
                .Select(s => new { s.Id, s.Title })
                .ToListAsync();

            var statuses = await _context.SupplierOrderStatuses
                .Select(s => new { s.Id, s.StatusName })
                .ToListAsync();


            ViewBag.Suppliers = new SelectList(suppliers, "Id", "Title");
            ViewBag.Statuses = new SelectList(statuses, "Id", "StatusName");
            ViewData["Products"] = await _context.Products
               .Select(p => new { id = p.Id, title = p.Title })
               .ToListAsync();
            var model = new SupplierOrderCreateViewModel
            {
                OrderDate = DateTime.Now,
                Details = new List<SupplierDetailCreateViewModel>()
            };

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SupplierOrderCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Повторная загрузка списков
                var suppliers = await _context.Suppliers.FromSqlRaw("SELECT * FROM suppliers").ToListAsync();
                var statuses = await _context.SupplierOrderStatuses.FromSqlRaw("SELECT * FROM supplier_order_statuses").ToListAsync();
                var products = await _context.Products.FromSqlRaw("SELECT * FROM products").ToListAsync();

                ViewData["Suppliers"] = new SelectList(suppliers, "Id", "Title", model.SupplierId);
                ViewData["Statuses"] = new SelectList(statuses, "Id", "StatusName", model.StatusId);
                ViewData["Products"] = products;

                return View(model);
            }

            // Вставляем заказ и получаем ID
            var insertedOrder = await _context.SupplierOrders
                .FromSqlInterpolated($@"
                    INSERT INTO supplier_orders 
                        (order_date, delivery_date, total_amount, supplier_id, status_id)
                    VALUES 
                        ({model.OrderDate}, {model.DeliveryDate}, {model.TotalAmount}, {model.SupplierId}, {model.StatusId})
                    RETURNING *")
                        .FirstAsync();

            // Вставка деталей
            foreach (var detail in model.Details)
            {
                await _context.Database.ExecuteSqlInterpolatedAsync($@"
                    INSERT INTO order_details 
                        (order_id, product_id, quantity, price, status_id)
                    VALUES 
                        ({insertedOrder.Id}, {detail.ProductId}, {detail.Quantity}, {detail.Price}, 1)");
            }

            return RedirectToAction("Details", new { id = insertedOrder.Id });
        }


        // GET: SupplierOrderController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: SupplierOrderController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: SupplierOrderController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: SupplierOrderController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
