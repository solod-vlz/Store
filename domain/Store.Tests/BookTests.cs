using System;
using Xunit;

namespace Store.Tests
{
    public class BookTests
    {
        [Fact]
        public void IsIsbn_WithNull_ReturnFalse()
        {
            var actual = Book.IsIsbn(null);

            Assert.False(actual);
        }

        [Fact]
        public void IsIsbn_WithBlankString_ReturnFalse()
        {
            var actual = Book.IsIsbn("    ");

            Assert.False(actual);
        }

        [Fact]
        public void IsIsbn_WithInvalidIsbn_ReturnFalse()
        {
            var actual = Book.IsIsbn("ISBN 111");

            Assert.False(actual);
        }

        [Fact]
        public void IsIsbn_WithIsbn10_ReturnTrue()
        {
            var actual = Book.IsIsbn("ISbn 123-342-345-1");

            Assert.True(actual);
        }

        [Fact]
        public void IsIsbn_WithIsbn13_ReturnTrue()
        {
            var actual = Book.IsIsbn("ISbn 123-342-345-1222");

            Assert.True(actual);
        }

        [Fact]
        public void IsIsbn_WithUncorrectStartAndEnd_ReturnFalse()
        {
            var actual = Book.IsIsbn("ssssISbn 123-342-345-1222sss");

            Assert.False(actual);
        }
    }
}
