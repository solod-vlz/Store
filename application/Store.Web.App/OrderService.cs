using Microsoft.AspNetCore.Http;
using PhoneNumbers;
using Store.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Store.Web.App
{
    public class OrderService
    {
        private readonly IBookRepository bookRepository;
        private readonly IOrderRepository orderRepository;
        private readonly INotificationService notificationService;
        private readonly IHttpContextAccessor httpContextAccessor;

        protected ISession Session => httpContextAccessor.HttpContext.Session;

        public OrderService(IBookRepository bookRepository,
                            IOrderRepository orderRepository,
                            INotificationService notificationService,
                            IHttpContextAccessor httpContextAccessor)
        {
            this.bookRepository = bookRepository;
            this.orderRepository = orderRepository;
            this.notificationService = notificationService;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<(bool hasValue, OrderModel model)> TryGetModelAsync()
        {
            var (hasValue, order) = await TryGetOrderAsync();

            if (hasValue)
                return (true, await MapAsync(order));

            return (false, null);
        }

        private async Task<OrderModel> MapAsync(Order order)
        {
            var books = await GetBooksAsync(order);

            var items = order.Items.Join(books,
                                    orderItem => orderItem.BookId,
                                    book => book.Id,
                                    (orderItem, book) => new OrderItemModel
                                    {
                                        BookId = book.Id,
                                        Author = book.Author,
                                        Title = book.Title,
                                        Price = orderItem.Price,
                                        Count = orderItem.Count,
                                    });

            return new OrderModel
            {
                Id = order.Id,
                Items = items.ToArray(),
                TotalCount = order.TotalCount,
                TotalPrice = order.TotalPrice,
                MobilePhone = order.MobilePhone,
                DeliveryDescription = order.Delivery?.Description,
                PaymentDescription = order.Payment?.Description,
            };
        }

        private async Task<IEnumerable<Book>> GetBooksAsync(Order order)
        {
            var bookId = order.Items.Select(item => item.BookId);

            return await bookRepository.GetAllByIdsAsync(bookId);
        }

        internal async Task<(bool hasValue, Order order)> TryGetOrderAsync()
        {
            if (Session.TryGetCart(out Cart cart))
            {
                var order = await orderRepository.GetByIdAsync(cart.OrderId);
                return (true, order);
            }

            return (false, null);
        }

        public async Task<OrderModel> AddBookAsync(int bookId, int count)
        {
            if (count < 0)
                throw new InvalidOperationException("Uncorrect books count.");

            var (hasValue, order) = await TryGetOrderAsync();

            if (!hasValue)
                order = await orderRepository.CreateAsync();

            await AddOrUpdateBookAsync(order, bookId, count);
            UpdateSession(order);

            return await MapAsync(order);
        }

        internal void UpdateSession(Order order)
        {
            var cart = new Cart(order.Id, order.TotalCount, order.TotalPrice);
            Session.Set(cart);
        }

        internal async Task AddOrUpdateBookAsync(Order order, int bookId, int count)
        {
            var book = await bookRepository.GetByIdAsync(bookId);

            if (order.Items.TryGet(bookId, out OrderItem orderItem))
                orderItem.Count += count;

            else
                order.Items.Add(bookId, book.Price, count);

            await orderRepository.UpdateAsync(order);
        }

        public async Task<OrderModel> UpdateBookAsync(int bookId, int count)
        {
            Order order = await GetOrderAsync();
            order.Items.Get(bookId).Count = count;

            await orderRepository.UpdateAsync(order);
            UpdateSession(order);

            return await MapAsync(order);
        }

        public async Task<OrderModel> RemoveBookAsync(int bookId)
        {
            Order order = await GetOrderAsync();
            order.Items.Remove(bookId);

            await orderRepository.UpdateAsync(order);
            UpdateSession(order);

            return await MapAsync(order);
        }

        public async Task<Order> GetOrderAsync()
        {
            var (hasValue, order) = await TryGetOrderAsync();

            if (hasValue) return order;

            throw new InvalidOperationException("Session is empty.");
        }

        public async Task<OrderModel> SendConfirmationAsync(string mobilePhone)
        {
            var order = await GetOrderAsync();
            var model = await MapAsync(order);

            if (TryFormatPhone(mobilePhone, out string formattedPhone))
            {
                var confirmationCode = 1111; // random.Next(1000, 10000)
                model.MobilePhone = formattedPhone;
                Session.SetInt32(formattedPhone, confirmationCode);
                await notificationService.SendConfirmationCodeAsync(mobilePhone, confirmationCode);
            }

            else
                model.Errors["mobilePhone"] = "The mobilephone number doesn`t respont required format +79876543210";

            return model;
        }

        private readonly PhoneNumberUtil phoneNumberUtil = PhoneNumberUtil.GetInstance();

        internal bool TryFormatPhone(string cellPhone, out string formattedPhone)
        {
            try
            {
                var phoneNumber = phoneNumberUtil.Parse(cellPhone, "ru");
                formattedPhone = phoneNumberUtil.Format(phoneNumber, PhoneNumberFormat.INTERNATIONAL);
                return true;
            }
            catch (NumberParseException)
            {
                formattedPhone = null;
                return false;
            }
        }

        public async Task<OrderModel> ConfirmMobilePhoneAsync(string mobilePhone, int confirmationCode)
        {
            int? storedCode = Session.GetInt32(mobilePhone);
            var model = new OrderModel();

            if (storedCode == null)
            {
                model.Errors["mobilePhone"] = "Code fiels can`t be empty. Repeat sending";
                return model;
            }

            if (storedCode != confirmationCode)
            {
                model.Errors["mobilePhone"] = "Code differs from send one. Try one more time";
                return model;
            }

            var order = await GetOrderAsync();
            order.MobilePhone = mobilePhone;
            await orderRepository.UpdateAsync(order);

            Session.Remove(mobilePhone);

            return await MapAsync(order);
        }

        public async Task<OrderModel> SetDeliveryAsync(OrderDelivery delivery)
        {
            var order = await GetOrderAsync();
            order.Delivery = delivery;
            await orderRepository.UpdateAsync(order);

            return await MapAsync(order);
        }

        public async Task<OrderModel> SetPaymentAsync(OrderPayment payment)
        {
            var order = await GetOrderAsync();
            order.Payment = payment;
            await orderRepository.UpdateAsync(order);
            Session.RemoveCart();

            notificationService.StartProcess(order);

            return await MapAsync(order);
        }
    }
}
