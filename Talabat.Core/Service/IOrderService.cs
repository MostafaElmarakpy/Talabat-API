﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.Order_Aggregate;

namespace Talabat.Core.Service
{
    public interface IOrderService
    {
        Task<Order> CreateOrderAsync(string buyerEmail, string basketId, int deliveryMethodId, Address ShippingAddress);

        Task<IReadOnlyList<Order>> GetOrderForUserAsync(string buyerEmail);

        Task<Order> GetOrderByIdForUserAsync(int orderId, string buyerEmail);

        Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodAsync();
    }
}
