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
                new OrderItem(0, 0m, value);
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
            var orderItem = new OrderItem(1, 3m, 2);

            Assert.Equal(1, orderItem.BookId);
            Assert.Equal(2, orderItem.Count);
            Assert.Equal(3m, orderItem.Price);
        }

        [Fact]
        public void SetCount_WithNegativeValue_ThrowArgumentOutOfRangeException()
        {
            var orderItem = new OrderItem(1, 0m, 1);

            Assert.Throws<ArgumentOutOfRangeException>(() => orderItem.Count = -1);
        }

        [Fact]
        public void SetCount_WithZeroValue_ThrowArgumentOutOfRangeException()
        {
            var orderItem = new OrderItem(1, 0m, 1);

            Assert.Throws<ArgumentOutOfRangeException>(() => orderItem.Count = 0);
        }

        [Fact]
        public void SetCount_WithPositiveValue_SetsValue()
        {
            var orderItem = new OrderItem(1, 0m, 1);

            var actual = 3;

            orderItem.Count = actual;

            Assert.Equal(actual, orderItem.Count);

        }
    }
}
