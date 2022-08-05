using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Store.Web.Models
{
    public class Cart
    {
        public int OrderId { get; }

        public int TotalCount { get; set; }

        public decimal TotalPrice { get; set; }

        public Cart(int orderId)
        {
            OrderId = orderId;
            TotalPrice = 0m;
            TotalCount = 0;
        }
    }
}
