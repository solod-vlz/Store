using System.Threading.Tasks;

namespace Store
{
    public interface IOrderRepository
    { 
        Task<Order> GetByIdAsync(int orderId);

        Task<Order> CreateAsync();

        Task UpdateAsync(Order order);
    }
}
