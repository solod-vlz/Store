using System;
using Xunit;

namespace Store.Tests
{
    public class OrderItemTests
    {
        
        public void OrderItem_TestUncorrectCountValue(int value)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                new OrderItem(0, value, 0m);
            });
        }
        
        [Fact]
        public void OrderItem_WithZeroCount_ThrowArgumentOutOfRangeException()
        {
            OrderItem_TestUncorrectCountValue(0);
        }

        [Fact]
        public void OrderItem_WithNegativeCount_ThrowArgumentOutOfRangeException()
        {
            OrderItem_TestUncorrectCountValue(-1);
        }

        [Fact]
        public void OrderItem_WithPositiveCount_SetsCount()
        {
            var orderItem = new OrderItem(1, 2, 3m);

            Assert.Equal(1, orderItem.BookId);
            Assert.Equal(2, orderItem.Count);
            Assert.Equal(3m, orderItem.Price);
        }
    }
}
