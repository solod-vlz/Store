
namespace Store.Web.App
{
    public class Cart
    {
        public int OrderId { get; }

        public int TotalCount { get; }

        public decimal TotalPrice { get; }

        public Cart(int orderId, int totalCount, decimal totalPrice)
        {
            OrderId = orderId;
            TotalPrice = totalPrice;
            TotalCount = totalCount;
        }
    }
}
