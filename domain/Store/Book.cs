using Store.Data;
using System;
using System.Text.RegularExpressions;

namespace Store
{
    public class Book
    {
        private readonly BookDto dto;

        public int Id => dto.Id;

        public string Isbn 
        {
            get => dto.Isbn;
            set
            {
                if (TryFormatIsbn(value, out string formatedIsbn))
                    dto.Isbn = formatedIsbn;
                else
                    throw new ArgumentException(nameof(Isbn));
            }
        }

        public string Author
        {
            get => dto.Author;
            set => dto.Author = value?.Trim();
        }

        public string Title 
        { 
            get => dto.Title;

            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException(nameof(Title));  //TODO write tests

                dto.Title = value;
            }
        }

        public string Description
        {
            get => dto.Description;
            set => dto.Description = value;
        }

        public decimal Price
        {
            get => dto.Price;
            set => dto.Price = value;
        }

        internal Book(BookDto dto)
        {
            this.dto = dto;
        }

        public static bool TryFormatIsbn(string isbn, out string formatedIsbn)
        {
            if(isbn == null)
            {
                formatedIsbn = null;
                return false;
            }

            formatedIsbn = isbn.Replace("-", "")
                              .Replace(" ", "")
                              .ToUpper();

            var pattern = "^ISBN\\d{10}(\\d{3})?$";

            return Regex.IsMatch(formatedIsbn, pattern);
        }

        public static bool IsIsbn(string isbn) => TryFormatIsbn(isbn, out _);

        public static class Mapper
        {
            public static Book Map(BookDto dto) => new Book(dto);

            public static BookDto Map(Book domain) => domain.dto;
        }

        public static class DtoFactory
        {
            public static BookDto Create(string isbn, string author, string title, string description, decimal price)
            {
                if (TryFormatIsbn(isbn, out string formatedIsbn))
                    isbn = formatedIsbn;
                else throw new ArgumentException(nameof(isbn));

                if (string.IsNullOrWhiteSpace(title))
                    throw new ArgumentException(nameof(title));

                return new BookDto
                {
                    Isbn = isbn,
                    Author = author?.Trim(),
                    Title = title.Trim(),
                    Description = description?.Trim(),
                    Price = price,
                };
            }
        }
    }
}
