namespace WebApiTestBook.Services.Interfaces
{
    public interface IOtpService
    {
        Task<string> GenerateOtp(string mobile);

        Task<bool> VerifyOtp(string mobile, string inputOtp);
    }
}
