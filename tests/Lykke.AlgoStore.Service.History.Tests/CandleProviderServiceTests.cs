using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models.AlgoMetaDataModels;
using Lykke.AlgoStore.Service.History.Core.Domain;
using Lykke.AlgoStore.Service.History.Services;
using Lykke.Service.CandlesHistory.Client;
using Lykke.Service.CandlesHistory.Client.Models;
using Microsoft.Rest;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.Service.History.Tests
{
    [TestFixture]
    public class CandleProviderServiceTests
    {
        private AlgoClientInstanceData InstanceData => new AlgoClientInstanceData
        {
            AlgoMetaDataInformation = new AlgoMetaDataInformation
            {
                Functions = new List<AlgoMetaDataFunction>
                {
                    new AlgoMetaDataFunction
                    {
                        Parameters = new List<AlgoMetaDataParameter>
                        {
                            new AlgoMetaDataParameter { Key = "FunctionInstanceIdentifier", Value = "func" },
                            new AlgoMetaDataParameter { Key = "CandleTimeInterval", Value = "Sec"},
                            new AlgoMetaDataParameter { Key = "AssetPair", Value = "BTCUSD" },
                            new AlgoMetaDataParameter { Key = "StartingDate", Value= "2018-01-01T00:00:00Z" },
                            new AlgoMetaDataParameter { Key = "EndingDate", Value = "2018-02-01T00:00:00Z" }
                        }
                    }
                },
                Parameters = new List<AlgoMetaDataParameter>
                {
                    new AlgoMetaDataParameter { Key = "CandleInterval", Value = "Day" },
                    new AlgoMetaDataParameter { Key = "AssetPair", Value = "BTCUSD" },
                    new AlgoMetaDataParameter { Key = "StartFrom", Value = "2018-01-01T00:00:00Z" },
                    new AlgoMetaDataParameter { Key = "EndOn", Value = "2018-02-01T00:00:00Z" }
                }
            },
            AuthToken = "algo"
        };

        [Test]
        public void Ctor_ThrowsArgumentNull_WhenCandlesHistoryNull()
        {
            Assert.Throws<ArgumentNullException>(() => new CandleProviderService(null));
        }

        [Test]
        public async Task GetCandlesForPeriod_ThrowsArgumentNull_WhenRequestNull()
        {
            await AsyncMethodThrows<ArgumentNullException>(
                async (service) => await service.GetCandlesForPeriod(null, null, null));
        }

        [Test]
        public async Task GetCandlesForPeriod_ThrowsArgumentNull_WhenInstanceDataNull()
        {
            await AsyncMethodThrows<ArgumentNullException>(
                async (service) => await service.GetCandlesForPeriod(new CandleRequest(), null, null));
        }

        [Test]
        public async Task GetCandlesForPeriod_ThrowsArgumentNull_WhenErrorDictionaryNull()
        {
            await AsyncMethodThrows<ArgumentNullException>(
                async (service) => await service.GetCandlesForPeriod(new CandleRequest(), 
                                                                     new AlgoClientInstanceData(), 
                                                                     null));
        }

        [Test]
        public async Task GetCandlesForPeriod_HasErrors_WhenRequestStartDateAfterEndDate()
        {
            var request = new CandleRequest
            {
                StartFrom = new DateTime(2018, 1, 2, 0, 1, 0),
                EndOn = new DateTime(2018, 1, 2, 0, 0, 0),
                IndicatorName = "func"
            };

            await ValidateHasErrors_ForGivenRequest(request);
        }

        [Test]
        public async Task GetCandlesForPeriod_HasErrors_WhenIndicatorUnknown()
        {
            var candleProviderService = new CandleProviderService(Mock.Of<ICandleshistoryservice>());

            var request = new CandleRequest
            {
                StartFrom = new DateTime(2018, 1, 2, 0, 0, 0),
                EndOn = new DateTime(2018, 1, 2, 0, 1, 0),
                IndicatorName = "unknown"
            };

            var result = await candleProviderService
                .GetCandlesForPeriod(request, InstanceData, Mock.Of<IErrorDictionary>());

            Assert.IsNull(result);
        }

        [Test]
        public async Task GetCandlesForPeriod_HasErrors_WhenRequestStartDateBeforeParamStartDate()
        {
            var request = new CandleRequest
            {
                StartFrom = new DateTime(2017, 1, 2, 0, 1, 0),
                EndOn = new DateTime(2018, 1, 2, 0, 0, 0),
                IndicatorName = "func"
            };

            await ValidateHasErrors_ForGivenRequest(request);
        }

        [Test]
        public async Task GetCandlesForPeriod_HasErrors_WhenRequestEndDateBeforeParamStartDate()
        {
            var request = new CandleRequest
            {
                StartFrom = new DateTime(2018, 1, 2, 0, 0, 0),
                EndOn = new DateTime(2017, 1, 2, 0, 1, 0),
                IndicatorName = "func"
            };

            await ValidateHasErrors_ForGivenRequest(request);
        }

        [Test]
        public async Task GetCandlesForPeriod_HasErrors_WhenRequestStartDateAfterParamEndDate()
        {
            var request = new CandleRequest
            {
                StartFrom = new DateTime(2019, 1, 2, 0, 0, 0),
                EndOn = new DateTime(2018, 1, 2, 0, 1, 0),
                IndicatorName = "func"
            };

            await ValidateHasErrors_ForGivenRequest(request);
        }

        [Test]
        public async Task GetCandlesForPeriod_HasErrors_WhenRequestEndDateAfterParamEndDate()
        {
            var request = new CandleRequest
            {
                StartFrom = new DateTime(2018, 1, 2, 0, 0, 0),
                EndOn = new DateTime(2019, 1, 2, 0, 1, 0),
                IndicatorName = "func"
            };

            await ValidateHasErrors_ForGivenRequest(request);
        }

        [Test]
        public async Task GetCandlesForPeriod_ThrowsTaskCancelled_WhenResponseNotCompleted()
        {
            var candlesHistoryMock = Given_CandlesHistoryServiceMock(
                (assetPair, timeInterval, start, end) 
                    => Task.FromCanceled<HttpOperationResponse<object>>(new System.Threading.CancellationToken(true)));
            var errorDictionaryMock = Given_Verifiable_ErrorDictionaryMock();

            var candleProviderService = new CandleProviderService(candlesHistoryMock.Object);

            var request = new CandleRequest
            {
                StartFrom = new DateTime(2018, 1, 2, 0, 0, 0),
                EndOn = new DateTime(2018, 1, 2, 0, 1, 0),
                IndicatorName = "algo"
            };

            try
            {
                await candleProviderService.GetCandlesForPeriod(request, InstanceData, errorDictionaryMock.Object);
                Assert.Fail();
            }
            catch(TaskCanceledException)
            { }
        }

        [Test]
        public async Task GetCandlesForPeriod_ReturnsCandles_WhenEverythingOk()
        {
            var operationResponse = new HttpOperationResponse<object>
            {
                Body = new CandlesHistoryResponseModel
                {
                    History = new List<Candle>
                    {
                        new Candle()
                    }
                }
            };

            var candlesHistoryMock = Given_CandlesHistoryServiceMock(
                (assetPair, timeInterval, start, end) => Task.FromResult(operationResponse));
            var errorDictionaryMock = Given_Verifiable_ErrorDictionaryMock();

            var candleProviderService = new CandleProviderService(candlesHistoryMock.Object);

            var request = new CandleRequest
            {
                StartFrom = new DateTime(2018, 1, 2, 0, 0, 0),
                EndOn = new DateTime(2018, 1, 2, 0, 1, 0),
                IndicatorName = "algo"
            };

            var result = await candleProviderService
                .GetCandlesForPeriod(request, InstanceData, errorDictionaryMock.Object);

            Assert.AreEqual(1, result.Count());
        }

        private async Task ValidateHasErrors_ForGivenRequest(CandleRequest request)
        {
            var errorDictionaryMock = Given_Verifiable_ErrorDictionaryMock();
            var candleProviderService = new CandleProviderService(Mock.Of<ICandleshistoryservice>());

            await candleProviderService.GetCandlesForPeriod(request, InstanceData, errorDictionaryMock.Object);

            errorDictionaryMock.Verify();
        }

        private Mock<ICandleshistoryservice> Given_CandlesHistoryServiceMock(
            Func<string, CandleTimeInterval, DateTime, DateTime, Task<HttpOperationResponse<object>>> returnCallback)
        {
            var candlesHistoryServiceMock = new Mock<ICandleshistoryservice>(MockBehavior.Strict);

            candlesHistoryServiceMock.Setup(s => s.GetCandlesHistoryOrErrorWithHttpMessagesAsync(
                    It.IsNotNull<string>(), CandlePriceType.Mid, It.IsAny<CandleTimeInterval>(),
                    It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<Dictionary<string, List<String>>>(),
                    It.IsAny<System.Threading.CancellationToken>()))
                .Returns((string assetPair, CandlePriceType priceType, CandleTimeInterval timeInterval,
                          DateTime start, DateTime end, Dictionary<string, List<String>> headers,
                          System.Threading.CancellationToken cancellationToken) 
                          => returnCallback(assetPair, timeInterval, start, end))
                .Verifiable();

            return candlesHistoryServiceMock;
        }

        private Mock<IErrorDictionary> Given_Verifiable_ErrorDictionaryMock()
        {
            var hasError = false;
            var mock = new Mock<IErrorDictionary>(MockBehavior.Strict);

            mock.Setup(d => d.Add(It.IsNotNull<string>(), It.IsNotNull<string>()))
                .Callback(() => hasError = true)
                .Verifiable();

            mock.Setup(d => d.IsValid)
                .Returns(() => !hasError)
                .Verifiable();

            return mock;
        }

        private async Task AsyncMethodThrows<T>(
            Func<CandleProviderService, Task> action) 
            where T : Exception
        {
            var candleProviderService = new CandleProviderService(Mock.Of<ICandleshistoryservice>());

            try
            {
                await action(candleProviderService);
                Assert.Fail();
            }
            catch(T)
            {
            }
        }
    }
}
