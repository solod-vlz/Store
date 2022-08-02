using System;
using System.Linq;

namespace Store.Memory
{
    public class BookRepository : IBookRepository
    {
        private readonly Book[] books = new[]
        {
            new Book(1, "ISBN 12312-31332", "D. Knuth", "Art of Programming"),
            new Book(2, "ISBN 12312-66666", "M. Fowler", "Refactoring"),
            new Book(3, "ISBN 12312-55555", "B. Kernighan", "C Programming Language"),
        };

        public Book[] GetAllByIsbn(string isbn)
        {
            throw new NotImplementedException();
        }

        public Book[] GetAllByTitleOrAuthor(string titlePart)
        {
            return books.Where(book => book.Title.Contains(titlePart))
                .ToArray();
        }
    }
}
  