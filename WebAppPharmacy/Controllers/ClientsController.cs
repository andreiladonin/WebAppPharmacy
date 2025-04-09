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
    public class ClientsController : Controller
    {
        private readonly AppDbContext _context;

        public ClientsController(AppDbContext context)
        {
            _context = context;
        }
        // GET: Clients
        public async Task<IActionResult> Index()
        {
            var clients = await _context.Clients
                .FromSqlRaw("SELECT * FROM clients")
                .ToListAsync();
            return View(clients);
        }

        // GET: Clients/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
                return NotFound();

            var client = await _context.Clients
                .FromSqlRaw("SELECT * FROM clients WHERE \"Id\" = {0}", id)
                .FirstOrDefaultAsync();

            if (client == null)
                return NotFound();

            return View(client);
        }

        // GET: Clients/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Clients/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FullName,DateBirthday,Email,Phone")] Client client)
        {
            if (ModelState.IsValid)
            {
                var sql = "INSERT INTO clients (\"FullName\", \"DateBirthday\", \"Email\", \"Phone\") VALUES ({0}, {1}, {2}, {3})";
                await _context.Database.ExecuteSqlRawAsync(sql, client.FullName, client.DateBirthday, client.Email, client.Phone);
                return RedirectToAction(nameof(Index));
            }
            return View(client);
        }

        // GET: Clients/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
                return NotFound();

            var client = await _context.Clients
                .FromSqlRaw("SELECT * FROM clients WHERE \"Id\" = {0}", id)
                .FirstOrDefaultAsync();

            if (client == null)
                return NotFound();

            return View(client);
        }

        // POST: Clients/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,FullName,DateBirthday,Email,Phone")] Client client)
        {
            if (id != client.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var sql = "UPDATE clients SET \"FullName\" = {0}, \"DateBirthday\" = {1}, \"Email\" = {2}, \"Phone\" = {3} WHERE \"Id\" = {4}";
                    await _context.Database.ExecuteSqlRawAsync(sql, client.FullName, client.DateBirthday, client.Email, client.Phone, client.Id);
                }
                catch (DbUpdateConcurrencyException)
                {
                    var exists = await _context.Clients
                        .FromSqlRaw("SELECT * FROM clients WHERE \"Id\" = {0}", client.Id)
                        .AnyAsync();

                    if (!exists)
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(client);
        }

        // GET: Clients/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
                return NotFound();

            var client = await _context.Clients
                .FromSqlRaw("SELECT * FROM clients WHERE \"Id\" = {0}", id)
                .FirstOrDefaultAsync();

            if (client == null)
                return NotFound();

            return View(client);
        }

        // POST: Clients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var sql = "DELETE FROM clients WHERE \"Id\" = {0}";
            await _context.Database.ExecuteSqlRawAsync(sql, id);
            return RedirectToAction(nameof(Index));
        }
    }
}
