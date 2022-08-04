using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Store.Tests
{
    public class OrderTests
    {
        [Fact]
        public void Order_WithNullItems_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new Order(1, null));
        }

        [Fact]
        public void TotalCount_WithEmptyItems_ReturnZero()
        {
            var order = new Order(1, new List<OrderItem>() { });

            Assert.Equal(0, order.TotalCount);
        }

        [Fact]
        public void TotalPrice_WithEmptyItems_ReturnZero()
        {
            var order = new Order(1, new List<OrderItem>() { });

            Assert.Equal(0m, order.TotalPrice);
        }

        [Fact]
        public void TotalCount_WithNoEmptyItems_CalculatesTotalCount()
        {
            var order = new Order(1, new List<OrderItem>() 
            { 
                new OrderItem(1, 3, 10m),
                new OrderItem(2, 5, 100m)
            });

            Assert.Equal(3+5, order.TotalCount);
        }

        [Fact]
        public void TotalPrice_WithNoEmptyItems_CalculatesTotalPrice()
        {
            var order = new Order(1, new List<OrderItem>()
            {
                new OrderItem(1, 3, 10m),
                new OrderItem(2, 5, 100m)
            });

            Assert.Equal(3*10m + 5*100m, order.TotalPrice);
        }
    }
}
