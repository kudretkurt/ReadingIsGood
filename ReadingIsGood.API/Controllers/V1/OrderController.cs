using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NServiceBus;
using ReadingIsGood.API.Models;
using ReadingIsGood.API.Models.V1;
using ReadingIsGood.OrderService.Contracts.Messages;
using ReadingIsGood.ProductService.Contracts.Commands;
using ReadingIsGood.Shared;
using ReadingIsGood.Shared.CommonTypes;

namespace ReadingIsGood.API.Controllers.V1
{
    /// <summary>
    ///     OrderController
    /// </summary>
    [Produces("application/json", "application/xml")]
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    public class OrderController : ControllerBase
    {
        /// <summary>
        ///     EndpointInstance(SenderEndpointInstance)
        /// </summary>
        private readonly IEndpointInstance _endpointInstance;

        /// <summary>
        ///     Logger
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="endpointInstance"></param>
        public OrderController(ILogger logger, IEndpointInstance endpointInstance)
        {
            _logger = logger;
            _endpointInstance = endpointInstance;
        }


        /// <summary>
        /// Sipariş Kaydetmek için kullanılır
        /// </summary>
        /// <param name="saveOrderRequestModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("SaveOrder")]
        [Consumes("application/json", "application/xml")]
        public async Task<ActionResult<CallbackResult<string>>> SaveOrder(
            [FromBody] SaveOrderRequestModel saveOrderRequestModel)
        {
            try
            {
                var userId = HttpContext.User.Claims.First(t => t.Type == "UserId").Value;
                var userName = HttpContext.User.Claims.First(t => t.Type == "UserName").Value;

                var sendOptions = new SendOptions();
                sendOptions.SetDestination(
                    ApplicationConfiguration.Instance.GetValue<string>("ProductService:EndpointName"));
                sendOptions.RequireImmediateDispatch();

                var tokenSource = new CancellationTokenSource();
                tokenSource.CancelAfter(TimeSpan.FromSeconds(5));

                var products = saveOrderRequestModel.Products.Select(t => new ProductInformationsInOrder
                {
                    Id = t.Id,
                    Count = t.Count,
                    Name = t.Name,
                    Price = t.Price
                }).ToList();

                await _endpointInstance.Send(
                    new OrderControlCommand(Guid.Parse(userId), userName, saveOrderRequestModel.DeliveryAddress,
                        saveOrderRequestModel.InvoiceAddress, products), sendOptions);

                return Ok(CallbackResult<string>.SuccessResult("Sipariş Talebiniz Alınmıştır. Teşekkür ederiz."));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "");
                return Ok(CallbackResult<string>.ErrorResult("", "Bilinmeyen bir hata oluştu"));
            }
        }


        /// <summary>
        /// Müşteriye ait siparişleri(sipariş detayları ile birlikte) listelemek için kullanılır.
        /// </summary>
        /// <param name="paginationModel"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetOrders")]
        [Consumes("application/json", "application/xml")]
        public async Task<ActionResult<CallbackResult<List<OrderCallbackResult>>>> GetOrders(
            [FromQuery] PaginationModel paginationModel)
        {
            try
            {
                var userId = HttpContext.User.Claims.First(t => t.Type == "UserId").Value;

                var sendOptions = new SendOptions();
                sendOptions.SetDestination(
                    ApplicationConfiguration.Instance.GetValue<string>("OrderService:CallbacksReceiverEndpointName"));
                sendOptions.RequireImmediateDispatch();

                var tokenSource = new CancellationTokenSource();
                tokenSource.CancelAfter(TimeSpan.FromSeconds(5));

                var result = await _endpointInstance.Request<CallbackResult<List<OrderCallbackResult>>>(
                    new OrderViewRequest(Guid.Parse(userId), paginationModel.PageSize, paginationModel.PageNumber),
                    sendOptions, tokenSource.Token).ConfigureAwait(false);

                return Ok(result);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "");
                return Ok(CallbackResult<string>.ErrorResult("", "Bilinmeyen bir hata oluştu"));
            }
        }
    }
}