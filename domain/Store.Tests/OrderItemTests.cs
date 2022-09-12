using Store.Data;
using System;
using Xunit;

namespace Store.Tests
{
    public class OrderItemTests
    {
        void OrderItem_TestUncorrectCountValue(int value)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var orderItemDto = OrderItem.DtoFactory.Create(new OrderDto(), 1, 10m, value);
                var orderItem = OrderItem.Mapper.Map(orderItemDto);
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
            var orderItemDto = OrderItem.DtoFactory.Create(new OrderDto(), 1, 3m, 2);
            var orderItem = OrderItem.Mapper.Map(orderItemDto);

            Assert.Equal(1, orderItem.BookId);
            Assert.Equal(2, orderItem.Count);
            Assert.Equal(3m, orderItem.Price);
        }

        [Fact]
        public void SetCount_WithNegativeValue_ThrowArgumentOutOfRangeException()
        {
            var orderItemDto = OrderItem.DtoFactory.Create(new OrderDto(), 1, 0m, 1);
            var orderItem = OrderItem.Mapper.Map(orderItemDto);

            Assert.Throws<ArgumentOutOfRangeException>(() => orderItem.Count = -1);
        }

        [Fact]
        public void SetCount_WithZeroValue_ThrowArgumentOutOfRangeException()
        {
            var orderItemDto = OrderItem.DtoFactory.Create(new OrderDto(), 1, 0m, 1);
            var orderItem = OrderItem.Mapper.Map(orderItemDto);

            Assert.Throws<ArgumentOutOfRangeException>(() => orderItem.Count = 0);
        }

        [Fact]
        public void SetCount_WithPositiveValue_SetsValue()
        {
            var orderItemDto = OrderItem.DtoFactory.Create(new OrderDto(), 1, 0m, 1);
            var orderItem = OrderItem.Mapper.Map(orderItemDto);

            orderItem.Count = 3;

            Assert.Equal(3, orderItem.Count);

        }
    }
}
