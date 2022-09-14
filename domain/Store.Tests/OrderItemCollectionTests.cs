using Store.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Store.Tests
{
    public class OrderItemCollectionTests
    {
        public static Order CreateTestOrder()
        {
            return new Order(new OrderDto
            {
                Id = 1,
                Items = new List<OrderItemDto>
                {
                    new OrderItemDto {BookId = 1, Price = 10m, Count = 3},
                    new OrderItemDto {BookId = 2, Price = 100m, Count = 5},
                },
            });
        }
        
        [Fact]
        public void InitOrderItemColl_WithNullValue_ThrowsArgumentNullExeption()
        {
            Assert.Throws<ArgumentNullException>(() => new OrderItemCollection(null));
        }

        [Fact]
        public void Add_WithExistingItem_ThrowsInvalidArgumentException()
        {
            var order = CreateTestOrder();

            Assert.Throws<InvalidOperationException>(() => order.Items.Add(1, 15m, 2));
        }

        [Fact]
        public void AddItem_WithBookIdNotExist_AddOrderItem()
        {
            var order = CreateTestOrder();

            order.Items.Add(3, 15m, 1);

            Assert.Collection(order.Items, item => Assert.Equal(1, item.BookId),
                                           item => Assert.Equal(2, item.BookId),
                                           item => Assert.Equal(3, item.BookId));
            
            Assert.Equal(3, order.Items.Count);
        }

        [Fact]
        public void GetItem_WithExistingItem_ReturnsItem()
        {
            var order = CreateTestOrder();

            var expected = order.Items.ToArray()[0];

            var actual = order.Items.Get(1);

            Assert.Equal(expected, actual);
            Assert.Equal(3, actual.Count);
        }

        [Fact]
        public void GetItem_WithNotExistingValue_ThrowBookException()
        {
            var order = CreateTestOrder();

            Assert.Throws<InvalidOperationException>(() => order.Items.Get(3));
        }

        [Fact]
        public void RemoveItem_WithExistingItem_RemovesItem()
        {
            var order = CreateTestOrder();

            order.Items.Remove(1);

            Assert.Collection(order.Items, item => Assert.Equal(2, item.BookId));
        }

        [Fact]
        public void RemoveItem_WithNotExistingValue_ThrowBookException()
        {
            var order = CreateTestOrder();

            Assert.Throws<InvalidOperationException>(() => order.Items.Remove(3));
        }
    }
}
