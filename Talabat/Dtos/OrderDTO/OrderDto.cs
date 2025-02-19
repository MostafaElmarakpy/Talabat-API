namespace Talabat.Dtos.OrderDTO
{
    public class OrderDto
    {
        public string BasketId { get; set; }
        public string Email { get; set; }
        public int DeliveryMethodId { get; set; }
        public AddressDto ShippingAddress { get; set; }
    }
}
