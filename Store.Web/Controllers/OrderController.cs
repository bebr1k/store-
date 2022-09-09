using Microsoft.AspNetCore.Mvc;
using Store.Web.Models;

namespace Store.Web.Controllers
{
    public class OrderController : Controller
    {

        private readonly IBookRepository bookRepository;
        private readonly IOrderRepository orderRepository;
        public OrderController(IBookRepository bookRepistory, IOrderRepository orderRepository)
        {
            this.bookRepository = bookRepistory;
            this.orderRepository = orderRepository;
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

        public IActionResult AddItem(int bookId, int count = 1) 
        {
            (Order order, Cart cart) = GetOrCreateOrderAndCart();
            var book = bookRepository.GetById(bookId);

            order.AddOrUpdateItem(book, count);
            
            SaveOrderAndCart(order,cart);

            return RedirectToAction("Index", "Book", new {id = bookId });
        }
       
        public IActionResult UpdateItem(int bookId, int count)
        {
            (Order order, Cart cart) = GetOrCreateOrderAndCart();
            var book = bookRepository.GetById(bookId);
            order.GetItem(bookId).Count += count;            
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
        
        
    }
            
}

        


