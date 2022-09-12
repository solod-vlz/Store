using System;
using System.Linq;
using Xunit;

namespace Store.Tests
{
    public class OrderItemCollectionTests
    {
        [Fact]
        public void InitOrderItemColl_WithNullValue_ThrowsArgumentNullExeption()
        {
            Assert.Throws<ArgumentNullException>(() => new OrderItemCollection(null));
        }

        [Fact]
        public void AddItem_WithBookIdExist_ThrowsInvalidArgumentException()
        {
            const int oldBookId = 1;

            var order = new Order(1, new OrderItem[1] { new OrderItem(oldBookId, 10m, 1) });

            Assert.Throws<InvalidOperationException>(() => order.Items.Add(oldBookId, 15m, 2));
        }

        [Fact]
        public void AddItem_WithBookIdNotExist_AddOrderItem()
        {
            const int oldBookId = 1;

            var order = new Order(1, new OrderItem[1] { new OrderItem(oldBookId, 10m, 1) });

            const int newBookId = oldBookId + 1;

            var newBookItem = new Book(newBookId, "ISBN 1234567890", "Author", "Title", "Description", 10m); ;

            order.Items.Add(newBookId, 15m, 1);

            Assert.Collection(order.Items, item => Assert.Equal(oldBookId, item.BookId),
                                           item => Assert.Equal(newBookId, item.BookId));

            Assert.Equal(2, order.Items.Count);
        }

        [Fact]
        public void GetItem_WithExistingItem_ReturnsItem()
        {
            var order = new Order(1, new[]
            {
                new OrderItem(1, 10m, 2),
                new OrderItem(2, 100m, 1),
            });

            var expected = order.Items.ToArray()[0];

            var actual = order.Items.Get(1);

            Assert.Equal(expected, actual);
            Assert.Equal(2, actual.Count);
        }

        [Fact]
        public void GetItem_WithNotExistingValue_ThrowBookException()
        {
            var order = new Order(1, new[]
            {
                new OrderItem(1, 10m, 2),
                new OrderItem(2, 100m, 1),
            });

            Assert.Throws<InvalidOperationException>(() => order.Items.Get(3));
        }

        [Fact]
        public void RemoveItem_WithExistingItem_RemovesItem()
        {
            var order = new Order(1, new[]
            {
                new OrderItem(1, 10m, 2),
                new OrderItem(2, 100m, 1),
            });

            order.Items.Remove(1);

            Assert.Collection(order.Items, item => Assert.Equal(2, item.BookId));
        }

        [Fact]
        public void RemoveItem_WithNotExistingValue_ThrowBookException()
        {
            var order = new Order(1, new[]
            {
                new OrderItem(1, 10m, 2),
                new OrderItem(2, 100m, 1),
            });

            Assert.Throws<InvalidOperationException>(() => order.Items.Remove(3));
        }
    }
}
