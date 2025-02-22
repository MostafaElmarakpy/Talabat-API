﻿using Talabat.Core.Entities;

namespace Talabat.Dtos
{
    public class ProductToReturnDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string PictureUrl { get; set; }
        public int ProductBrandId { get; set; }
        public int ProductTypeId { get; set; }
        public string? ProductBrand { get; set; }
        public string? ProductType { get; set; }
    }
}
