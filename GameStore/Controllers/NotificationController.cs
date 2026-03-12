using GameStore.Data;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.Controllers
{
    public class NotificationController : Controller
    {
        private readonly AppDbContext _context;

        public NotificationController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Unread()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return Json(new List<object>());

            var notifications = _context.Notifications
                .Where(x => x.UserId == userId && !x.IsRead)
                .OrderByDescending(x => x.CreateAt)
                .Select(x => new
                {
                    x.Id,
                    x.Message
                })
                .ToList();

            return Json(notifications);
        }

        public IActionResult UnreadCount()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return Json(new { count = 0 });

            var count = _context.Notifications
                .Count(x => x.UserId == userId && !x.IsRead);

            return Json(new { count });
        }

        [HttpPost]
        public IActionResult MarkRead()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return Ok();

            var notifications = _context.Notifications
                .Where(x => x.UserId == userId && !x.IsRead)
                .ToList();

            foreach (var n in notifications)
            {
                n.IsRead = true;
            }

            _context.SaveChanges();

            return Ok();
        }
    }
}