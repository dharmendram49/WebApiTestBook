using System.ComponentModel.DataAnnotations.Schema;
using WebApiTestBook.Models;

namespace WebApiTestBook.Dto
{
    public class OrderDto
    {
        public int Id { get; set; }

        public decimal Total { get; set; }

        public DateTime Date { get; set; }
    }
}
