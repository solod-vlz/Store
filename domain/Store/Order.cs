using System;
using System.Collections.Generic;
using System.Linq;

namespace Store
{
    public class Order
    {
        public int Id { get; }

        private readonly List<OrderItem> items;

        public IReadOnlyCollection<OrderItem> Items => items;

        public int TotalCount => items.Sum(item => item.Count);

        public decimal TotalPrice => items.Sum(item => item.Count * item.Price);

        public Order(int id, IEnumerable<OrderItem> items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            Id = id;

            this.items = new List<OrderItem>(items);
        }

        public void AddOrUpdateItem (Book book, int count)
        {
            if (book == null)
                throw new ArgumentNullException(nameof(book));

            var index = items.FindIndex(item => item.BookId == book.Id);

            if (index == -1)
                items.Add(new OrderItem(book.Id, count, book.Price));

            else
                items[index].Count += count;
        }

        public OrderItem GetItem (int bookId)
        {
            int index = items.FindIndex(item => item.BookId == bookId);

            if (index == -1)
                ThrowBookException("Book not found", bookId);

            return items[index];
        }

        public void RemoveItem(int bookId)
        {
            var index = items.FindIndex(item => item.BookId == bookId);

            if (index == -1)
                ThrowBookException("Order dosn`t contain specified item.", bookId);

            items.RemoveAt(index);
        }

        private void ThrowBookException (string message, int bookId)
        {
            var exception = new InvalidOperationException(message);

            exception.Data["Id"] = bookId;

            throw exception;
        }
    }
}
