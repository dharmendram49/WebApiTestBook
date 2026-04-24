namespace WebApiTestBook.Services.Interfaces
{
    public class ICurrentUserService
    {
        string UserId { get; }
        string Email { get; }
        string Role { get; }
        bool IsAuthenticated { get; }
    }
}
