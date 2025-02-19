using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using System.Text;
using Talabat.Core.Entities;
using Talabat.Core.Service;
using Talabat.Dtos;
using Talabat.Errors;
using System.IO;
using System.Threading.Tasks;
using Talabat.Core.Entities.Order_Aggregate;

namespace Talabat.Controllers
{

    public class PaymentsController : BaseApiController
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogger<PaymentsController> _logger;
        private readonly string _webhookSecret;

        public PaymentsController(
            IPaymentService paymentService,
            ILogger<PaymentsController> logger,
            IConfiguration configuration
            )
        {
            _paymentService = paymentService;
            _logger = logger;
            _webhookSecret = configuration["StripeSetting:WebhookSecret"];
        }


        [Authorize]
        [HttpPost("{basketId}")]
        public async Task<ActionResult<CustomerBasketDto>> CreateAndUpdatePaymentIntent(string basketId)
        {
            var basket = await _paymentService.CreateOrUpdatePaymentIntent(basketId);

            if (basket == null) return BadRequest(new ApiResponse(400, " a Problem with your Basket"));

            return Ok(basket);

        }




        // POST api/<PaymentsController>/webhook
        [HttpPost("webhook")]
        public async Task<IActionResult> WebHook()
        {
            // Read the request body as a string
            var json = await new StreamReader(HttpContext.Request.Body, Encoding.UTF8).ReadToEndAsync();


            // Validate the Stripe webhook signature
            var stripeEvent = EventUtility.ConstructEvent
                (
                    json,
                    Request.Headers["Stripe-Signature"],
                    _webhookSecret
                );


            PaymentIntent intent;
            Order order;

            // Handle the event based on its type
            switch (stripeEvent.Type)
            {
                case EventTypes.PaymentIntentSucceeded:
                    intent = (PaymentIntent)stripeEvent.Data.Object;
                    order = await _paymentService.UpdatePaymentIntentToSucceedOrFailed(intent.Id, true);
                    // Handle successful payment intent

                    _logger.LogInformation($"Payment succeeded: {intent.Id}");
                    Console.WriteLine($"PaymentIntent succeeded: {intent.Id}");
                    break;

                case EventTypes.PaymentIntentPaymentFailed:
                    intent = (PaymentIntent)stripeEvent.Data.Object;
                    order = await _paymentService.UpdatePaymentIntentToSucceedOrFailed(intent.Id, true);
                    // Handle failed payment intent

                    _logger.LogWarning($"Payment failed: {intent.Id}");
                    Console.WriteLine($"PaymentIntent failed: {intent.Id}");
                    break;

                //case EventTypes.ChargeSucceeded:
                //    var charge = stripeEvent.Data.Object as Charge;
                //    // Handle successful charge
                //    Console.WriteLine($"Charge succeeded: {charge.Id}");
                //    break;

                //case EventTypes.ChargeRefunded:
                //    var refundedCharge = stripeEvent.Data.Object as Charge;
                //    // Handle refunded charge
                //    Console.WriteLine($"Charge refunded: {refundedCharge.Id}");
                //    break;

                //// Add more event types as needed
                default:
                    _logger.LogWarning($"Unhandled event type: {stripeEvent.Type}");
                    break;
            }

            // Return a 200 OK response to Stripe
            return new EmptyResult();


        }



    }
}
