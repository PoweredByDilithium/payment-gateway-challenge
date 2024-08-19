using System.Net;
using Microsoft.AspNetCore.Mvc;
using PaymentGateway.Api.BLL;
using PaymentGateway.Api.Entities;

namespace PaymentGateway.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiExplorerSettings(IgnoreApi = false)]
    public sealed class PaymentGatewayController(IPaymentGatewayManager paymentGatewayManager)
        : Controller
    {
        private readonly IPaymentGatewayManager _paymentGatewayManager = paymentGatewayManager;

        /// <summary>
        /// Processes a payment for a merchant. Assumes an unauthenticated endpoint being called from the merchant
        /// </summary>
        /// <returns>PaymentResponse entity.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(PaymentResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> ProcessPayment([FromBody] PaymentRequest paymentRequest)
        {
            try
            {
                return Ok(await _paymentGatewayManager.ProcessPaymentAsync(paymentRequest));
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}