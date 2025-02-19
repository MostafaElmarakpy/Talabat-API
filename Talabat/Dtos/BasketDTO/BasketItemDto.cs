using System.ComponentModel.DataAnnotations;

namespace Talabat.Dtos.BasketDTO
{
    public class BasketItemDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string PictureUrl { get; set; }

        [Range(1, double.MaxValue, ErrorMessage = " Price Must be Greater than Zero !!")]
        public required decimal Price { get; set; }
        public required string Category { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Quantity muste be one item at least !!")]
        public required int Quantity { get; set; }
        public required string Brand { get; set; }
        public required string Type { get; set; }
    }
}