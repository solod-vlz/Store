using System;
using System.Collections.Generic;
using System.Linq;

namespace Store
{
    public class Order
    {
        public int Id { get; }

        private List<OrderItem> items;

        public IReadOnlyCollection<OrderItem> Items
        {
            get => items;
        }
        
        public Order(int id, IEnumerable<OrderItem> items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            Id = id;
            this.items = new List<OrderItem>(items);
        }

        public int TotalCount
        {
            get => items.Sum(item => item.Count);
        }

        public decimal TotalPrice
        {
            get => items.Sum(item => item.Count * item.Price);
        }
    }
}
