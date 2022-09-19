using System.Threading.Tasks;

namespace Store
{
    public interface IOrderRepository
    {
        Order Create();

        Order GetById(int id);

        void Update(Order order);

        Task<Order> GetByIdAsync(int orderId);

        Task<Order> CreateAsync();

        Task UpdateAsync(Order order);
    }
}
