using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Order_Aggregate;
using Talabat.Data;

namespace Talabat.Repository.Data
{
    public class ApplicationDbContextSeed
    {

        public static async Task SeedAsync(ApplicationDbContext context, ILoggerFactory loggerFactory)
        {
            try
            {
                // Seed Product Brands
                if (!context.ProductBrands.Any())
                {
                    var brandsData = await File.ReadAllTextAsync("../Talabat.Repository/Data/DataSeed/brands.json");
                    var brands = JsonSerializer.Deserialize<List<ProductBrand>>(brandsData);

                    foreach (var brand in brands)
                    {
                        await context.ProductBrands.AddAsync(brand);
                    }
                    await context.SaveChangesAsync();
                }

                // Seed Product Types
                if (!context.ProductTypes.Any())
                {
                    var typesData = await File.ReadAllTextAsync("../Talabat.Repository/Data/DataSeed/types.json");
                    var types = JsonSerializer.Deserialize<List<ProductType>>(typesData);

                    foreach (var type in types)
                    {
                        await context.ProductTypes.AddAsync(type);
                    }
                    await context.SaveChangesAsync();
                }

                // Seed Products
                if (!context.Products.Any())
                {
                    var productsData = await File.ReadAllTextAsync("../Talabat.Repository/Data/DataSeed/products.json");
                    var products = JsonSerializer.Deserialize<List<Product>>(productsData);
                    foreach (var product in products)
                    {
                        await context.Products.AddAsync(product);
                    }
                    await context.SaveChangesAsync();
                }
                if (!context.DeliveryMethods.Any())
                {
                    var deliveryMethodsData = await File.ReadAllTextAsync("../Talabat.Repository/Data/DataSeed/delivery.json");
                    var deliveryMethods = JsonSerializer.Deserialize<List<DeliveryMethod>>(deliveryMethodsData);
                    foreach (var deliveryMethod in deliveryMethods)
                    {
                        await context.Set<DeliveryMethod>().AddAsync(deliveryMethod);
                    }
                    await context.SaveChangesAsync();
                }


            }
            catch (Exception ex)
            {
                var logger = loggerFactory.CreateLogger<ApplicationDbContextSeed>();
                logger.LogError(ex.Message);
            }
        }

    }
}
