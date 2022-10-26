using Microsoft.AspNetCore.Mvc;
using Store.YandexKassa.Areas.YandexKassa.Models;

namespace Store.YandexKassa.Areas.YandexKassa.Controllers
{
    
    public class HomeController : Controller
    {
        [Area("YandexKassa")]
        public IActionResult Index(int orderId, string returnUri)
        {
            var model = new ExampleModel
            {
                OrderId = orderId,
                ReturnUri = returnUri,
            };

            return View(model);
        }
        [Area("YandexKassa")]
        public IActionResult Callback(int orderId, string returnUri)
        {
            var model = new ExampleModel
            {
                OrderId = orderId,
                ReturnUri = returnUri,
            };

            return View( model);
        }
    }
}