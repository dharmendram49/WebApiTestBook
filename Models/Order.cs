using System.ComponentModel.DataAnnotations.Schema;

namespace WebApiTestBook.Models
{
    public class Order
    {
        public int Id { get; set; }

        public int CustomerId { get; set; }

        [ForeignKey(nameof(CustomerId))]
        public required Customer Customer { get; set; }

        public decimal Total { get; set; }

        public DateTime Date { get; set; }
    }
}
