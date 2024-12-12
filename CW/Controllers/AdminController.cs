using CW.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CW.Controllers
{
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;

        private bool IsAdmin()
        {
            var isAdmin = HttpContext.Session.GetString("IsAdmin");
            return isAdmin != null && bool.Parse(isAdmin);
        }

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        public static string TranslateStatus(string status)
        {
            return status switch
            {
                "Pending" => "Очікується",
                "Shipped" => "Відправлено",
                "Delivered" => "Доставлено",
                "Cancelled" => "Скасовано",
                _ => "Невідомий статус"
            };
        }




        [HttpGet]
        public IActionResult Orders()
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Account");
            }

            var orders = _context.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .Include(o => o.User)
                .ToList();

            ViewBag.StatusTranslations = new Dictionary<string, string>
            {
                { "Pending", "Очікується" },
                { "Shipped", "Відправлено" },
                { "Delivered", "Доставлено" },
                { "Cancelled", "Скасовано" }
            };

            ViewBag.StatusOptions = new List<SelectListItem>
            {
                new SelectListItem { Value = "Pending", Text = "Очікується" },
                new SelectListItem { Value = "Shipped", Text = "Відправлено" },
                new SelectListItem { Value = "Delivered", Text = "Доставлено" },
                new SelectListItem { Value = "Cancelled", Text = "Скасовано" }
            };

            return View(orders);
        }

        [HttpPost]
        public IActionResult UpdateOrderStatus(int orderId, string status)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Account");
            }

            var order = _context.Orders.FirstOrDefault(o => o.Id == orderId);
            if (order == null)
            {
                return NotFound("Замовлення не знайдено.");
            }

            
            order.Status = status;
            _context.SaveChanges();

            return RedirectToAction("Orders");
        }

        [HttpGet]
        public IActionResult OrderDetails(int id)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Account");
            }

            var order = _context.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .Include(o => o.User)
                .FirstOrDefault(o => o.Id == id);

            if (order == null)
            {
                return NotFound("Замовлення не знайдено.");
            }

            return View(order);
        }
    }
}
