using WebApiTestBook.Models;

namespace WebApiTestBook.Dto
{
    public class CustomerDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public List<OrderDto>? Orders { get; set; }
    }
}
