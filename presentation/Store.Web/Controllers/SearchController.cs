using Microsoft.AspNetCore.Mvc;
using Store.Web.App;

namespace Store.Web.Controllers
{
    public class SearchController : Controller
    {
        private readonly BookService bookService;

        public SearchController(BookService bookService)
        {
            this.bookService = bookService;
        }
        
        public IActionResult Index(string query)
        {
            var books = query == null ? new BookModel[0]:bookService.GetAllByQuery(query);

            return View(books);
        }
    }
}
