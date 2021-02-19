using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NServiceBus;
using ReadingIsGood.API.Models.V1;
using ReadingIsGood.ProductService.Contracts.Messages;
using ReadingIsGood.Shared;

namespace ReadingIsGood.API.Controllers.V1
{
    /// <summary>
    ///     ProductController
    /// </summary>
    [Produces("application/json", "application/xml")]
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    public class ProductController : ControllerBase
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
        public ProductController(ILogger logger, IEndpointInstance endpointInstance)
        {
            _logger = logger;
            _endpointInstance = endpointInstance;
        }

        /// <summary>
        ///     Bir Ürüne Ait Stok Bilgisini Güncellemek için bu method kullanılır.
        /// </summary>
        /// <param name="updateStockRequestModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("UpdateStock")]
        [Consumes("application/json", "application/xml")]
        public async Task<ActionResult<CallbackResult<string>>> UpdateStock(
            [FromBody] UpdateStockRequestModel updateStockRequestModel)
        {
            try
            {
                var userId = HttpContext.User.Claims.First(t => t.Type == "UserId").Value;

                var sendOptions = new SendOptions();
                sendOptions.SetDestination(
                    ApplicationConfiguration.Instance.GetValue<string>("ProductService:CallbacksReceiverEndpointName"));
                sendOptions.RequireImmediateDispatch();

                var tokenSource = new CancellationTokenSource();
                tokenSource.CancelAfter(TimeSpan.FromSeconds(5));

                var result = await _endpointInstance.Request<CallbackResult<string>>(
                    new UpdateStockRequest(updateStockRequestModel.ProductId, Guid.Parse(userId),
                        updateStockRequestModel.Stock), sendOptions, tokenSource.Token).ConfigureAwait(false);

                return Ok(result);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "");
                return Ok(CallbackResult<string>.ErrorResult("", "Bilinmeyen bir hata oluştu"));
            }
        }

        /// <summary>
        ///     Ürün kaydetmek için bu method kullanılır.
        /// </summary>
        /// <param name="saveProductRequestModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("SaveProduct")]
        [Consumes("application/json", "application/xml")]
        public async Task<ActionResult<CallbackResult<string>>> SaveProduct(
            SaveProductRequestModel saveProductRequestModel)
        {
            try
            {
                var userId = HttpContext.User.Claims.First(t => t.Type == "UserId").Value;

                var sendOptions = new SendOptions();
                sendOptions.SetDestination(
                    ApplicationConfiguration.Instance.GetValue<string>("ProductService:CallbacksReceiverEndpointName"));
                sendOptions.RequireImmediateDispatch();

                var tokenSource = new CancellationTokenSource();
                tokenSource.CancelAfter(TimeSpan.FromSeconds(5));

                var result = await _endpointInstance.Request<CallbackResult<string>>(
                        new SaveProductRequest(saveProductRequestModel.Name, saveProductRequestModel.Stock,
                            saveProductRequestModel.Price, Guid.Parse(userId)), sendOptions, tokenSource.Token)
                    .ConfigureAwait(false);

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