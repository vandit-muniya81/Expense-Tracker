using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Expense_Tracker.Models;

namespace Expense_Tracker.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Category
        public async Task<IActionResult> Index()
        {
            // Get the logged-in user's ID
            var userId = Convert.ToInt32(User.FindFirst("loginId")?.Value);

            // Query and display only the categories belonging to the logged-in user
            var categories = await _context.Categories
                                           .Where(c => c.UserId == userId)
                                           .ToListAsync();
            return View(categories);
        }

        // GET: Category/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Get the logged-in user's ID
            var userId = Convert.ToInt32(User.FindFirst("loginId")?.Value);

            // Find the category that belongs to the user and matches the ID
            var category = await _context.Categories
                                         .FirstOrDefaultAsync(c => c.CategoryId == id && c.UserId == userId);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // GET: Category/Create
        public IActionResult Create()
        {
            return View(new Category());
        }

        // POST: Category/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CategoryId,Title,Icon,Type")] Category category)
        {
            // Check if the model state is valid
            if (ModelState.IsValid)
            {
                // Get the logged-in user's ID from the claims (similar to TransactionController)
                var userId = Convert.ToInt32(User.FindFirst("loginId")?.Value);

                // Assign the userId to the category
                category.UserId = userId;

                // Add the category to the database
                _context.Add(category);
                await _context.SaveChangesAsync();

                // Redirect to the Index action after successful creation
                return RedirectToAction(nameof(Index));
            }

            // If the model state is not valid, return the view with the current category data
            return View(category);
        }

        // GET: Category/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Get the logged-in user's ID
            var userId = Convert.ToInt32(User.FindFirst("loginId")?.Value);

            // Find the category that belongs to the user and matches the ID
            var category = await _context.Categories
                                         .FirstOrDefaultAsync(c => c.CategoryId == id && c.UserId == userId);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Category/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CategoryId,Title,Icon,Type")] Category category)
        {
            // Ensure the category being edited belongs to the logged-in user
            var userId = Convert.ToInt32(User.FindFirst("loginId")?.Value);

            // Find the original category that belongs to the user and matches the ID
            var originalCategory = await _context.Categories
                                                 .FirstOrDefaultAsync(c => c.CategoryId == id && c.UserId == userId);

            if (originalCategory == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Update the original category's fields with new values
                    originalCategory.Title = category.Title;
                    originalCategory.Icon = category.Icon;
                    originalCategory.Type = category.Type;

                    _context.Update(originalCategory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.CategoryId))
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
            return View(category);
        }

        // GET: Category/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Get the logged-in user's ID
            var userId = Convert.ToInt32(User.FindFirst("loginId")?.Value);

            // Find the category that belongs to the user and matches the ID
            var category = await _context.Categories
                                         .FirstOrDefaultAsync(c => c.CategoryId == id && c.UserId == userId);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Category/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Get the logged-in user's ID
            var userId = Convert.ToInt32(User.FindFirst("loginId")?.Value);

            // Find the category that belongs to the user and matches the ID
            var category = await _context.Categories
                                         .FirstOrDefaultAsync(c => c.CategoryId == id && c.UserId == userId);

            if (category != null)
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
        {
            // Check if the category exists for the current user
            var userId = Convert.ToInt32(User.FindFirst("loginId")?.Value);
            return _context.Categories?.Any(c => c.CategoryId == id && c.UserId == userId) ?? false;
        }
    }
}
