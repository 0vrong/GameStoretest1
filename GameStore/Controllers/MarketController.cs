using GameStore.Data;
using GameStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Controllers
{
    public class MarketController : Controller
    {
        private readonly AppDbContext _context;

        public MarketController(AppDbContext context)
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
            var listings = _context.MarketListings
                .Include(x => x.Item)
                .ThenInclude(x => x.Game)
                .Include(x => x.Seller)
                .Where(x => x.Status == "Active")
                .ToList();

            return View(listings);
        }

        [HttpPost]
        public IActionResult SellItem(int itemId, int price)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login", "Account");

            var userItem = _context.UserItems
                .FirstOrDefault(x => x.UserId == userId && x.ItemId == itemId);

            if (userItem == null)
                return RedirectBackWithError("Предмет не найден в инвентаре.");

            var listing = new MarketListing
            {
                ItemId = itemId,
                SellerId = userId.Value,
                Price = price,
                Status = price > 500 ? "Pending" : "Active"
            };

            _context.MarketListings.Add(listing);
            _context.UserItems.Remove(userItem);
            _context.SaveChanges();

            TempData["SuccessMessage"] = price > 500
                ? "Предмет отправлен на подтверждение."
                : "Предмет выставлен на торговую площадку.";

            return RedirectToAction("Inventory", "User");
        }

        [HttpPost]
        public IActionResult Buy(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login", "Account");

            var listing = _context.MarketListings
                .Include(x => x.Item)
                .ThenInclude(x => x.Game)
                .Include(x => x.Seller)
                .FirstOrDefault(x => x.Id == id && x.Status == "Active");

            if (listing == null)
                return RedirectBackWithError("Лот не найден.");

            var buyer = _context.Users.FirstOrDefault(x => x.Id == userId);
            var seller = _context.Users.FirstOrDefault(x => x.Id == listing.SellerId);

            if (buyer == null || seller == null)
                return RedirectBackWithError("Ошибка обработки покупки.");

            if (buyer.Id == seller.Id)
                return RedirectBackWithError("Нельзя купить свой же предмет.");

            if (buyer.Balace < listing.Price)
                return RedirectBackWithError("Недостаточно средств для покупки предмета.");

            buyer.Balace -= listing.Price;
            seller.Balace += listing.Price;

            _context.UserItems.Add(new UserItem
            {
                UserId = buyer.Id,
                ItemId = listing.ItemId
            });

            var notification = new Notification
            {
                UserId = seller.Id,
                Message = "Ваш предмет купили за " + listing.Price + " ₽",
                CreateAt = DateTime.Now,
                IsRead = false
            };

            _context.Notifications.Add(notification);
            _context.MarketListings.Remove(listing);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Предмет успешно куплен.";
            return RedirectToAction("Inventory", "User");
        }

        public IActionResult Pending()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return Json(new List<object>());

            var listings = _context.MarketListings
                .Include(x => x.Item)
                .ThenInclude(x => x.Game)
                .Where(x => x.SellerId == userId && x.Status == "Pending")
                .Select(x => new
                {
                    x.Id,
                    ItemName = x.Item.Name,
                    GameTitle = x.Item.Game.Title,
                    x.Price
                })
                .ToList();

            return Json(listings);
        }

        public IActionResult PendingCount()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return Json(new { count = 0 });

            var count = _context.MarketListings
                .Count(x => x.SellerId == userId && x.Status == "Pending");

            return Json(new { count });
        }

        [HttpPost]
        public IActionResult Confirm(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return Unauthorized();

            var listing = _context.MarketListings
                .FirstOrDefault(x => x.Id == id && x.SellerId == userId && x.Status == "Pending");

            if (listing == null)
                return NotFound();

            listing.Status = "Active";
            _context.SaveChanges();

            return Ok();
        }

        [HttpPost]
        public IActionResult Cancel(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return Unauthorized();

            var listing = _context.MarketListings
                .FirstOrDefault(x => x.Id == id && x.SellerId == userId && x.Status == "Pending");

            if (listing == null)
                return NotFound();

            _context.UserItems.Add(new UserItem
            {
                UserId = userId.Value,
                ItemId = listing.ItemId
            });

            _context.MarketListings.Remove(listing);
            _context.SaveChanges();

            return Ok();
        }
    }
}