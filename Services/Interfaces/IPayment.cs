namespace WebApiTestBook.Services.Interfaces
{
    public interface IPayment
    {
        string PaymentType { get; }
        string Pay();
    }
}
