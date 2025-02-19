using AutoMapper;
using Talabat.Core.Entities.Order_Aggregate;
using Talabat.Dtos.OrderDTO;

namespace Talabat.Helpers
{
    public class OrderItemPicsUrlResolver(IConfiguration configuration) : IValueResolver<OrderItem, OrderItemDto, string>
    {
        public IConfiguration Configuration { get; } = configuration;

        public string Resolve(OrderItem source, OrderItemDto destination, string destMember, ResolutionContext context)
        {
            if (string.IsNullOrEmpty(source.Product.ProductUrl))
                return $"{Configuration["BaseApiUrl"]}{source.Product.ProductUrl}";
            return null;
        }
    }
}
