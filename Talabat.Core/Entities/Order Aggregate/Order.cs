using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talabat.Core.Entities.Order_Aggregate
{
    public class Order : BaseEntity
    {
        private readonly string _paymentIntentId;
        public Order(string buyerEmail, Address shipToAddress, DeliveryMethod deliveryMethod, IReadOnlyCollection<OrderItem> orderItems, decimal subtotal, string paymentIntentId)
        {
            PaymentIntentId = paymentIntentId;
            BuyerEmail = buyerEmail;
            ShipToAddress = shipToAddress;
            DeliveryMethod = deliveryMethod;
            OrderItems = orderItems;
            Subtotal = subtotal;
        }
        public Order()
        {

        }
        public string BuyerEmail { get; set; }

        public DateTimeOffset OrderDate { get; set; } = DateTimeOffset.UtcNow;

        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        public Address ShipToAddress { get; set; }


        public int DeliveryMethodId { get; set; }

        public DeliveryMethod DeliveryMethod { get; set; }

        public IReadOnlyCollection<OrderItem> OrderItems { get; private set; } = new List<OrderItem>(); //navigation property many to one

        public decimal Subtotal { get; set; }

        public decimal GetTotal()
            => Subtotal + DeliveryMethod.Cost;

        public string PaymentIntentId { get; set; }

    }

}
