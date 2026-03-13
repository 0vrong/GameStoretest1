using GameStore.Data;
using GameStore.Models;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.Controllers
{
    public class GamesController : Controller
    {
        private readonly AppDbContext _context;

        public GamesController(AppDbContext context)
        {
            _context = context;
        }

        private IActionResult RedirectBackWithError(string message)
        {
            TempData["ErrorMessage"] = message;

            var referer = Request.Headers["Referer"].ToString();

            if (!string.IsNullOrWhiteSpace(referer))
                return Redirect(referer);

            return RedirectToAction("Index", "Home");
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
                return RedirectBackWithError("Игра не найдена.");

            var user = _context.Users.FirstOrDefault(u => u.Id == userId);

            if (user == null)
                return RedirectBackWithError("Пользователь не найден.");

            var alreadyOwned = _context.UserGames
                .Any(x => x.UserId == userId && x.GameId == id);

            if (alreadyOwned)
                return RedirectBackWithError("У вас уже есть эта игра.");

            if (user.Balace < game.Price)
                return RedirectBackWithError("Недостаточно средств для покупки игры.");

            user.Balace -= game.Price;

            var userGame = new UserGame
            {
                UserId = user.Id,
                GameId = game.Id,
                PurchaseDate = DateTime.Now
            };

            _context.UserGames.Add(userGame);

            var items = _context.Items
                .Where(x => x.GameId == id)
                .ToList();

            foreach (var item in items)
            {
                _context.UserItems.Add(new UserItem
                {
                    UserId = user.Id,
                    ItemId = item.Id
                });
            }

            _context.SaveChanges();

            TempData["SuccessMessage"] = "Игра успешно куплена.";
            return RedirectToAction("Liblary", "User");
        }
    }
}