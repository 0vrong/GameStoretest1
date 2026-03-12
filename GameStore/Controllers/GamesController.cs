using GameStore.Data;
using GameStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Controllers
{
    public class GamesController : Controller
    {
        private readonly AppDbContext _context;

        public GamesController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var games = _context.Games.ToList();
            return View(games);
        }

        public IActionResult Details(int id)
        {
            var game = _context.Games.FirstOrDefault(g => g.Id == id);

            if (game == null)
                return NotFound();

            return View(game);
        }

        [HttpPost]
        public IActionResult Buy(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login", "Account");

            var game = _context.Games.FirstOrDefault(g => g.Id == id);

            if (game == null)
                return NotFound();

            var user = _context.Users.FirstOrDefault(u => u.Id == userId);

            if (user.Balace < game.Price)
                return Content("Недостаточно средств");

            var alreadyOwned = _context.UserGames
                .Any(x => x.UserId == userId && x.GameId == id);

            if (alreadyOwned)
                return Content("У вас уже есть эта игра");

            user.Balace -= game.Price;

            var userGame = new UserGame
            {
                UserId = user.Id,
                GameId = game.Id,
                PurchaseDate = DateTime.Now
            };

            _context.UserGames.Add(userGame);

            _context.SaveChanges();

            return RedirectToAction("Liblary", "User");
        }
    }
}