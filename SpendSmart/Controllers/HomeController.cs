using Microsoft.AspNetCore.Mvc;
using SpendSmart.Models;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace SpendSmart.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly SpendSmartDbContext _context;

        private readonly UserDbContext _userContext;


        public HomeController(ILogger<HomeController> logger, SpendSmartDbContext context, UserDbContext userContext)
        {
            _logger = logger;
            _context = context;
            _userContext = userContext;
        }

        [AllowAnonymous]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index");
            }
            return View();
        }

        [AllowAnonymous]
        public IActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> manageAuth(string Username, string Password)
        {
            // Create admin user if it doesn't exist
            var adminUser = await _userContext.Users.FirstOrDefaultAsync(u => u.Username == "admin");
            if (adminUser == null)
            {
                UserHandle userHandle = new UserHandle(_userContext);
                await userHandle.Register("admin", "admin", "Admin");
            }

            UserHandle loginHandle = new UserHandle(_userContext);
            var role = await loginHandle.Login(Username, Password);
            
            if (role == null)
            {
                // Login eșuat
                return RedirectToAction("Login");
            }
            
            // Creăm claims pentru autentificare
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, Username),
                new Claim(ClaimTypes.Role, role)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true // Cookie persistent
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);
            
            // Login reușit
            return RedirectToAction("Index");
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> manageRegister(string Username, string Password, string ConfirmPassword)
        {
            if (Password != ConfirmPassword)
            {
                return RedirectToAction("Register");
            }

            UserHandle userHandle = new UserHandle(_userContext);
            var success = await userHandle.Register(Username, Password);
            
            if (!success)
            {
                // Înregistrare eșuată
                return RedirectToAction("Register");
            }
            
            // Înregistrare reușită, redirecționăm la login
            return RedirectToAction("Login");
        }

        [Authorize]
        public IActionResult allUsers()
        {
            var users = _userContext.Users.ToList();
            return View(users);
        }

        [Authorize]
        public IActionResult Expenses()
        {
            var allExpenses = _context.Expenses.ToList();
            var totalExpenses = allExpenses.Sum(e => e.Value);
            ViewBag.Expenses = totalExpenses; // suma totala
            return View(allExpenses);
        }

        [Authorize]
        public IActionResult CreateEditExpense(int? id)
        {
            if(id != null)
            {
                var expenseInDb = _context.Expenses.FirstOrDefault(e => e.Id == id);
                return View(expenseInDb);
            }
            return View();
        }

        [Authorize]
        public IActionResult DeleteExpense(int id)
        {
            var expenseinDb = _context.Expenses.Find(id);
            
            // Verificăm dacă cheltuiala există
            if (expenseinDb == null)
            {
                return NotFound();
            }

            _context.Expenses.Remove(expenseinDb);
            _context.SaveChanges();

            return RedirectToAction("Expenses");
        }

        [Authorize]
        public IActionResult CreateEditExpenseForm(Expense model)
        {
            if (!ModelState.IsValid)
            {
                return View("CreateEditExpense", model);
            }

            if (model.Id == 0)
            {
                _context.Expenses.Add(model);
            }
            else
            {
                var existingExpense = _context.Expenses.Find(model.Id);
                if (existingExpense == null)
                {
                    return NotFound();
                }
                _context.Entry(existingExpense).CurrentValues.SetValues(model);
            }
            _context.SaveChanges();
            return RedirectToAction("Expenses");
        }

        [Authorize]
        public IActionResult Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login");
            }
            return View();
        }

        [Authorize]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // Adaugă o metodă pentru logout
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            // Forțăm ștergerea cookie-ului de autentificare
            return RedirectToAction("Login");
        }
    }
}
