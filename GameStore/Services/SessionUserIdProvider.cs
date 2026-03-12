using Microsoft.AspNetCore.SignalR;

namespace GameStore.Services
{
    public class SessionUserIdProvider : IUserIdProvider
    {
        public string GetUserId(HubConnectionContext connection)
        {
            var httpContext = connection.GetHttpContext();

            if (httpContext == null)
                return null;

            var userId = httpContext.Session.GetInt32("UserId");

            return userId?.ToString();
        }
    }
}