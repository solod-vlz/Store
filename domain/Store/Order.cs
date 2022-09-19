﻿using Store.Data;
using System;
using System.Linq;

namespace Store
{
    public class Order
    {
        private readonly OrderDto dto;

        public int Id => dto.Id;

        public OrderItemCollection Items { get; }

        public string MobilePhone
        {
            get => dto.MobilePhone;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentNullException(nameof(MobilePhone));

                dto.MobilePhone = value;
            }
        }

        public OrderDelivery Delivery
        {
            get
            {
                if (dto.DeliveryUniqueCode == null)
                    return null;

                return new OrderDelivery(dto.DeliveryUniqueCode, dto.DeliveryDescription, dto.DeliveryPrice, dto.DeliveryParameters);
            }

            set
            {
                if (value == null)
                    throw new ArgumentException(nameof(Delivery));

                dto.DeliveryUniqueCode = value.UniqueCode;
                dto.DeliveryDescription = value.Description;
                dto.DeliveryPrice = value.Price;
                dto.DeliveryParameters = value.Parameters.ToDictionary(p => p.Key, p => p.Value);
            }
        }

        public OrderPayment Payment
        {
            get
            {
                if (dto.PaymentServiceName == null)
                    return null;

                return new OrderPayment(dto.PaymentServiceName, dto.PaymentDescription, dto.PaymentParameters);
            }

            set
            {
                if (value == null)
                    throw new ArgumentException(nameof(Payment));

                dto.PaymentServiceName = value.UniqueCode;
                dto.PaymentDescription = value.Description;
                dto.PaymentParameters = value.Parameters.ToDictionary(p => p.Key, p => p.Value);
            }
        }

        public int TotalCount => Items.Sum(item => item.Count);

        public decimal TotalPrice => Items.Sum(item => item.Count * item.Price)
                                     + (Delivery?.Price ?? 0m);

        internal Order(OrderDto dto)
        {
            this.dto = dto;
            Items = new OrderItemCollection(dto);
        }

        public static class DtoFactory
        {
            public static OrderDto Create() => new OrderDto();
        }

        public static class Mapper
        {
            public static OrderDto Map(Order domain) => domain.dto;

            public static Order Map(OrderDto dto) => new Order(dto);
        }
    }
}
