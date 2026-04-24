using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using WebApiTestBook.Services.Interfaces;

namespace WebApiTestBook.Services
{
    public class CurrentUserService: ICurrentUserService
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public CurrentUserService(
            IHttpContextAccessor httpContextAccessor
            )
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        private ClaimsPrincipal User => httpContextAccessor.HttpContext?.User!;

        public string userId => User.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? string.Empty;

        public string Email => User?.FindFirst(ClaimTypes.Email)?.Value;

        public string Role =>
            User?.FindFirst(ClaimTypes.Role)?.Value;

        public bool IsAuthenticated =>
            User?.Identity?.IsAuthenticated ?? false;
    }
}
