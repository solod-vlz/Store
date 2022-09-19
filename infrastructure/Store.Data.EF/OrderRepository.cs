using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Store.Data.EF
{
    public class OrderRepository : IOrderRepository
    {
        private readonly DbContextFactory dbContextFactory;

        public OrderRepository(DbContextFactory dbContextFactory)
        {
            this.dbContextFactory = dbContextFactory;
        }

        public Order Create()
        {
            var dbContext = dbContextFactory.Create(typeof(OrderRepository));

            var dto = Order.DtoFactory.Create();
            dbContext.Orders.Add(dto);
            dbContext.SaveChanges();

            return Order.Mapper.Map(dto);
        }

        public async Task<Order> CreateAsync()
        {
            var dbContext = dbContextFactory.Create(typeof(OrderRepository));

            var dto = Order.DtoFactory.Create();
            
            dbContext.Orders.Add(dto);
            await dbContext.SaveChangesAsync();

            return Order.Mapper.Map(dto);
        }

        public Order GetById(int id)
        {
            var dbContext = dbContextFactory.Create(typeof(OrderRepository));

            var dto = dbContext.Orders
                               .Include(order => order.Items)
                               .Single(order => order.Id == id);

            return Order.Mapper.Map(dto);
        }

        public async Task<Order> GetByIdAsync(int id)
        {
            var dbContext = dbContextFactory.Create(typeof(OrderRepository));

            var dto = await dbContext.Orders
                                     .Include(order => order.Items)
                                     .SingleAsync(order => order.Id == id);

            return Order.Mapper.Map(dto);
        }

        public void Update(Order order)
        {
            var dbContext = dbContextFactory.Create(typeof(OrderRepository));

            dbContext.SaveChanges();
        }

        public async Task UpdateAsync(Order order)
        {
            var dbContext = dbContextFactory.Create(typeof(OrderRepository));

            await dbContext.SaveChangesAsync();
        }
    }
}
