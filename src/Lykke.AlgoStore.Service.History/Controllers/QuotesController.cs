using Common.Log;
using Lykke.AlgoStore.Algo.Charting;
using Lykke.AlgoStore.Service.History.Core.Services;
using Lykke.AlgoStore.Service.History.Models;
using Lykke.AlgoStore.Service.History.Utils;
using Lykke.Common.Log;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.Service.History.Controllers
{
    public class QuotesController : Controller
    {
        private IQuoteChartingUpdateService _quotesService;
        private ILog _log;

        public QuotesController(IQuoteChartingUpdateService quotesService, ILogFactory logFactory)
        {
            _quotesService = quotesService ?? throw new ArgumentNullException(nameof(quotesService));
            _log = logFactory.CreateLog(this);
        }

        [HttpGet("")]
        [SwaggerOperation("GetQuotesForPeriod")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IEnumerable<QuoteChartingUpdate>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ErrorResponseModel))]
        [ProducesResponseType((int)HttpStatusCode.TooManyRequests)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> GetQuotesForPeriod([Required]string instanceId, string asetPair,  DateTime from, DateTime to, bool? isBuy = null)
        {
            _log.Info(nameof(GetQuotesForPeriod), $"Request for instanceId {instanceId}, from {from}, to: {to}", context: nameof(GetQuotesForPeriod));

            if (!ModelState.IsValid)
            {
                return BadRequest(ErrorResponseModel.CreateFromModelState(ModelState));
            }

            try
            {
                var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
                var functions = await _quotesService.GetQuoteChartingUpdateForPeriodAsync(from, to, instanceId, asetPair, new ModelStateWrapper(ModelState), cts.Token, isBuy);

                if (!ModelState.IsValid)
                    return BadRequest(ErrorResponseModel.CreateFromModelState(ModelState));

                return Ok(functions);
            }
            catch (TaskCanceledException)
            {
                var errorMsg = "Couldn't complete the request withing the time allowed, possibly due to high number of quotes found.";
                await _log.WriteWarningAsync(nameof(GetQuotesForPeriod), instanceId, $"Timout. Request for Get quotes from {from} to {to}. {errorMsg} ");
                ModelState.AddModelError("RequestTimeOut", errorMsg);
                return BadRequest(ErrorResponseModel.CreateFromModelState(ModelState));
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
