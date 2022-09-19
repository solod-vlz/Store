using Microsoft.AspNetCore.Mvc;
using Store.Web.App;
using System.Threading.Tasks;

namespace Store.Web.Controllers
{
    public class BookController : Controller
    {
        private readonly BookService bookService;

        public BookController(BookService bookService)
        {
            this.bookService = bookService;
        }

        //public IActionResult Index(int id)
        //{
        //    var model = bookService.GetById(id);

        //    return View(model);
        //}

        public async Task<IActionResult> Index(int id)
        {
            var model = await bookService.GetByIdAsync(id);

            return View(model);
        }
    }
}
