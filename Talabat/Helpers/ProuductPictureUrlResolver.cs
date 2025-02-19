using AutoMapper;
using AutoMapper.Execution;
using Microsoft.IdentityModel.Tokens;
using Talabat.Core.Entities;
using Talabat.Dtos;

namespace Talabat.Helpers
{
    public class ProuductPictureUrlResolver : IValueResolver<Product, ProductToReturnDto, string>
    {
        public IConfiguration Configuration { get; }
        public ProuductPictureUrlResolver(IConfiguration configuration)
        {
            Configuration = configuration;
        }


        public string Resolve(Product source, ProductToReturnDto destination, string destMember, ResolutionContext context)
        {
            if (string.IsNullOrEmpty(source.PictureUrl))
                return $"{Configuration["BaseApiUrl"]}{source.PictureUrl}";
            return null;
        }
    }
}
