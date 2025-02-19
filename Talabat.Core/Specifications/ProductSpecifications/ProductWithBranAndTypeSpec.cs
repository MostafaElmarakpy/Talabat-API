using Talabat.Core.Entities;

namespace Talabat.Core.Specifications.ProductSpecifications
{
    public class ProductWithBranAndTypeSpec : BaseSpecification<Product>
    {
        public ProductWithBranAndTypeSpec(ProductSpecParams productParams)
            : base(p =>
                        (string.IsNullOrEmpty(productParams.Search) || p.Name.ToLower().Contains(productParams.Search)) &&
                        (!productParams.BrandId.HasValue || p.ProductBrandId == productParams.BrandId.Value) &&
                        (!productParams.TypeId.HasValue || p.ProductTypeId == productParams.TypeId.Value)
            )
        {
            Includes.Add(p => p.ProductBrand); // Eager Loading
            Includes.Add(p => p.ProductType);


            ApplyPagination(productParams.PageSize * (productParams.PageIndex - 1), productParams.PageSize);

            if (!string.IsNullOrEmpty(productParams.Sort))
            {
                switch (productParams.Sort)
                {
                    case "priceAsc":
                        AddOrderBy(p => p.Price);
                        break;
                    case "priceDesc":
                        AddOrderByDescending(p => p.Price);
                        break;

                    default:
                        AddOrderBy(p => p.Name);
                        break;
                }
            }
        }
        public ProductWithBranAndTypeSpec(int id) : base(P => P.Id == id)
        {
            Includes.Add(p => p.ProductBrand);
            Includes.Add(p => p.ProductType);
        }
    }
}
