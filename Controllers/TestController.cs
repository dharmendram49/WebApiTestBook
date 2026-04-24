using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.Collections.Concurrent;
using System.Diagnostics;
using WebApiTestBook.Filters;
using WebApiTestBook.Services;
using WebApiTestBook.Services.Interfaces;

namespace WebApiTestBook.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly IEmailService emailServices;
        private readonly IEmailService emailService1;
        private readonly ICacheService cacheService1;
        private readonly ICacheService cacheService2;
        private readonly IMasterService masterService;
        private readonly IEnumerable<IPayment> payments;

        public IEmailService EmailService { get; set; }

        public TestController(IEmailService emailService ,
            IEmailService emailService1,
            ICacheService cacheService1,
            ICacheService cacheService2,
            IMasterService masterService,
            IEnumerable<IPayment> payments
            )
        {
            this.emailServices = emailService;
            this.emailService1 = emailService1;
            this.cacheService1 = cacheService1;
            this.cacheService2 = cacheService2;
            this.masterService = masterService;
            this.payments = payments;
        }


        [HttpGet("TestWithoutMultiThreading")]
        public async Task<ActionResult> TestWithoutMultiThreading()
        {
            var timer =new  Stopwatch();
            timer.Start();
            var users = await GetUser();
            var orders = await GetOrders();
            var notifications = await GetNotifications();

            timer.Stop();

            var timeTaken = timer.ElapsedMilliseconds;

            return Ok(new { users, orders, notifications, timeTaken });
        }

        [HttpGet("TestWithMultiThreading")]
        public async Task<ActionResult> TestWithMultiThreading()
        {
            var timer = new Stopwatch();
            timer.Start();

            var usersTask = GetUser();
            var ordersTask = GetOrders();
            var notificationsTask = GetNotifications();

            await Task.WhenAll(usersTask, ordersTask, notificationsTask);

            timer.Stop();

            var timeTaken = timer.ElapsedMilliseconds;

            return Ok(new { 
                user = usersTask.Result, 
                orders =  ordersTask.Result,
                notifications = notificationsTask.Result,
                timeTaken
            });
        }

        [HttpGet("TestMuitpPaymentMethods")]
        public ActionResult TestMuitpPaymentMethods(string type)
        {
            var payment =  payments.FirstOrDefault(P => P.PaymentType == type);
            if (payment == null)
            {
                return BadRequest();
            }

            var result = payment.Pay();
            return Ok(result);
        }

        [HttpGet("TestDictionary")]
        public ActionResult TestDictionary()
        {
            var dict = new Dictionary<int, string>();
            dict[5] = "A";
            dict[5] = "B";
            if(dict.TryGetValue(5,out var value))
            {
                //return Ok(value);
            }

            var value2 = dict.GetValueOrDefault(5, "no");
            return Ok(value2);
        }

        [HttpGet("TestMemoryCache")]
        public ActionResult TestMemoryCache()
        {
           
            var data = masterService.GetCountries();
            return Ok(data);
        }

        [HttpGet("GeneratePassword")]
        public ActionResult GeneratePassword()
        {
            var hasher = new PasswordHasher<IdentityUser>();
            var hash = hasher.HashPassword(null, "123456");

            return Ok(hash);
        }

        [HttpGet("TesMVCtExceptionFilter")]
        [ServiceFilter(typeof(MVCExceptionFilter))]
        public ActionResult TesMVCtExceptionFilter()
        {
            throw new Exception("This is a test exception");
        }

        [ServiceFilter(typeof(ResponseWrapperFilter))]
        [HttpGet("TestResponseWrapperFilter")]
        public ActionResult TestResponseWrapperFilter()
        {
            return Ok("TestResponseWrapperFilter");
        }

        [ServiceFilter(typeof(ExecutionTimeFilter))]
        [HttpGet("TestExecutionTimeFilter")]
        public async Task<ActionResult> TestExecutionTimeFilter()
        {
            await Task.Delay(5000); // wait for 5 seconds (non-blocking)
            return Ok("TestExecutionTimeFilter");
        }

        [ServiceFilter(typeof(CacheResourceFilter))]
        [HttpGet("TestCaching")]
        public ActionResult TestCaching()
        {
            return Ok("TestCaching");
        }

        [HttpGet("TestSingleton")]
        public ActionResult TestSingleton()
        {
            return Ok(cacheService1.GetHashCode() + " - " + cacheService2.GetHashCode());
        }

        [HttpGet("TestSingleton2")]
        public ActionResult TestSingleton2()
        {
            return Ok(cacheService1.GetHashCode() + " - " + cacheService2.GetHashCode());
        }

        [HttpGet("TestTransiant")]
        public ActionResult TestTransiant()
        {
            return Ok(emailServices.GetHashCode()+" - "+ emailServices.GetHashCode());
        }

        //public TestController(IEnumerable<IEmailService> emailServices)
        //{
        //    this.emailServices = emailServices;
        //}


        [HttpGet("TestEmail")]
        public ActionResult TestEmail()
        {
            var results = emailServices.sendEmail();
            return Ok(results);
        }

        //[HttpGet("TestEmail")]
        //public ActionResult TestEmail()
        //{
        //    var results = emailServices.Select(s => s.sendEmail());
        //    return Ok(results);
        //}

        [HttpGet("TestMethodDI")]
        public ActionResult TestMethodDI([FromServices] IEmailService emailService)
        {
            return Ok(emailService.sendEmail());
        }

        [HttpGet("TestPropertyDI")]
        public ActionResult TestPropertyDI()
        {
            return Ok(EmailService.sendEmail());
        }


        private async Task<string> GetUser()
        {
            await Task.Delay(2000); //simlate db calls
            return "User data";
        }

        private async Task<string> GetOrders()
        {
            await Task.Delay(3000);
            return "Orders data";
        }

        private async Task<string> GetNotifications()
        {
            await Task.Delay(4000);
            return "Notification data";
        }
    }
}