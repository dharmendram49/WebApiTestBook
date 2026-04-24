using WebApiTestBook.Services.Interfaces;

namespace WebApiTestBook.Services
{
    public class SmtpEmailService : IEmailService
    {
        public string sendEmail()
        {
            return "Email send my smtp";
        }
    }
}
