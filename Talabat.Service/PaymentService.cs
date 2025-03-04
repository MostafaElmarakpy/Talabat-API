﻿using Microsoft.Extensions.Configuration;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Order_Aggregate;
using Talabat.Core.Repositories;
using Talabat.Core.Service;
using Talabat.Core.Specifications;
using Talabat.Core.Specifications.OrderSpecifications;
using Product = Talabat.Core.Entities.Product;

namespace Talabat.Service
{
    public class PaymentService(IConfiguration configuration,
        IBasketRepository basketRepository,
        IUnitOfWork unitOfWork
            ) : IPaymentService
    {
        private readonly IConfiguration _configuration = configuration;
        private readonly IBasketRepository _basketRepository = basketRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        // if basket don't have payment Intent create it else update 
        public async Task<CustomerBasket> CreateOrUpdatePaymentIntent(string basketId)
        {

            StripeConfiguration.ApiKey = _configuration["StripeSetting:Secretkey"];

            // get basket to check the price and delivery Method
            var basket = await _basketRepository.GetBasketAsync(basketId);

            if (basket == null) return null;

            var shippingPrice = 0m;

            if (basket.DeliveryMethodId.HasValue)
            {
                var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(basket.DeliveryMethodId.Value);
                shippingPrice = deliveryMethod.Cost;
                basket.ShippingPrice = deliveryMethod.Cost;
            }

            // check the price is correct from DB
            foreach (var item in basket.Items)
            {
                var product = await _unitOfWork.Repository<Product>().GetByIdAsync(item.Id);

                if (item.Price != product.Price)
                    item.Price = product.Price;
            }

            var service = new PaymentIntentService();
            PaymentIntent intent;

            //create Payment Intent
            if (string.IsNullOrEmpty(basket.PaymentIntentId))
            {
                var options = new PaymentIntentCreateOptions()
                {
                    Amount = (long)basket.Items.Sum(item => item.Price * item.Quantity) * 100 + (long)shippingPrice * 100,
                    Currency = "usd",
                    PaymentMethodTypes = new List<string> { "card" },
                };
                intent = await service.CreateAsync(options);

                basket.PaymentIntentId = intent.Id;
                basket.ClientSecret = intent.ClientSecret;

            }

            //Update Payment Intent
            else
            {
                var option = new PaymentIntentUpdateOptions()
                {
                    Amount = (long)basket.Items.Sum(item => item.Price * item.Quantity) * 100 + (long)shippingPrice * 100,

                };
                await service.UpdateAsync(basket.PaymentIntentId, option);
            }

            await _basketRepository.UpdateBasketAsync(basket);
            return basket;

        }


        public async Task<Order> UpdatePaymentIntentToSucceedOrFailed(string paymentId, bool IsSucceed)
        {
            var spec = new OrderBypaymentIntentIdSpec(paymentId);
            var order = await _unitOfWork.Repository<Order>().GetByIdWithSpecAsync(spec);

            if (IsSucceed)
                order.Status = OrderStatus.PaymentRecived;
            else
                order.Status = OrderStatus.PaymentFailed;

            _unitOfWork.Repository<Order>().Update(order);

            await _unitOfWork.Complete();

            return order;
        }
    }
}