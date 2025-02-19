using Talabat.Core.Entities.Order_Aggregate;

namespace Talabat.Dtos.OrderDTO
{
    public class OrderToReturnDto
    {
        public int Id { get; set; }
        public string BuyerEmail { get; set; }

        public DateTimeOffset OrderDate { get; set; }

        public string Status { get; set; }

        public Address ShipToAddress { get; set; }

        public int DeliveryMethodId { get; set; }

        public string DeliveryMethod { get; set; }
        public decimal DeliveryMethodCost { get; set; }

        public ICollection<OrderItemDto> OrderItems { get; set; }
        public decimal Subtotal { get; set; }

        public string PaymentIntentId { get; set; }

        public decimal Total { get; set; }

    }
}
