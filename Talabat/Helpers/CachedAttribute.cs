using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.CodeDom.Compiler;
using System.Drawing;
using System.Text;
using Talabat.Core.Repositories;

namespace Talabat.Helpers
{
    public class CachedAttribute : Attribute, IAsyncActionFilter
    {
        private readonly int _timeToLive;

        public CachedAttribute(int timeToLive)
        {
            _timeToLive = timeToLive;
        }
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // object from IResponseCacheSerice D I
            var cacheService = context.HttpContext.RequestServices.GetRequiredService<IResponseCacheSerice>();

            var cacheKey = GenerateCasheKeyFromRequest(context.HttpContext.Request);

            var cacheResponse = await cacheService.GetCachedResponseAsync(cacheKey);
            if (!string.IsNullOrEmpty(cacheResponse))
            {
                var contentResult = new ContentResult()
                {
                    Content = cacheResponse,
                    ContentType = "application/Json",
                    StatusCode = 200,

                };
                context.Result = contentResult;
                return;
            }
            //after finsh end point 
            var exeutedEndointContext = await next();

            if (exeutedEndointContext.Result is OkObjectResult okObjectResult)
            {

                await cacheService.CashResponseAsync(cacheKey, okObjectResult.Value, TimeSpan.FromSeconds(_timeToLive));
            }

        }


        private string GenerateCasheKeyFromRequest(HttpRequest request)
        {
            //api/products?pageIndex=1&Size=5&sort=name
            var keyBuilder = new StringBuilder();

            keyBuilder.Append(request.Path);// /api/products

            //pageIndex = 1 & Size = 5 & sort = name
            foreach (var (key, value) in request.Query.OrderBy(x=>x.Key))
            {
                keyBuilder.Append($"|{key}-{value}");
            }
            return keyBuilder.ToString();
        }
    }
}
