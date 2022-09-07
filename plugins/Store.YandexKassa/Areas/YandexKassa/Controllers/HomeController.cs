using Microsoft.AspNetCore.Mvc;

namespace Store.YandexKassa.Areas.Controllers
{
    [Area("YandexKassa")]
    public class HomeController: Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Callback()
        {
            return View();
        }
    }


}
