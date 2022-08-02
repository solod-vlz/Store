using System;
using System.Text.RegularExpressions;

namespace Store
{
    public class Book
    {
        public int Id { get; }

        public int Isbn { get; }

        public string Author { get; }

        public string Title { get; }

        public Book(int id, string isbn, string author, string title)
        {
            Id = id;
            Title = title;
            Author = author;
            Title = title;
        }

        internal static bool IsIsbn (string s)
        {
            if (s == null)
                return false;

            var correctStr = s.Replace("-", "")
                              .Replace(" ", "")
                              .ToUpper();

            var pattern = "^ISBN\\d{10}(\\d{3})?$";

            return Regex.IsMatch(correctStr, pattern);

        }
    }
}
