using WebApiTestBook.Services.Interfaces;

namespace WebApiTestBook.Services
{
    public class MailinatorEmailService: IEmailService
    {
        public string sendEmail()
        {
            return "Email send my mailinator";
        }
    }
}
