using AutoMapper;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;
using Lykke.AlgoStore.Service.History.Controllers;
using Lykke.AlgoStore.Service.History.Core.Domain;
using Lykke.AlgoStore.Service.History.Core.Services;
using Lykke.AlgoStore.Service.History.Models;
using Lykke.Service.CandlesHistory.Client.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.Service.History.Tests
{
    [TestFixture]
    public class CandlesControllerTests
    {
        [SetUp]
        public void MapperSetup()
        {
            Mapper.Reset();
            // Use startup logic to init mapper
            Startup.InitMapper();
        }

        [Test]
        public void Ctor_ThrowsArgumentNull_WhenCandlesProviderNull()
        {
            Assert.Throws<ArgumentNullException>(() => new CandlesController(null));
        }

        [Test]
        public async Task GetCandlesForPeriod_ReturnsBadRequest_WhenModelStateInvalid()
        {
            var candlesController = new CandlesController(Mock.Of<ICandleProviderService>());
            var model = new HistoryRequestModel();
            candlesController.ModelState.AddModelError("test", "test");

            var result = await candlesController.GetCandlesForPeriod(model);

            Then_Result_BadRequestWithErrors(result);
        }

        [Test]
        public async Task GetCandlesForPeriod_ReturnsBadRequest_WhenServiceFailsValidations()
        {
            var candleProviderServiceMock = new Mock<ICandleProviderService>(MockBehavior.Strict);
            candleProviderServiceMock
                .Setup(c => c.GetCandlesForPeriod(
                    It.IsNotNull<CandleRequest>(),
                    It.IsAny<AlgoClientInstanceData>(),
                    It.IsNotNull<IErrorDictionary>()))
                .ReturnsAsync((CandleRequest cr, AlgoClientInstanceData id, IErrorDictionary errorDictionary) =>
                {
                    errorDictionary.Add("test", "test");
                    return null;
                })
                .Verifiable();

            var candlesController = new CandlesController(candleProviderServiceMock.Object);
            var model = new HistoryRequestModel();

            var result = await candlesController.GetCandlesForPeriod(model);

            Then_Result_BadRequestWithErrors(result);
        }

        [Test]
        public async Task GetCandlesForPeriod_ReturnsOk_WhenNoErrors()
        {
            var candleToReturn = new Candle();

            var candleProviderServiceMock = new Mock<ICandleProviderService>(MockBehavior.Strict);
            candleProviderServiceMock
                .Setup(c => c.GetCandlesForPeriod(
                    It.IsNotNull<CandleRequest>(),
                    It.IsAny<AlgoClientInstanceData>(),
                    It.IsNotNull<IErrorDictionary>()))
                .ReturnsAsync(new Candle[] { candleToReturn })
                .Verifiable();

            var candlesController = new CandlesController(candleProviderServiceMock.Object);
            var model = new HistoryRequestModel();

            var result = await candlesController.GetCandlesForPeriod(model);

            Then_Result_OkWithCandle(result, candleToReturn);
        }

        [Test]
        public async Task GetCandlesForPeriod_ReturnsServiceUnavailable_WhenTaskCanceled()
        {
            var candleProviderServiceMock = new Mock<ICandleProviderService>(MockBehavior.Strict);
            candleProviderServiceMock
                .Setup(c => c.GetCandlesForPeriod(
                    It.IsNotNull<CandleRequest>(),
                    It.IsAny<AlgoClientInstanceData>(),
                    It.IsNotNull<IErrorDictionary>()))
                .ThrowsAsync(new TaskCanceledException());

            var candlesController = new CandlesController(candleProviderServiceMock.Object);

            candlesController.ControllerContext = 
                Mock.Of<ControllerContext>(
                    (c) => c.HttpContext == Mock.Of<HttpContext>(
                        (hc) => hc.Response == Mock.Of<HttpResponse>(
                            (hr) => hr.Headers == Mock.Of<IHeaderDictionary>())));

            var model = new HistoryRequestModel();

            var result = await candlesController.GetCandlesForPeriod(model);

            Then_Result_ServiceUnavailable(result);
        }

        private void Then_Result_BadRequestWithErrors(IActionResult result)
        {
            Assert.IsTrue(result is ObjectResult);

            var objectResult = result as ObjectResult;

            Assert.AreEqual(400, objectResult.StatusCode);
            Assert.IsTrue(objectResult.Value is ErrorResponseModel);

            var errors = objectResult.Value as ErrorResponseModel;

            Assert.AreEqual(1, errors.Errors.Count());
            Assert.AreEqual("test", errors.Errors.First());
        }

        private void Then_Result_OkWithCandle(IActionResult result, Candle candle)
        {
            Assert.IsTrue(result is OkObjectResult);

            var objectResult = result as OkObjectResult;

            Assert.AreEqual(200, objectResult.StatusCode);
            Assert.IsTrue(objectResult.Value is IEnumerable<CandleModel>);

            var candles = objectResult.Value as IEnumerable<CandleModel>;

            Assert.AreEqual(1, candles.Count());
        }

        private void Then_Result_ServiceUnavailable(IActionResult result)
        {
            Assert.IsTrue(result is StatusCodeResult);

            var statusCodeResult = result as StatusCodeResult;

            Assert.AreEqual(503, statusCodeResult.StatusCode);
        }
    }
}
