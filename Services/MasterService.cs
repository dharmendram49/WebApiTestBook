using Microsoft.Extensions.Caching.Memory;
using WebApiTestBook.Models;
using WebApiTestBook.Services.Interfaces;

namespace WebApiTestBook.Services
{
    public class MasterService : IMasterService
    {
        private readonly IMemoryCache cache;

        public MasterService(IMemoryCache cache)
        {
            this.cache = cache;
        }
        public IList<MasterData> GetCountries()
        {
            if(!cache.TryGetValue("COUNTRIES", out IList<MasterData> data )){
                data = getCountries();
                cache.Set("COUNTRIES", data, TimeSpan.FromHours(1));
            }
            return data == null ? new List<MasterData>() : data;
        }

        private IList<MasterData> getCountries()
        {
            return new List<MasterData>
            {
                new MasterData { Name = "United States", Value = "US" },
                new MasterData { Name = "Canada", Value = "CA" },
                new MasterData { Name = "United Kingdom", Value = "UK" },
                new MasterData { Name = "Australia", Value = "AU" },
                new MasterData { Name = "Germany", Value = "DE" }
            }; ;
        }
    }
}
