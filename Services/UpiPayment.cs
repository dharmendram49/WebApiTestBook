using WebApiTestBook.Services.Interfaces;

namespace WebApiTestBook.Services
{
    public class UpiPayment: IPayment
    {
        public string PaymentType => "UPI";

        public string Pay()
        {
            return "Processing UPI payment... Payment successful!";
        }
    }
}
