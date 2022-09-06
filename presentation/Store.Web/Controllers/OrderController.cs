using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Store.Contractors;
using Store.Messages;
using Store.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Store.Web.Controllers
{
    public class OrderController : Controller
    {
        private readonly IBookRepository bookRepository;
        private readonly IOrderRepository orderRepository;
        private readonly INotificationService notificationService;
        private readonly IEnumerable<IDeliveryService> deliveryServices;

        public OrderController(IBookRepository bookRepository, 
                               IOrderRepository orderRepository,
                               INotificationService notificationService,
                               IEnumerable<IDeliveryService> deliveryServices)
        {
            this.bookRepository = bookRepository;
            this.orderRepository = orderRepository;
            this.notificationService = notificationService;
            this.deliveryServices = deliveryServices;
        }

        private OrderModel Map(Order order)
        {
            var bookIds = order.Items.Select(item => item.BookId);
            var books = bookRepository.GetAllByIds(bookIds);
            var itemModels = order.Items
                                  .Join(books,
                                        item => item.BookId,
                                        book => book.Id,
                                        (item, book) => new OrderItemModel
                                        {
                                            BookId = book.Id,
                                            Title = book.Title,
                                            Author = book.Author,
                                            Count = item.Count,
                                            Price = item.Price
                                        });

            return new OrderModel
            {
                Id = order.Id,
                Items = itemModels.ToArray(),
                TotalCount = order.TotalCount,
                TotalPrice = order.TotalPrice
            };
        }
        
        [HttpGet]
        public IActionResult Index()
        {
            if (HttpContext.Session.TryGetCart(out Cart cart))
            {
                var order = orderRepository.GetById(cart.OrderId);
                OrderModel model = Map(order);

                return View(model);
            }

            return View("Empty");
        }

        [HttpPost]
        public IActionResult AddItem(int bookId, int count = 1)
        {
            (Order order, Cart cart) = GetOrCreateOrderAndCart();

            var book = bookRepository.GetById(bookId);

            order.AddOrUpdateItem(book, count);

            SaveOrderAndCart(order, cart);

            return RedirectToAction("Index", "Book", new { id = bookId });
        }

        [HttpPost]
        public IActionResult UpdateItem(int bookId, int count)
        {
            (Order order, Cart cart) = GetOrCreateOrderAndCart();

            order.GetItem(bookId).Count = count;

            SaveOrderAndCart(order, cart);

            return RedirectToAction("Index", "Book", new { id = bookId });
        }

        [HttpPost]
        public IActionResult RemoveItem(int bookId)
        {
            (Order order, Cart cart) = GetOrCreateOrderAndCart();

            order.RemoveItem(bookId);

            SaveOrderAndCart(order, cart);

            return RedirectToAction("Index", "Order"); 
        }

        private (Order order, Cart cart) GetOrCreateOrderAndCart()
        {
            Order order;

            if (HttpContext.Session.TryGetCart(out Cart cart))
            {
                order = orderRepository.GetById(cart.OrderId);
            }

            else
            {
                order = orderRepository.Create();
                cart = new Cart(order.Id);
            }

            return (order, cart);
        }

        private void SaveOrderAndCart(Order order, Cart cart)
        {
            orderRepository.Update(order);

            cart.TotalCount = order.TotalCount;
            cart.TotalPrice = order.TotalPrice;

            HttpContext.Session.Set(cart);
        }

        [HttpPost]
        public IActionResult SendConfirmationCode(int id, string mobilePhone)
        {
            var order = orderRepository.GetById(id);
            var model = Map(order);

            if (!IsValidCellPhone(mobilePhone))
            {
                model.Errors["mobilePhone"] = "The mobilephone number doesn`t respont required format +79876543210";
                return View("Index", model);
            }

            int code = 1111; // random.Next(1000, 10000)
            HttpContext.Session.SetInt32(mobilePhone, code);
            notificationService.SendConfirmationCode(mobilePhone, code);

            return View("Confirmation",
                        new ConfirmationModel
                        {
                            OrderId = id,
                            MobilePhone = mobilePhone
                        });
        }

        private bool IsValidCellPhone(string mobilePhone)
        {
            if (mobilePhone == null)
                return false;

            mobilePhone = mobilePhone.Replace(" ", "")
                                 .Replace("-", "");

            return Regex.IsMatch(mobilePhone, @"^\+?\d{11}$");
        }

        [HttpPost]
        public IActionResult Confirmate(int id, string mobilePhone, int code)
        {
            int? storedCode = HttpContext.Session.GetInt32(mobilePhone);

            if (storedCode == null)
                return View("Confirmation",
                            new ConfirmationModel()
                            {
                                OrderId = id,
                                MobilePhone = mobilePhone,
                                Errors = new Dictionary<string, string>()
                                {
                                    { "code", "Code fiels can`t be empty. Repeat sending" }
                                }
                            });

            if (storedCode != code)
                return View("Confirmation",
                            new ConfirmationModel()
                            {
                                OrderId = id,
                                MobilePhone = mobilePhone,
                                Errors = new Dictionary<string, string>()
                                {
                                    { "code", "Code differs from send one" }
                                }
                            });

            var order = orderRepository.GetById(id);
            order.MobilePhone = mobilePhone;
            orderRepository.Update(order);

            HttpContext.Session.Remove(mobilePhone);
   
            var model = new DeliveryModel 
            { 
                OrderId = id,
                Methods = deliveryServices.ToDictionary(service => service.UniqueCode,
                                                        service => service.Title)
            };

            return View("DeliveryMethod", model);
        }

        [HttpPost]
        public IActionResult StartDelivery(int id, string uniqueCode)
        {
            var deliveryService = deliveryServices.Single(service => service.UniqueCode == uniqueCode);

            var order = orderRepository.GetById(id);

            var form = deliveryService.CreateForm(order);

            return View("DeliveryStep", form);
        }

        [HttpPost]
        public IActionResult NextDelivery(int id, string uniqueCode, int step, Dictionary<string, string> values)
        {
            var deliveryService = deliveryServices.Single(service => service.UniqueCode == uniqueCode);

            var form = deliveryService.MoveNextForm(id, step, values);

            if (form.IsFinal)
            {
                return null;
            }

            return View("DeliveryStep", form);
        }
    }
}
