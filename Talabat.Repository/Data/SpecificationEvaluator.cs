using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Specifications;

namespace Talabat.Repository.Data
{
    public class SpecificationEvaluator<TEntity> where TEntity : BaseEntity
    {
        public static IQueryable<TEntity> GetQuery(IQueryable<TEntity> inputQuery, ISpecification<TEntity> spec)
        {

            var query = inputQuery; //_context.Set<Product>()

            if (spec.Criteria != null) //  p=> p.Id == 10
                query = query.Where(spec.Criteria);

            if (spec.IsPaginationEnable) //   
                query = query.Skip(spec.Skip).Take(spec.Take);

            if (spec.OrderBy != null) //  
                query = query.OrderBy(spec.OrderBy);

            if (spec.OrderByDescending != null) //  p=> p.price
                query = query.OrderByDescending(spec.OrderByDescending);


            query = spec.Includes.Aggregate(query, (currentQuery, IncludeExpression) => currentQuery.Include(IncludeExpression));
            // _context.Set<Product>().Where(p=> p.Id == 10).Include(P => P.ProductBrand)
            //query = spec.Includes.Aggregate(query, (N1, N2) => N1.Include(N2));

            return query;
        }
    }
}
