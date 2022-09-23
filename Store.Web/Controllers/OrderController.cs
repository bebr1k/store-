using Microsoft.AspNetCore.Mvc;
using Store.Contractors;
using Store.Web.Contractors;
using Store.Web.Models;
using System.Text.RegularExpressions;

namespace Store.Web.Controllers
{
    public class OrderController : Controller
    {

        private readonly IBookRepository bookRepository;
        private readonly IOrderRepository orderRepository;
        private readonly IEnumerable<IDeliveryService> deliveryServices;
        private readonly IEnumerable<IPaymentService> paymentServices;
        private readonly IEnumerable<IWebContractorService> webContractorServices;


        public OrderController(IBookRepository bookRepistory, 
                               IOrderRepository orderRepository, 
                               IEnumerable<IDeliveryService> deliveryServices, 
                               IEnumerable<IPaymentService> paymentServices,
                               IEnumerable<IWebContractorService> webContractorServices)
        {

            this.bookRepository = bookRepistory;
            this.orderRepository = orderRepository;
            this.deliveryServices = deliveryServices;
            this.paymentServices = paymentServices;
            this.webContractorServices = webContractorServices;

        }
        public IActionResult Index()
        {
            if (HttpContext.Session.TryGetCart(out Cart cart))
            {
                var order = orderRepository.GetById(cart.OrderId);
                var model = Map(order);
                return View(model);
            }
            else
            {
                return View("Empty");
            }
        }
        [HttpPost]
        public IActionResult AddItem(int bookId, int count = 1)
        {
            (Order order, Cart cart) = GetOrCreateOrderAndCart();
            var book = bookRepository.GetById(bookId);

            order.AddOrUpdateItem(book, count);

            SaveOrderAndCart(order, cart);

            return RedirectToAction("Index", "Book", new { id = bookId });
        }
        [HttpPost]
        public IActionResult UpdateItem(int bookId, int count)
        {
            (Order order, Cart cart) = GetOrCreateOrderAndCart();
            var book = bookRepository.GetById(bookId);
            order.GetItem(bookId).Count += count;
            SaveOrderAndCart(order, cart);

            return RedirectToAction("Index", "Order");
        }
        [HttpPost]
        public IActionResult RemoveItem(int bookId)
        {
            (Order order, Cart cart) = GetOrCreateOrderAndCart();

            order.RemoveItem(bookId);

            SaveOrderAndCart(order, cart);

            return RedirectToAction("Index", "Order");
        }

        private (Order order, Cart cart) GetOrCreateOrderAndCart()
        {
            Order order;
            if (HttpContext.Session.TryGetCart(out Cart cart))
            {
                order = orderRepository.GetById(cart.OrderId);
            }
            else
            {
                order = orderRepository.Create();
                cart = new Cart(order.Id);
            }
            return (order, cart);
        }
        private void SaveOrderAndCart(Order order, Cart cart)
        {
            orderRepository.Update(order);
            cart.TotalCount = order.TotalCount;
            cart.TotalPrice = order.TotalPrice;
            HttpContext.Session.Set(cart);
        }

        private OrderModel Map(Order order)
        {
            var booksIds = order.Items.Select(item => item.BookId);
            var books = bookRepository.GetAllByIds(booksIds);
            var itemModels = from item in order.Items
                             join book in books on item.BookId equals book.Id
                             select new OrderItemModel
                             {
                                 BookId = book.Id,
                                 Title = book.Title,
                                 Author = book.Author,
                                 Price = item.Price,
                                 Count = item.Count,
                             };
            return new OrderModel
            {
                Id = order.Id,
                Items = itemModels.ToArray(),
                TotalCount = order.TotalCount,
                TotalPrice = order.TotalPrice
            };
        }

        public IActionResult SendConfirmationCode(int id, string cellPhone)
        {
            var order = orderRepository.GetById(id);
            var model = Map(order);
            if (!IsValidCellPhone(cellPhone))
            {
                return View("Index", model);
            }

            int code = 1111; //Random.Next(0,10000);
            HttpContext.Session.SetInt32(cellPhone, code);

            return View("Confirmation",
                new ConfirmationModel
                {
                    OrderId = id,
                    CellPhone = cellPhone,
                });

        }

        static private bool IsValidCellPhone(string cellPhone)
        {
            if (cellPhone == null)
                return false;

            cellPhone = cellPhone.Replace(" ", "").
                                  Replace(" ", "").
                                  Replace("+", "").
                                  Replace("(", "").
                                  Replace(")", "").
                                  Replace("-", "");

            return Regex.IsMatch(cellPhone, @"^\+?\d{11}$");
        }

        [HttpPost]
        public IActionResult Confirmate(int id, string cellPhone, int code)
        {
            int? storedCode = HttpContext.Session.GetInt32(cellPhone);
            if (storedCode == null)
            {
                return View("Confirmation",
               new ConfirmationModel
               {
                   OrderId = id,
                   CellPhone = cellPhone,
                   Errors = new Dictionary<string, string>
                   {
                       {
                           "code", "Сессия закончилась, повторите отправку кода подтверждения."
                       },
                   }
               }); ;
            }
            if (code == 0)
                return View("Confirmation",
               new ConfirmationModel
               {
                   OrderId = id,
                   CellPhone = cellPhone,
                   Errors = new Dictionary<string, string>
                   {
                       {
                           "code", "Введите код подтверждения."
                       },
                   }
               }); ;

            if (code == storedCode)
            {

                var order = orderRepository.GetById(id);
                order.CellPhone = cellPhone;
                orderRepository.Update(order);

                HttpContext.Session.Remove(cellPhone);
                var model = new DeliveryModel()
                {
                    OrderId = id,
                    Methods = deliveryServices.ToDictionary(service => service.UniqueCode, service => service.Title)
                };
                return View("DeliveryMethod", model);

            }
            else
                return View("Confirmation", new ConfirmationModel
                {
                    OrderId = id,
                    CellPhone = cellPhone,
                    Errors = new Dictionary<string, string>
                    {
                        {
                            "code", "Введен неверный код."
                        },
                    }
                });
        }
        [HttpPost]
        public IActionResult StartDelivery(int id, string uniqueCode)
        {
            var deliveryService = deliveryServices.Single(service => service.UniqueCode == uniqueCode);
            var order = orderRepository.GetById(id);

            var form = deliveryService.CreateForm(order);
            return View("DeliveryStep", form);
        }
        [HttpPost]
        public IActionResult NextDelivery(int id, string uniqueCode, int step, Dictionary<string, string> values)
        {
            var deliveryService = deliveryServices.Single(service => service.UniqueCode == uniqueCode);
            var form = deliveryService.MoveNext(id, step, values);
            if (form.IsFinal)
            {
                var order = orderRepository.GetById(id);
                order.Delivery = deliveryService.GetDelivery(form);
                orderRepository.Update(order);

                var model = new DeliveryModel()
                {
                    OrderId = id,
                    Methods = paymentServices.ToDictionary(service => service.UniqueCode, 
                                                           service => service.Title)
                };
                return View("PaymentMethod", model);
            }

            return View("DeliveryStep", form);
        }
        [HttpPost]
        public IActionResult StartPayment(int id, string uniqueCode)
        {
            var paymentService = paymentServices.Single(service => service.UniqueCode == uniqueCode);
            var order = orderRepository.GetById(id);

            var form = paymentService.CreateForm(order);

            var webContractorService = webContractorServices.SingleOrDefault(service => service.UniqueCode == uniqueCode);
            if (webContractorService != null)
                return Redirect(webContractorService.GetUri);

            return View("PaymentStep", form);
        }
        [HttpPost]
        public IActionResult NextPayment(int id, string uniqueCode, int step, Dictionary<string, string> values)
        {
            var paymentService = paymentServices.Single(service => service.UniqueCode == uniqueCode);
            var form = paymentService.MoveNext(id, step, values);
            if (form.IsFinal)
            {
                var order = orderRepository.GetById(id);
                order.Payment = paymentService.GetPayment(form);
                orderRepository.Update(order);
               
                return View("Finish");
            }

            return View("DeliveryStep", form);
        }

        public IActionResult Finish()
        {

            HttpContext.Session.RemoveCart(); 
            return View();
        }
    }
}








