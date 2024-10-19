using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Expense_Tracker.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;


namespace Expense_Tracker.Controllers
{
    public class AccessController : Controller
    {
        private readonly ApplicationDbContext _context;
        public AccessController(ApplicationDbContext context)
        {
            _context = context;
            
        }
        public IActionResult Login()
        {
            ClaimsPrincipal claimUser = HttpContext.User;
            
            if(claimUser.Identity.IsAuthenticated)
            {
                return RedirectToAction("Create", "Transaction");
            }

            return View();
        }
        [HttpPost]

        public async Task<IActionResult> Login(VMLogin user)
        {
            // Check if user data is valid
            if (ModelState.IsValid)
            {
                // Query the database for the user with the provided email
                var dbUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.email);

                if (dbUser != null)
                {
                    // Verify password (assuming passwords are stored in plain text, otherwise use a hashing method)
                    if (dbUser.Password == user.password)
                    {
                        // Create claims for the logged-in user
                     
               
                     List<Claim> claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.NameIdentifier, dbUser.Email),
                    new Claim("loginId", dbUser.UserId.ToString()) // Storing the user ID
                
                };

                        // Create identity and authentication properties
                        ClaimsIdentity identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        AuthenticationProperties authenticationProperties = new AuthenticationProperties()
                        {
                            AllowRefresh = true,
                            IsPersistent = user.keepLoggedIn,
                        };

                        // Sign in the user
                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity), authenticationProperties);

                        // Redirect to the desired page after successful login
                        return RedirectToAction("Create", "Transaction");
                    }
                }

                // If user not found or password doesn't match, show an error message
                ViewData["ValidateMessage"] = "Invalid email or password";
            }

            // Return the view if the model state is invalid or authentication fails
            return View();
        }

        [HttpGet]
        public IActionResult Signup()
        {
            ClaimsPrincipal claimUser = HttpContext.User;

            if (claimUser.Identity.IsAuthenticated)
            {
                return RedirectToAction("Create", "Transaction");
            }

            return View();
        }

        [HttpPost]
        public IActionResult Signup(User model)
        {

            if (ModelState.IsValid)
            {
                _context.Users.Add(model);
                _context.SaveChanges();
                return RedirectToAction("Login");

            }
            

            return View();
        }
    }
}
