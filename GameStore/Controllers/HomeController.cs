using GameStore.Data;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.Controllers
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
            // получаем игры из БД
            var games = _context.Games.ToList();

            // перемешиваем и берем 4 случайные
            var randomGames = games
                .OrderBy(x => Guid.NewGuid())
                .Take(3)
                .ToList();

            // получаем пользователя из сессии
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId != null)
            {
                var user = _context.Users.FirstOrDefault(u => u.Id == userId);

                if (user != null)
                {
                    ViewBag.Username = user.UserName;
                    ViewBag.Balance = user.Balace;
                }
            }

            return View(randomGames);
        }
    }
}