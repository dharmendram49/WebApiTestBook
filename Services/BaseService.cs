namespace WebApiTestBook.Services
{
    public abstract class BaseService
    {
        protected void Log(string message)
        {
            Console.WriteLine($"[LOG]: {message}");
        }

        protected void HandleError(Exception ex)
        {
            Console.WriteLine($"[ERROR]: {ex.Message}");
        }

        //public abstract void Save();
    }
}
