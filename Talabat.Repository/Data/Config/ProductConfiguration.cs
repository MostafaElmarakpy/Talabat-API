using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;

namespace Talabat.Repository.Data.Config
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {

            builder.Property(p => p.Name)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(p => p.Description)
                   .HasMaxLength(500);

            builder.Property(p => p.Price)
                   .HasPrecision(18, 2);

            builder.Property(p => p.PictureUrl)
                    .IsRequired();

            // Relationships
            builder.HasOne(p => p.ProductBrand)
                   .WithMany()
                   .HasForeignKey(p => p.ProductBrandId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.ProductType).WithMany()
                   .HasForeignKey(p => p.ProductTypeId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(p => p.Name);
            builder.HasIndex(p => p.Price);


        }
    }
}
