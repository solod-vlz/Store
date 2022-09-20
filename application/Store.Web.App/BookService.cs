using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Store.Web.App
{
    public class BookService
    {
        private readonly IBookRepository bookRepository;

        public BookService(IBookRepository bookRepository)
        {
            this.bookRepository = bookRepository;
        }

        public async Task<IReadOnlyCollection<BookModel>> GetAllByQueryAsync(string query)
        {
            var books = Book.IsIsbn(query)
                        ? await bookRepository.GetAllByIsbnAsync(query)
                        : await bookRepository.GetAllByTitleOrAuthorAsync(query);

            return books.Select(Map)
                        .ToArray();
        }

        public async Task<BookModel> GetByIdAsync(int id)
        {
            return Map(await bookRepository.GetByIdAsync(id));
        }

        private BookModel Map(Book book)
        {
            return new BookModel
            {
                Id = book.Id,
                Isbn = book.Isbn,
                Title = book.Title,
                Author = book.Author,
                Description = book.Description,
                Price = book.Price,
            };
        }
    }
}
