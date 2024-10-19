using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Expense_Tracker.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Expense_Tracker.Controllers
{
    [Authorize]
    public class TransactionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TransactionController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Statement()
        {
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Login", "Access");
        }

        // GET: Transaction
        public async Task<IActionResult> Index()
        {
            // Check if the user is authenticated
            if (User.Identity.IsAuthenticated)
            {
                // Get the logged-in user's ID from the claims
                var userId = User.FindFirst("loginId")?.Value;

                if (!string.IsNullOrEmpty(userId))
                {
                    // Query transactions that belong to the logged-in user
                    var applicationDbContext = _context.Transactions
                        .Include(t => t.Category)
                        .Where(t => t.userId == int.Parse(userId)); // Filter by user ID

                    // Return the filtered transactions to the view
                    return View(await applicationDbContext.ToListAsync());
                }
            }

            // If not authenticated or no user ID is found, redirect to the login page
            return RedirectToAction("Login", "Account");
        }


        // GET: Transaction/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Transactions == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transactions
                .Include(t => t.Category)
                .FirstOrDefaultAsync(m => m.TransactionId == id);
            if (transaction == null)
            {
                return NotFound();
            }

            return View(transaction);
        }

        // GET: Transaction/Create
        public IActionResult Create()
        {
            string expenseValue = "Expense";
            var userId = Convert.ToInt32(User.FindFirst("loginId")?.Value);

            // Calculate the total sum of transactions for the logged-in user
            int totalSum = _context.Transactions
                                   .Where(t => t.userId == userId) // Filter by user ID
                                   .Sum(t => t.Amount);
           
            // Calculate the sum of transactions in the last 30 days for the logged-in user and expense category
            DateTime thirtyDaysAgo = DateTime.Now.AddDays(-30);

            int lastMonthSum = _context.Transactions
                                       .Where(t => t.Date >= thirtyDaysAgo && t.userId == userId && t.Category.Type == expenseValue) // Filter by user ID and category type
                                       .Sum(t => t.Amount);

            // Pass the calculated sums to the view using ViewBag
            ViewBag.TotalSum = totalSum;
            ViewBag.LastMonthSum = lastMonthSum;
            var transaction = new Transaction
            {
                Date = DateTime.Now
            };
            PopulateCategories();
            return View(transaction);
        }

        // POST: Transaction/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TransactionId,CategoryId,Amount,Note,Date")] Transaction transaction)
        {
            // Populate the categories for the dropdown
            PopulateCategories();

            // Get the logged-in user's ID from the claims
            var userId = Convert.ToInt32(User.FindFirst("loginId")?.Value);
            // Calculate the total sum of transactions for the logged-in user
            int totalSum = _context.Transactions
                                   .Where(t => t.userId == userId) // Filter by user ID
                                   .Sum(t => t.Amount);

            // Calculate the sum of transactions in the last 30 days for the logged-in user and expense category
            DateTime thirtyDaysAgo = DateTime.Now.AddDays(-30);
            string expenseValue = "Expense";

            int lastMonthSum = _context.Transactions
                                       .Where(t => t.Date >= thirtyDaysAgo && t.userId == userId && t.Category.Type == expenseValue) // Filter by user ID and category type
                                       .Sum(t => t.Amount);

            // Pass the calculated sums to the view using ViewBag
            ViewBag.TotalSum = totalSum;
            ViewBag.LastMonthSum = lastMonthSum;

            // Create a new transaction with the current date
            var transaction1 = new Transaction
            {
                Date = DateTime.Now
            };

            // Validation for CategoryId
            if (transaction.CategoryId <= 0)
            {
                ModelState.AddModelError("CategoryId", "Please select a category.");
                return View(transaction1);
            }

            // Validation for Amount
            if (transaction.Amount <= 0)
            {
                ModelState.AddModelError("Amount", "Please enter a valid amount.");
                return View(transaction1);
            }

            // Validation for Date
            if (transaction.Date == DateTime.MinValue)
            {
                ModelState.AddModelError("Date", "Please enter a Date.");
                return View(transaction1);
            }

            // Create a new transaction object
            var t = new Transaction
            {
                Date = transaction.Date,
                CategoryId = transaction.CategoryId,
                Amount = transaction.Amount,
                Note = transaction.Note,
                userId = userId // Set the userId from the logged-in user
            };

            // Save the transaction to the database
            _context.Add(t);
            await _context.SaveChangesAsync();

            // Redirect to the Index action
            return RedirectToAction(nameof(Index));
        }


        // GET: Transaction/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {

            if (id == null || _context.Transactions == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction == null)
            {
                return NotFound();
            }
            PopulateCategories();
            return View(transaction);
        }

        // POST: Transaction/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TransactionId,CategoryId,Amount,Note,Date")] Transaction transaction)
        {
            PopulateCategories();
            if (id != transaction.TransactionId)
            {
                return NotFound();
            }

            if (transaction.CategoryId <= 0)
            {
                ModelState.AddModelError("CategoryId", "Please select a category.");
                return View(transaction);
            }

            if (transaction.Amount <= 0)
            {
                ModelState.AddModelError("Amount", "Please enter a valid amount.");
                return View(transaction);
            }

            if (transaction.Date == DateTime.MinValue)
            {
                ModelState.AddModelError("Date", "Please enter a Date.");
                return View(transaction);
            }

            try

                {
                 transaction.userId = Convert.ToInt32(User.FindFirst("loginId")?.Value);
                _context.Update(transaction);
              
            await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TransactionExists(transaction.TransactionId))
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

        // GET: Transaction/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Transactions == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transactions
                .Include(t => t.Category)
                .FirstOrDefaultAsync(m => m.TransactionId == id);
            if (transaction == null)
            {
                return NotFound();
            }

            return View(transaction);
        }

        // POST: Transaction/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Transactions == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Transactions'  is null.");
            }
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction != null)
            {
                _context.Transactions.Remove(transaction);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TransactionExists(int id)
        {
          return (_context.Transactions?.Any(e => e.TransactionId == id)).GetValueOrDefault();
        }

        [NonAction]
        public void PopulateCategories()
        {
            // Get the logged-in user's ID
            var userId = Convert.ToInt32(User.FindFirst("loginId")?.Value);

            // Fetch the categories that belong to the logged-in user
            var CategoryCollection = _context.Categories
                                             .Where(c => c.UserId == userId)
                                             .ToList();

            // Add a default category to the collection
            Category DefaultCategory = new Category() { CategoryId = 0, Title = "Choose a Category" };
            CategoryCollection.Insert(0, DefaultCategory);

            // Store the filtered categories in ViewBag
            ViewBag.Categories = CategoryCollection;
        }


        [Route("Transaction/Search")]
        public IActionResult Search(DateTime fromDate, DateTime toDate)
        {
            // Get the logged-in user's ID
            var UserId = Convert.ToInt32(User.FindFirst("loginId")?.Value);

            // Fetch transactions within the date range for the logged-in user
            var transactions = _context.Transactions
                .Where(t => t.userId == UserId && t.Date >= fromDate && t.Date <= toDate)
                .ToList();

            return Json(transactions);
        }

    }
}
