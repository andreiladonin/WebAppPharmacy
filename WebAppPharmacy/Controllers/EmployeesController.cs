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
    public class EmployeesController : Controller
    {
        private readonly AppDbContext _context;

        public EmployeesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Employees
        public async Task<IActionResult> Index()
        {
            var employees = await _context.Employees
                .FromSqlRaw("SELECT * FROM employees")
                .ToListAsync();
            return View(employees);
        }

        // GET: Employees/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
                return NotFound();

            var employee = await _context.Employees
                .FromSqlRaw("SELECT * FROM employees WHERE \"Id\" = {0}", id)
                .FirstOrDefaultAsync();

            if (employee == null)
                return NotFound();

            return View(employee);
        }

        // GET: Employees/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Employees/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FullName,Position")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                var sql = "INSERT INTO employees (\"FullName\", \"Position\") VALUES ({0}, {1})";
                await _context.Database.ExecuteSqlRawAsync(sql, employee.FullName, employee.Position);
                return RedirectToAction(nameof(Index));
            }
            return View(employee);
        }

        // GET: Employees/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
                return NotFound();

            var employee = await _context.Employees
                .FromSqlRaw("SELECT * FROM employees WHERE \"Id\" = {0}", id)
                .FirstOrDefaultAsync();

            if (employee == null)
                return NotFound();

            return View(employee);
        }

        // POST: Employees/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,FullName,Position")] Employee employee)
        {
            if (id != employee.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var sql = "UPDATE employees SET \"FullName\" = {0}, \"Position\" = {1} WHERE \"Id\" = {2}";
                    await _context.Database.ExecuteSqlRawAsync(sql, employee.FullName, employee.Position, employee.Id);
                }
                catch (DbUpdateConcurrencyException)
                {
                    var exists = await _context.Employees
                        .FromSqlRaw("SELECT * FROM employees WHERE \"Id\" = {0}", employee.Id)
                        .AnyAsync();

                    if (!exists)
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(employee);
        }

        // GET: Employees/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
                return NotFound();

            var employee = await _context.Employees
                .FromSqlRaw("SELECT * FROM employees WHERE \"Id\" = {0}", id)
                .FirstOrDefaultAsync();

            if (employee == null)
                return NotFound();

            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var sql = "DELETE FROM employees WHERE \"Id\" = {0}";
            await _context.Database.ExecuteSqlRawAsync(sql, id);
            return RedirectToAction(nameof(Index));
        }
    }
}
