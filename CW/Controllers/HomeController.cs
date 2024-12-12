using CW.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CW.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var products = _context.Products.ToList(); 
            return View(products); 
        }
    }

}
