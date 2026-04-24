using WebApiTestBook.Models;

namespace WebApiTestBook.Services.Interfaces
{
    public interface IMasterService
    {
       public IList<MasterData> GetCountries();
    }
}
