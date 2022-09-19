using System.Collections.Generic;
using System.Threading.Tasks;

namespace Store
{
    public interface IBookRepository
    {
        Book[] GetAllByIsbn(string isbn);

        Task<Book[]> GetAllByIsbnAsync(string isbn);

        Book[] GetAllByTitleOrAuthor(string titleOrAuthor);

        Task<Book[]> GetAllByTitleOrAuthorAsync(string titleOrAuthor);

        Book GetById(int id);

        Task<Book> GetByIdAsync(int id);

        Book[] GetAllByIds(IEnumerable<int> bookIds);

        Task<Book[]> GetAllByIdsAsync(IEnumerable<int> bookIds);
    }
}
