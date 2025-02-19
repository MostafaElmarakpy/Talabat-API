namespace Talabat.Dtos
{
    public class AddressDto
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Country { get; set; }
        public required string City { get; set; }
        public required string Street { get; set; }
    }
}
