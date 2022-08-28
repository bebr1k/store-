using Microsoft.AspNetCore.Mvc;
using Store.Web.Models;

namespace Store.Web.Controllers
{
    public class OrderController : Controller
    {
        private readonly IBookRepository bookRepository;
        private readonly IOrderRepository orderRepository;
        public OrderController(IBookRepository bookRepistory,IOrderRepository orderRepository)
        {
            this.bookRepository = bookRepistory;
            this.orderRepository = orderRepository;
        }
        public IActionResult AddItem (int id)
        {           

            Order order;
            Cart cart;
            if(HttpContext.Session.TryGetCart(out cart))
            {
                order = orderRepository.GetById(cart.OrderId);
            }
            else
            {
                order = orderRepository.Create();
                cart = new Cart(order.Id);
            }
            var book = bookRepository.GetById(id);
            order.AddItem(book, 1);
            orderRepository.Update(order);

            cart.TotalCount = order.TotalCount;
            cart.TotalPrice = order.TotalPrice;
           
            HttpContext.Session.Set(cart);
            
            return RedirectToAction("Index", "Book", new { id }); 
        }
    }
}
