using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApiTestBook.Data;
using WebApiTestBook.Dto;

namespace WebApiTestBook.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EFController : ControllerBase
    {
        private readonly ApiDbContext dbContext;

        public EFController(
            ApiDbContext dbContext
            )
        {
            this.dbContext = dbContext;
        }

        [HttpGet("GetAll")]
        public IActionResult GetAllCustomers()
        {
            var customers = dbContext.Customers.ToList();
            return Ok(customers);
        }

        [HttpGet("GetAllBySelcet")]
        public IActionResult GetAllBySelcet()
        {
            var customers = dbContext.Customers
                .Select(x => new { x.Id, x.FirstName, x.LastName })
                .ToList();

            var customersDto = dbContext.Customers
                .Select(x => new CustomerDto
                {
                    Id = x.Id,
                    FirstName = x.FirstName,
                    LastName = x.LastName
                })
                .ToList();

            return Ok(customers);
        }

        [HttpGet("Join")]
        public IActionResult Join()
        {
            var customers = dbContext.Customers
                 .Select(x => new CustomerDto
                 {
                     Id = x.Id,
                     FirstName = x.FirstName,
                     LastName = x.LastName,
                     Orders = x.Orders != null ? x.Orders //EF automatically handles JOIN internally ✅
                     .Select(o => new OrderDto
                     {
                         Id = o.Id,
                         Total = o.Total,
                         Date = o.Date
                     }).ToList() : null
                 }).ToList();

            return Ok(customers);
        }

        [HttpGet("Join2")]
        public IActionResult Join2()
        {
            var customers = dbContext.Customers
                 .Join(
                     dbContext.Orders,
                     c => c.Id,
                     o => o.CustomerId,
                     (c, o) => new
                     {
                         CustomerFirstName = c.FirstName,
                         CustomerLastName = c.LastName,
                         OrderTotalAmount = o.Total,
                         OrderDate = o.Date
                     }
                 ).ToList();
            return Ok(customers);
        }

        [HttpGet("Join3")]
        public IActionResult Join3()
        {
            var customers = (from c in dbContext.Customers
                             join o in dbContext.Orders
                             on c.Id equals o.CustomerId
                             select new
                             {
                                 c.Id,
                                 CustomerFirstName = c.FirstName,
                                 CustomerLastName = c.LastName,
                                 OrderTotalAmount = o.Total,
                                 OrderDate = o.Date
                             }).ToList();

            return Ok(customers);
        }

        [HttpGet("leftJoin")]
        public IActionResult LeftJoin()
        {
            var customers = (from c in dbContext.Customers
                             join o in dbContext.Orders
                             on c.Id equals o.CustomerId into co
                             from o in co.DefaultIfEmpty()
                             select new
                             {
                                 customerFirstName = c.FirstName,
                                 customerLastName = c.LastName,
                                 orderTotalAmount = o != null ? o.Total : 0,
                             }
                            ).ToList();

            return Ok(customers);
        }

        [HttpGet("leftJoin2")]
        public IActionResult LeftJoin2()
        {
            var customers = dbContext.Customers
                .GroupJoin(
                     dbContext.Orders,
                     c => c.Id,
                     o => o.CustomerId,
                     (c, orders) => new { c, orders }
                 ).SelectMany(
                    x => x.orders.DefaultIfEmpty(),
                    (a, b) => new
                    {
                        customerFirstName = a.c.FirstName,
                        customerLastName = a.c.LastName,
                        orderTotalAmount = b != null ? b.Total : 0,
                    }
                ).ToList();

            return Ok(customers);
        }

        [HttpGet("groupBy")]
        public IActionResult GroupBy()
        {
            var orders = dbContext.Orders
                .GroupBy(x => x.CustomerId)
                .Select(g=>new
                {
                   CustomerId =  g.Key,
                   TotalAmount = g.Sum(x=>x.Total),
                   OrderCount = g.Count()
                })
                .ToList();

            return Ok(orders);
        }

        [HttpGet("groupByWithJoin")]
        public IActionResult GroupByWithJoin()
        {
            var orders = dbContext.Orders
                .GroupBy(x => x.Customer)
                .Select(g => new
                {
                    CustomerName = g.Key.FirstName + g.Key.LastName,
                    TotalAmount = g.Sum(x => x.Total)
                }).ToList();

            return Ok(orders);
        }

        [HttpGet("leftJoinWithGroupBy")]
        public IActionResult LeftJoinWithGroupBy()
        {
            var orders = dbContext.Customers
                .GroupJoin(
                    dbContext.Orders,
                    c => c.Id,
                    o => o.CustomerId,
                    (c, o) => new
                    {
                        CustomerName = c.FirstName + c.LastName,
                        TotalAmount = o.Sum(x => x.Total),
                        OrderCount = o.Count()
                    }
                ).ToList();

            return Ok(orders);
        }

        [HttpGet("groupByByNavigationProperty")]
        public IActionResult GroupByByNavigationProperty()
        {
            var result = dbContext.Customers
                    .Select(c => new
                    {
                        CustomerName = c.FirstName + " " + c.LastName,
                        TotalAmount = c.Orders.Sum(o => (decimal?)o.Total) ?? 0,
                        OrderCount = c.Orders.Count()
                    })
                    .ToList();

            dbContext.Customers
                .Select(x => new
                {
                    CustomerId = x.Id,
                    Name = x.FirstName + x.LastName,
                    OrderCount = x.Orders.Count()
                })
                .OrderByDescending(x => x.OrderCount)
                .FirstOrDefault();

            dbContext.Customers
                .Select(x => new
                {
                    x.Id,
                    Name = x.FirstName + x.LastName,
                    MaxOrder = x.Orders.Max(x => x.Total)
                }).ToList();

            dbContext.Customers
                 .Select(x => !x.Orders.Any()).ToList();


            return Ok(result);
        }
    }
}
