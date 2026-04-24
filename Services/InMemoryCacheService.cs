using WebApiTestBook.Services.Interfaces;

namespace WebApiTestBook.Services
{
    public class InMemoryCacheService: ICacheService
    {
        private readonly Dictionary<string, string> _cache = new();
        public void Set(string key, string Value)
        {
            _cache[key] = Value;
        }

        public string? Get(string key)
        {
            _cache.TryGetValue(key, out var value);
            return value;
        }
    }
}
