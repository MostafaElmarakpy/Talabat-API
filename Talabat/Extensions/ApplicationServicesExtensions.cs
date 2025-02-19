using Microsoft.AspNetCore.Mvc;
using Talabat.Core.Repositories;
using Talabat.Core.Service;
using Talabat.Errors;
using Talabat.Helpers;
using Talabat.Repository;
using Talabat.Service;

namespace Talabat.Extensions
{
    public static class ApplicationServicesExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped(typeof(IOrderService), typeof(OrderService));
            services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));
            services.AddScoped(typeof(IPaymentService), typeof(PaymentService));
            services.AddScoped(typeof(IBasketRepository), typeof(BasketRepsoitory));
            services.AddSingleton(typeof(IResponseCacheSerice), typeof(ResponseCacheSerice));
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddAutoMapper(typeof(MappingProfiles));
            services.AddScoped(typeof(IProductService), typeof(ProductService));

            //services.AddAutoMapper(m => m.AddProfile<MappingProfiles>());

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", options =>
                {
                    options.AllowAnyHeader().AllowAnyMethod().WithOrigins("https://localhost:7299");
                });
            });

            //services.AddScoped<ProductPictureUrlResolver>();

            services.Configure<ApiBehaviorOptions>(options =>
                options.InvalidModelStateResponseFactory = actionContext =>
                {
                    var errors = actionContext.ModelState
                      .Where(M => M.Value.Errors.Count > 0)
                      .SelectMany(x => x.Value.Errors)
                      .Select(x => x.ErrorMessage)
                      .ToArray();

                    var validationErrorResponse = new ApiValidationErrorResponse
                    {
                        Errors = errors,
                        //Message = "خطأ في التحقق من صحة البيانات",
                        //StatusCode = StatusCodes.Status400BadRequest
                    };
                    return new BadRequestObjectResult(new ApiValidationErrorResponse
                    {
                        Errors = errors
                    });
                });
            return services;
        }
    }
}