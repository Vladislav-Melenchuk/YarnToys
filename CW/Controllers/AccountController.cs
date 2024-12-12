using CW.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CW.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        // Сторінка реєстрації
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            
            if (_context.Users.Any(u => u.Username == model.Username))
            {
                ModelState.AddModelError(nameof(model.Username), "Користувач із таким ім'ям уже існує.");
            }

            
            if (!ModelState.IsValid)
            {
                return View(model); 
            }

            
            var user = new User
            {
                Username = model.Username,
                PhoneNumber = model.PhoneNumber,
                Password = model.Password,
                IsAdmin = false
            };

            
            _context.Users.Add(user);
            _context.SaveChanges();

            
            return RedirectToAction("Login");
        }

        // Сторінка логіна
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password, bool rememberMe)
        {
            var user = _context.Users.SingleOrDefault(u => u.Username == username && u.Password == password);
            if (user != null)
            {
                HttpContext.Session.SetInt32("UserId", user.Id);
                HttpContext.Session.SetString("Username", user.Username);
                HttpContext.Session.SetString("IsAdmin", user.IsAdmin.ToString());

                if (rememberMe)
                {
                    var cookieOptions = new CookieOptions
                    {
                        Expires = DateTime.Now.AddDays(7),
                        HttpOnly = true,
                        Secure = HttpContext.Request.IsHttps
                    };

                    Response.Cookies.Append("UserId", user.Id.ToString(), cookieOptions);
                    Response.Cookies.Append("Username", user.Username, cookieOptions);
                    Response.Cookies.Append("IsAdmin", user.IsAdmin.ToString(), cookieOptions);
                }

                return RedirectToAction("Index", "Home");
            }

            
            ViewBag.ErrorMessage = "Неправильне ім'я користувача або пароль.";
            return View();
        }

        // Профіль користувача
        public IActionResult Profile()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                if (Request.Cookies.ContainsKey("UserId"))
                {
                    userId = int.Parse(Request.Cookies["UserId"]);
                    HttpContext.Session.SetInt32("UserId", userId.Value);
                    HttpContext.Session.SetString("Username", Request.Cookies["Username"]);
                }
                else
                {
                    return RedirectToAction("Login");
                }
            }

            var user = _context.Users.Find(userId);
            return View(user);
        }

        // Сторінка історії замовлення
        [HttpGet]
        public IActionResult OrderHistory()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login");

            var orders = _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.Items)
                .ThenInclude(oi => oi.Product)
                .ToList();

            return View(orders);
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login");
            }

            return View();
        }

        [HttpPost]
        public IActionResult ChangePassword(string oldPassword, string newPassword, string confirmPassword)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login");
            }

            var user = _context.Users.Find(userId);
            if (user == null)
            {
                return NotFound();
            }

            if (user.Password != oldPassword)
            {
                ModelState.AddModelError("", "Старий пароль неправильний.");
                return View();
            }

            if (newPassword != confirmPassword)
            {
                ModelState.AddModelError("", "Паролі не збігаються.");
                return View();
            }

            user.Password = newPassword;
            _context.SaveChanges();

            return RedirectToAction("Profile");
        }

        // Редагування профілю
        [HttpGet]
        public IActionResult ProfileEdit()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login");
            }

            var user = _context.Users.Find(userId);
            if (user == null)
            {
                return NotFound();
            }

            var model = new EditProfileViewModel
            {
                Username = user.Username,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult ProfileEdit(EditProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var user = _context.Users.Find(userId);
            if (user == null)
            {
                return NotFound();
            }

            
            if (user.Password != model.CurrentPassword)
            {
                ModelState.AddModelError("CurrentPassword", "Неправильний пароль.");
                return View(model);
            }

            
            var existingUser = _context.Users.FirstOrDefault(u => u.Username == model.Username && u.Id != userId);
            if (existingUser != null)
            {
                ModelState.AddModelError("Username", "Це ім'я користувача вже зайнято.");
                return View(model);
            }

            
            user.Username = model.Username;
            user.PhoneNumber = model.PhoneNumber;
            user.Email = model.Email;

            _context.SaveChanges();

            return RedirectToAction("Profile");
        }

        // Вихід з акаунту
        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
