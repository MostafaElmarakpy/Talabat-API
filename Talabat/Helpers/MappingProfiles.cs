using AutoMapper;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Identity;
using Talabat.Core.Entities.Order_Aggregate;
using Talabat.Dtos;
using Talabat.Dtos.BasketDTO;
using Talabat.Dtos.OrderDTO;

namespace Talabat.Helpers
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Product, ProductToReturnDto>()
                .ForMember(d => d.ProductBrand, o => o.MapFrom(S => S.ProductBrand.Name))
                .ForMember(d => d.ProductType, o => o.MapFrom(S => S.ProductType.Name))
                .ForMember(d => d.PictureUrl, o => o.MapFrom<ProuductPictureUrlResolver>());

            CreateMap<CustomerBasketDto, CustomerBasket>().ReverseMap();
            //.ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));
            CreateMap<BasketItemDto, BasketItem>();

            CreateMap<AddressDto, Core.Entities.Identity.Address>().ReverseMap();
            CreateMap<AddressDto, Core.Entities.Order_Aggregate.Address>().ReverseMap();


            CreateMap<Order, OrderToReturnDto>()
                .ForMember(d => d.DeliveryMethod, o => o.MapFrom(s => s.DeliveryMethod.ShortName))
                .ForMember(d => d.DeliveryMethodCost, o => o.MapFrom(s => s.DeliveryMethod.Cost));

            CreateMap<OrderItem, OrderItemDto>()
                    .ForMember(d => d.ProductId, o => o.MapFrom(s => s.Product.ProductId))
                    .ForMember(d => d.ProductName, o => o.MapFrom(s => s.Product.ProductName))
                    .ForMember(d => d.PictureUrl, o => o.MapFrom(s => s.Product.ProductUrl))
                    .ForMember(d => d.PictureUrl, o => o.MapFrom<OrderItemPicsUrlResolver>());


        }
    }
}