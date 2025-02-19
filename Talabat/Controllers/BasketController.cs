using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.Core.Entities;
using Talabat.Core.Repositories;
using Talabat.Dtos;

namespace Talabat.Controllers
{

    public class BasketController(IBasketRepository basketRepsitory, IMapper mapper)
        : BaseApiController
    {
        private readonly IBasketRepository _basketRepsitory = basketRepsitory;
        private readonly IMapper _mapper = mapper;

        [HttpGet] //
        public async Task<ActionResult<CustomerBasket>> GetBasketById(string id)
        {
            var basket = await _basketRepsitory.GetBasketAsync(id);

            return Ok(basket ?? new CustomerBasket(id));
        }


        [HttpPost]
        public async Task<ActionResult<CustomerBasket>> UpdateBasked(CustomerBasketDto basket)
        {
            var mappedBasket = _mapper.Map<CustomerBasketDto, CustomerBasket>(basket);
            var UpdatedOrCreateBasket = await _basketRepsitory.UpdateBasketAsync(mappedBasket);
            return Ok(UpdatedOrCreateBasket);
        }
        [HttpDelete]
        public async Task DeleteBasked(string id)
        {
            await _basketRepsitory.DeleteBasketAsync(id);
        }
    }
}
