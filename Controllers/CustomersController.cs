using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiTestBook.Data;
using WebApiTestBook.Models;

namespace WebApiTestBook.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly ApiDbContext _db;

        public CustomersController(ApiDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<ActionResult<List<Customer>>> GetCustomers()
        {
            var customers = await _db.Customers
                .AsNoTracking()
                .OrderBy(c => c.Id)
                .ToListAsync();

            return Ok(customers);
        }
    }
}
