namespace WebApiTestBook.Services.Interfaces
{
    public interface ICacheService
    {
        void Set(string key, string value);
        string? Get(string key);
    }
}
