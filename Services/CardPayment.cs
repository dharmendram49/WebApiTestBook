using WebApiTestBook.Services.Interfaces;

namespace WebApiTestBook.Services
{
    public class CardPayment: IPayment
    {
        public string PaymentType => "CARD";

        public string Pay()
        {
            return "Processing Card payment... Payment successful!";
        }
    }
}
