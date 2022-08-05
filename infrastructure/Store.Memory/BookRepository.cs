using System;
using System.Collections.Generic;
using System.Linq;

namespace Store.Memory
{
    public class BookRepository : IBookRepository
    {
        private readonly Book[] books = new[]
        {
            new Book(1, "ISBN 12312-31332", "D. Knuth", "Art of Programming, Vol.1", 
                "The bible of all fundamental algorithms and the work that taught many " +
                "of today's software developers most of what they know about computer programming",
                69.68m),
            new Book(2, "ISBN 12312-66666", "M. Fowler", "Refactoring: Improving the Design of Existing Code (2nd Edition)",
                "A guide to refactoring, the process of changing a software system so that it does not alter" +
                "the external behavior of the code yet improves its internal structure, for professional" +
                "programmers.Early chapters cover general principles, rationales, examples, and testing." +
                "The heart of the book is a catalog of refactorings, organized in chapters on composing" +
                "methods, moving features between objects, organizing data, simplifying conditional" +
                "expressions, and dealing with generalizations",
                40.01m),
            new Book(3, "ISBN 12312-55555", "B. Kernighan", "C Programming Language",
                "C is a general-purpose programming language which features economy of" +
                "expression, modern control flow and data structures, and a rich set of operators." +
                "C is not a \"very high level\" language, nor a \"big\" one, and is not specialized to" +
                "any particular area of application. But its absence of restrictions and its generality " +
                "make it more convenient and effective for many tasks than supposedly more powerful languages",
                25.34m)
        };

        public Book[] GetAllByIds(IEnumerable<int> bookIds)
        {
            return books.Where(book => bookIds.Contains(book.Id))
                        .ToArray();
        }

        public Book[] GetAllByIsbn(string isbn)
        {
            return books.Where(book => book.Isbn == isbn)
                        .ToArray();
        }

        public Book[] GetAllByTitleOrAuthor(string query)
        {
            return books.Where(book => book.Title.Contains(query)
                                       || book.Author.Contains(query))
                        .ToArray();
        }

        public Book GetById(int id)
        {
            return books.Single(book => book.Id == id);
        }
    }
}
  