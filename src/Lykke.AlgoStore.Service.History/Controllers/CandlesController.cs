using AutoMapper;
using Lykke.AlgoStore.Security.InstanceAuth;
using Lykke.AlgoStore.Service.History.Core.Domain;
using Lykke.AlgoStore.Service.History.Core.Services;
using Lykke.AlgoStore.Service.History.Models;
using Lykke.AlgoStore.Service.History.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.Service.History.Controllers
{
    [Authorize]
    [RateLimit]
    [Route("api/v1/candles")]
    public class CandlesController : Controller
    {
        private ICandleProviderService _candleService;

        public CandlesController(ICandleProviderService candleService)
        {
            _candleService = candleService ?? throw new ArgumentNullException(nameof(candleService));
        }

        [HttpGet("")]
        [SwaggerOperation("GetCandles")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IEnumerable<CandleModel>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ErrorResponseModel))]
        [ProducesResponseType((int)HttpStatusCode.ServiceUnavailable)]
        public async Task<IActionResult> GetCandlesForPeriod(HistoryRequestModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelStateToErrorResponseModel());

            var candleRequest = Mapper.Map<CandleRequest>(model);

            try
            {
                var candles = await _candleService.GetCandlesForPeriod(
                    candleRequest,
                    User.GetInstanceData(),
                    new ModelStateWrapper(ModelState));

                if (!ModelState.IsValid)
                    return BadRequest(ModelStateToErrorResponseModel());

                return Ok(candles.Select(Mapper.Map<CandleModel>));
            }
            catch(TaskCanceledException)
            {
                return StatusCode((int)HttpStatusCode.ServiceUnavailable);
            }
        }

        private ErrorResponseModel ModelStateToErrorResponseModel()
        {
            return new ErrorResponseModel
            {
                Errors = ModelState.SelectMany(e => e.Value.Errors).Select(e => e.ErrorMessage)
            };
        }
    }
}
