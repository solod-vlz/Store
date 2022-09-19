using Microsoft.AspNetCore.Mvc;
using Store.Web.App;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Store.Web.Controllers
{
    public class SearchController : Controller
    {
        private readonly BookService bookService;

        public SearchController(BookService bookService)
        {
            this.bookService = bookService;
        }
        
        //public IActionResult Index(string query)
        //{
        //    var books = query == null ? new BookModel[0]:bookService.GetAllByQuery(query);

        //    return View(books);
        //}

        public async Task<IActionResult> Index(string query)
        {
            IReadOnlyCollection<BookModel> books;
            
            if (query == null)
                books = new BookModel[0];

            else
                books = await bookService.GetAllByQueryAsync(query);

            return View(books);
        }
    }
}
