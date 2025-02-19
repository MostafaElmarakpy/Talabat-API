using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;

namespace Talabat.Core.Specifications.ProductSpecifications
{
    public class ProductWithFilterCountSpes : BaseSpecification<Product>
    {
        public ProductWithFilterCountSpes(ProductSpecParams productParams)
        : base(p =>
                    (string.IsNullOrEmpty(productParams.Search) || p.Name.ToLower().Contains(productParams.Search)) &&
                    (!productParams.BrandId.HasValue || p.ProductBrandId == productParams.BrandId.Value) &&
                    (!productParams.TypeId.HasValue || p.ProductTypeId == productParams.TypeId.Value)
            )
        {
        }
    }
}
