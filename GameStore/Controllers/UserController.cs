using GameStore.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Controllers
{
    public class UserController : Controller
    {
        private readonly AppDbContext _context;

        public UserController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Liblary()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login", "Account");

            var games = _context.UserGames
                .Where(x => x.UserId == userId)
                .Include(x => x.Game)
                .Select(x => x.Game)
                .ToList();

            return View(games);
        }
        public IActionResult Inventory()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login", "Account");

            var items = _context.UserItems
                .Where(x => x.UserId == userId)
                .Include(x => x.Item)
                .ThenInclude(x => x.Game)
                .ToList();

            return View(items);
        }
    }
}