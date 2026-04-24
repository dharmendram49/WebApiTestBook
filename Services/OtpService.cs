using Microsoft.Extensions.Caching.Distributed;
using WebApiTestBook.Services.Interfaces;

namespace WebApiTestBook.Services
{
    public class OtpService: IOtpService
    {
        private const string CacheKeyPrefix = "otp_";

        private readonly IDistributedCache distributedCache;

        public OtpService(IDistributedCache distributedCache)
        {
            this.distributedCache = distributedCache;
        }

        public async Task<string> GenerateOtp(string mobile)
        {
            var otp = new Random().Next(100000, 999999).ToString();
            var key = $"otp_{mobile}";

            await distributedCache.SetStringAsync(key, otp, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            });
            return otp;
        }

        public async Task<bool> VerifyOtp(string mobile, string inputOtp)
        {
            var key = $"otp_{mobile}";
            var storedOtp =  await distributedCache.GetStringAsync(key);

            if (storedOtp == null)
                return false;

            if (storedOtp == inputOtp)
            {
                await distributedCache.RemoveAsync(key); // 🔥 remove after success
                return true;
            }

            return false;   
        }
    }
}
