namespace Talabat.Core.Entities.Identity
{
    public class Address
    {
        public int Id { get; set; }
        public required string FName { get; set; }
        public required string LName { get; set; }
        public required string Country { get; set; }
        public string? City { get; set; }
        public string? Street { get; set; }
        public required string AppUserId { get; set; }
        //public AppUser User { get; set; } // [one]

    }
}