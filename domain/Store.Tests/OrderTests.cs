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

            Assert.Equal(3 + 5, order.TotalCount);
        }

        [Fact]
        public void TotalPrice_WithNoEmptyItems_CalculatesTotalPrice()
        {
            var order = new Order(1, new List<OrderItem>()
            {
                new OrderItem(1, 3, 10m),
                new OrderItem(2, 5, 100m)
            });

            Assert.Equal(3 * 10m + 5 * 100m, order.TotalPrice);
        }

        [Fact]
        public void AddItem_WithBookNullValue_ThrowsArgumentNullExeption()
        {
            var order = new Order(1, new OrderItem[0]);

            Assert.Throws<ArgumentNullException>(() => order.AddItem(null, 0));
        }

        [Fact]
        public void AddItem_WithBookIdExist_UpdateBookCount()
        {
            const int oldBookId = 1;
            const int oldBookCount = 1;

            var oldBookItem = new Book(oldBookId, "ISBN 1234567890", "Author", "Title", "Description", 10m);

            var orderItems = new OrderItem[1] { new OrderItem(oldBookItem.Id, oldBookCount, oldBookItem.Price) };

            var order = new Order(1, orderItems);

            var newBookItem = oldBookItem;

            order.AddItem(newBookItem, 1);

            Assert.Equal(oldBookId, order.Items.Count);
            Assert.Equal(oldBookCount + 1, order.Items.ToArray()[0].Count);
        }

        [Fact]
        public void AddItem_WithBookIdNotExist_UpdateBookCount()
        {
            const int oldBookId = 1;
            const int oldBookCount = 1;

            var oldBookItem = new Book(oldBookId, "ISBN 1234567890", "Author", "Title", "Description", 10m);

            var orderItems = new OrderItem[1] { new OrderItem(oldBookItem.Id, oldBookCount, oldBookItem.Price) };

            var order = new Order(1, orderItems);

            const int newBookId = oldBookId + 1;
            const int newBookCount = 1;

            var newBookItem = new Book(newBookId, "ISBN 1234567890", "Author", "Title", "Description", 10m); ;

            order.AddItem(newBookItem, newBookCount);

            Assert.Equal(2, order.Items.Count);

            Assert.Equal(oldBookId, order.Items.ToArray()[0].BookId);
            Assert.Equal(oldBookCount, order.Items.ToArray()[0].Count);

            Assert.Equal(newBookId, order.Items.ToArray()[1].BookId);
            Assert.Equal(newBookCount, order.Items.ToArray()[1].Count);
        }
    }
}
