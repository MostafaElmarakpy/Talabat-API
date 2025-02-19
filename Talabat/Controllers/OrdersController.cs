using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using System.Security.Claims;
using Talabat.Core.Entities.Order_Aggregate;
using Talabat.Core.Service;
using Talabat.Dtos;
using Talabat.Dtos.OrderDTO;
using Talabat.Errors;

namespace Talabat.Controllers
{
    [Authorize]
    public class OrdersController(
        IOrderService orderService, IMapper mapper
            ) : BaseApiController
    {
        private readonly IOrderService _orderService = orderService;
        private readonly IMapper _mapper = mapper;

        [HttpPost]

        public async Task<ActionResult<OrderToReturnDto>> CreateOrder(OrderDto orderDto)
        {
            var buyerEmail = User.FindFirstValue(ClaimTypes.Email);
            var orderAddress = _mapper.Map<AddressDto, Address>(orderDto.ShippingAddress);
            var order = await _orderService.CreateOrderAsync(buyerEmail, orderDto.BasketId, orderDto.DeliveryMethodId, orderAddress);
            if (order == null) return BadRequest(new ApiResponse(400));
            return Ok(_mapper.Map<Order, OrderToReturnDto>(order));
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<OrderToReturnDto>>> GetOrdersForUser()
        {
            var buyerEmail = User.FindFirstValue(ClaimTypes.Email);
            var orders = await _orderService.GetOrderForUserAsync(buyerEmail);

            if (orders == null) return BadRequest(new ApiResponse(400));
            return Ok(_mapper.Map<IReadOnlyList<Order>, IReadOnlyList<OrderToReturnDto>>(orders));
        }

        [HttpGet("{Id}")]
        public async Task<ActionResult<OrderToReturnDto>> GetOrderForUser(int id)
        {
            var buyerEmail = User.FindFirstValue(ClaimTypes.Email);
            var order = await _orderService.GetOrderByIdForUserAsync(id, buyerEmail);
            if (order == null) return BadRequest(new ApiResponse(400));
            return Ok(_mapper.Map<Order, OrderToReturnDto>(order));
        }

        [HttpGet("deliveryMethod")]
        public ActionResult<IReadOnlyList<DeliveryMethod>> GetDeliveryMethods()
        {
            var deliveryMethod = _orderService.GetDeliveryMethodAsync();
            return Ok(deliveryMethod);
        }
    }
}
