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

namespace Talabat.Service
{
    public class OrderService(
        IBasketRepository basketRepo,
        IUnitOfWork unitOfWork,
        IPaymentService paymentService
            ) : IOrderService
    {
        private readonly IBasketRepository _basketRepo = basketRepo;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IPaymentService _paymentService = paymentService;

        #region Order Operations
        public async Task<Order> CreateOrderAsync(string buyerEmail, string basketId, int deliveryMethodId, Address shippingAddress)
        {
            //1. Get Basket From BasketRepository
            var basket = await _basketRepo.GetBasketAsync(basketId);
            if (basket == null || !basket.Items.Any())
                throw new ArgumentException("Basket is empty or invalid");

            // 2. Process Order Items
            var orderItems = new List<OrderItem>();
            foreach (var item in basket.Items)
            {
                var product = await _unitOfWork.Repository<Product>().GetByIdAsync(item.Id);

                if (product == null) throw new Exception($"Product {item.Id} not found");
                var productItemOrder = new ProductItemOrder(product.Id, product.Name, product.PictureUrl);

                var orderItem = new OrderItem(productItemOrder, product.Price, item.Quantity);

                orderItems.Add(orderItem);
            }

            // 3. Calculate Order Totals
            var subtotal = orderItems.Sum(item => item.Price * item.Quantity);

            // 4. Validate Delivery Method
            var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(deliveryMethodId);

            // 5. Handle Existing Payment Intents
            var spec = new OrderBypaymentIntentIdSpec(basket.PaymentIntentId);
            var existingOrder = await _unitOfWork.Repository<Order>().GetByIdWithSpecAsync(spec);

            if (existingOrder != null)
            {
                _unitOfWork.Repository<Order>().Delete(existingOrder);
                await _paymentService.CreateOrUpdatePaymentIntent(basketId);
            }


            // 6. Create and Save Order
            var order = new Order(
                buyerEmail,
                shippingAddress,
                deliveryMethod,
                orderItems,
                subtotal,
                basket.PaymentIntentId
            );

            await _unitOfWork.Repository<Order>().CreateAsync(order);

            var result = await _unitOfWork.Complete();
            if (result <= 0) return null;
            return order;

        }
        #endregion
        public async Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodAsync()
        {
            var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetAllAsync();
            return deliveryMethod;
        }

        public async Task<Order> GetOrderByIdForUserAsync(int orderId, string buyerEmail)
        {
            var spec = new OrderWithItemsAndDeliveryMethodSpec(orderId, buyerEmail);
            var order = await _unitOfWork.Repository<Order>().GetByIdWithSpecAsync(spec);
            return order;
        }

        public async Task<IReadOnlyList<Order>> GetOrderForUserAsync(string buyerEmail)
        {
            {
                var ordersRepo = _unitOfWork.Repository<Order>();
                var spec = new OrderWithItemsAndDeliveryMethodSpec(buyerEmail);
                var orders = await ordersRepo.GetAllWithSpecAsync(spec);
                return orders.ToList();
            }
        }
    }
}