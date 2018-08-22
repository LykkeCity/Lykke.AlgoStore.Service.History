using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Common.Log;
using Lykke.AlgoStore.Algo.Charting;
using Lykke.AlgoStore.Security.InstanceAuth;
using Lykke.AlgoStore.Service.History.Core.Services;
using Lykke.AlgoStore.Service.History.Models;
using Lykke.AlgoStore.Service.History.Utils;
using Lykke.Common.Log;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Lykke.AlgoStore.Service.History.Controllers
{
    [Authorize]
    [RateLimit]
    [Route("api/v1/functions")]
    public class FunctionsController :  Controller
    {
        private IFunctionChartingUpdateService _functionsService;
        private ILog _log;

        public FunctionsController(IFunctionChartingUpdateService functionsService, ILogFactory logFactory)
        {
            _functionsService = functionsService ?? throw new ArgumentNullException(nameof(functionsService));
            _log = logFactory.CreateLog(this);
        }

        [HttpGet("")]
        [SwaggerOperation("GetFunctionForPeriod")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IEnumerable<FunctionChartingUpdate>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ErrorResponseModel))]
        [ProducesResponseType((int)HttpStatusCode.TooManyRequests)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> GetFunctionsForPeriod([Required]string instanceId, DateTime from, DateTime to)
        {
            _log.Info(nameof(GetFunctionsForPeriod), $"Request for instanceId {instanceId}, from {from}, to: {to}", context: nameof(GetFunctionsForPeriod));

            if (!ModelState.IsValid)
            {
                return BadRequest(ErrorResponseModel.CreateFromModelState(ModelState));
            }

            try
            {
                var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
                var functions = await _functionsService.GetFunctionChartingUpdateForPeriodAsync(instanceId,from,to, new ModelStateWrapper(ModelState), cts.Token);

                if (!ModelState.IsValid)
                    return BadRequest(ErrorResponseModel.CreateFromModelState(ModelState));

                return Ok(functions);
            }
            catch (TaskCanceledException)
            {
                var errorMsg = "Couldn't complete the request withing the time allowed, possibly due to high number of functions found.";
                await _log.WriteWarningAsync(nameof(GetFunctionsForPeriod), instanceId, $"Timout. Request for Get functions from {from} to {to}. {errorMsg} ");
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
