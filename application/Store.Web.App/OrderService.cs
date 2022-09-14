using Microsoft.AspNetCore.Http;
using PhoneNumbers;
using Store.Messages;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public bool TryGetModel(out OrderModel model)
        {
            if (TryGetOrder(out Order order))
            {
                model = Map(order);
                return true;
            }

            model = null;
            return false;
        }

        private OrderModel Map(Order order)
        {
            var books = GetBooks(order);

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

        private IEnumerable<Book> GetBooks(Order order)
        {
            var bookId = order.Items.Select(item => item.BookId);

            return bookRepository.GetAllByIds(bookId);
        }

        internal bool TryGetOrder(out Order order)
        {
            if (Session.TryGetCart(out Cart cart))
            {
                order = orderRepository.GetById(cart.OrderId);
                return true;
            }

            order = null;
            return false;
        }

        public OrderModel AddBook(int bookId, int count)
        {
            if (count < 0)
                throw new InvalidOperationException("Uncorrect books count.");

            if (!TryGetOrder(out Order order)) 
                order = orderRepository.Create();

            AddOrUpdateBook(order, bookId, count);
            UpdateSession(order);

            return Map(order);
        }

        internal void UpdateSession(Order order)
        {
            var cart = new Cart(order.Id, order.TotalCount, order.TotalPrice);
            Session.Set(cart);
        }

        internal void AddOrUpdateBook(Order order, int bookId, int count)
        {
            var book = bookRepository.GetById(bookId);

            if (order.Items.TryGet(bookId, out OrderItem orderItem))
                orderItem.Count += count;

            else
                order.Items.Add(bookId, book.Price, count);

            orderRepository.Update(order);
        }

        public OrderModel UpdateBook(int bookId, int count)
        {
            Order order = GetOrder();
            order.Items.Get(bookId).Count = count;

            orderRepository.Update(order);
            UpdateSession(order);

            return Map(order);
        }

        public OrderModel RemoveBook(int bookId)
        {
            Order order = GetOrder();
            order.Items.Remove(bookId);

            orderRepository.Update(order);
            UpdateSession(order);

            return Map(order);
        }

        public Order GetOrder()
        {
            if (TryGetOrder(out Order order))
                return order;

            throw new InvalidOperationException("Session is empty.");
        }

        public OrderModel SendConfirmation(string mobilePhone)
        {
            var order = GetOrder();
            var model = Map(order);

            if (TryFormatPhone(mobilePhone, out string formattedPhone))
            {
                var confirmationCode = 1111; // random.Next(1000, 10000)
                model.MobilePhone = formattedPhone;
                Session.SetInt32(formattedPhone, confirmationCode);
                notificationService.SendConfirmationCode(mobilePhone, confirmationCode);
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

        public OrderModel ConfirmMobilePhone(string mobilePhone, int confirmationCode)
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

            var order = GetOrder();
            order.MobilePhone = mobilePhone;
            orderRepository.Update(order);

            Session.Remove(mobilePhone);

            return Map(order);
        }

        public OrderModel SetDelivery(OrderDelivery delivery)
        {
            var order = GetOrder();
            order.Delivery = delivery;
            orderRepository.Update(order);

            return Map(order);
        }

        public OrderModel SetPayment(OrderPayment payment)
        {
            var order = GetOrder();
            order.Payment = payment;
            orderRepository.Update(order);
            Session.RemoveCart();

            return Map(order);
        }

    }
}
