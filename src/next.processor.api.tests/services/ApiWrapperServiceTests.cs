﻿using Bogus;
using Moq;
using next.processor.api.extensions;
using next.processor.api.models;
using System.Net.Http.Formatting;
using System.Text.Json;

namespace next.processor.api.tests.services
{
    public class ApiWrapperServiceTests
    {

        [Fact]
        public void ApiCanGetWrapper()
        {
            var error = Record.Exception(() =>
            {
                var service = new MockApiWrapperService();
                _ = service.GetFakeWrapper();
            });
            Assert.Null(error);
        }

        [Theory]
        [InlineData(200, 20)]
        [InlineData(400, 20)]
        [InlineData(200, 20, 400)]
        [InlineData(200, 20, 401)]
        [InlineData(200, 20, 200, 0)]
        [InlineData(200, 20, 200, 1)]
        [InlineData(200, 20, 200, 2)]
        public async Task ApiCanFetchAsync(int statusCode, int recordCount, int httpCode = 200, int messageId = 10)
        {
            var data = recordfaker.Generate(recordCount);
            data.ForEach(d => d.Payload = searchrequestfaker.Generate().ToJsonString());
            var error = await Record.ExceptionAsync(async () =>
            {
                var service = new MockApiWrapperService();
                var mock = service.MockClient;
                var json = messageId switch
                {
                    0 => null,
                    1 => string.Empty,
                    2 => "    ",
                    _ => data.ToJsonString()
                };
                var message = GetMockResponse(httpCode, statusCode, json);
                mock.Setup(m => m.PostAsJsonAsync<object?>(
                    It.IsAny<HttpClient>(),
                    It.IsAny<string>(),
                    It.IsAny<object?>(),
                    It.IsAny<JsonSerializerOptions>(),
                    It.IsAny<CancellationToken>())).ReturnsAsync(message);
                _ = await service.FetchAsync();
            });
            Assert.Null(error);
        }

        [Theory]
        [InlineData(200)]
        [InlineData(400)]
        [InlineData(200, 400)]
        [InlineData(200, 401)]
        [InlineData(200, 200, 0)]
        [InlineData(200, 200, 1)]
        [InlineData(200, 200, 2)]
        public async Task ApiCanStartAsync(int statusCode, int httpCode = 200, int messageId = 10)
        {
            var data = recordfaker.Generate();
            data.Payload = searchrequestfaker.Generate().ToJsonString();
            var error = await Record.ExceptionAsync(async () =>
            {
                var service = new MockApiWrapperService();
                var mock = service.MockClient;
                var json = messageId switch
                {
                    0 => null,
                    1 => string.Empty,
                    2 => "    ",
                    _ => data.ToJsonString()
                };
                var message = GetMockResponse(httpCode, statusCode, json);
                mock.Setup(m => m.PostAsJsonAsync<object?>(
                    It.IsAny<HttpClient>(),
                    It.IsAny<string>(),
                    It.IsAny<object?>(),
                    It.IsAny<JsonSerializerOptions>(),
                    It.IsAny<CancellationToken>())).ReturnsAsync(message);
                await service.StartAsync(data);
            });
            Assert.Null(error);
        }

        [Theory]
        [InlineData(200)]
        [InlineData(400)]
        [InlineData(200, 400)]
        [InlineData(200, 401)]
        [InlineData(200, 200, 0)]
        [InlineData(200, 200, 1)]
        [InlineData(200, 200, 2)]
        [InlineData(200, 200, 15, -1)]
        [InlineData(200, 200, 15, 0)]
        [InlineData(200, 200, 15, 1)]
        [InlineData(200, 200, 15, 2)]
        [InlineData(200, 200, 15, 3)]
        [InlineData(200, 200, 15, 4)]
        [InlineData(200, 200, 15, 5)]
        [InlineData(200, 200, 15, 6)]
        [InlineData(200, 200, 15, 7)]
        [InlineData(200, 200, 20, 1, -1)]
        [InlineData(200, 200, 20, 1, 0)]
        [InlineData(200, 200, 20, 1, 1)]
        [InlineData(200, 200, 20, 1, 2)]
        [InlineData(200, 200, 20, 1, 5)]
        public async Task ApiCanPostStatusAsync(
            int statusCode,
            int httpCode = 200,
            int payloadId = 10,
            int messageId = 0,
            int statusId = 1)
        {
            var data = recordfaker.Generate();
            data.Payload = searchrequestfaker.Generate().ToJsonString();
            var error = await Record.ExceptionAsync(async () =>
            {
                var service = new MockApiWrapperService();
                var mock = service.MockClient;
                var json = payloadId switch
                {
                    0 => null,
                    1 => string.Empty,
                    2 => "    ",
                    _ => data.ToJsonString()
                };
                var message = GetMockResponse(httpCode, statusCode, json);
                mock.Setup(m => m.PostAsJsonAsync<object?>(
                    It.IsAny<HttpClient>(),
                    It.IsAny<string>(),
                    It.IsAny<object?>(),
                    It.IsAny<JsonSerializerOptions>(),
                    It.IsAny<CancellationToken>())).ReturnsAsync(message);
                await service.PostStatusAsync(data, messageId, statusId);
            });
            Assert.Null(error);
        }


        [Theory]
        [InlineData(200)]
        [InlineData(400)]
        [InlineData(200, 400)]
        [InlineData(200, 401)]
        [InlineData(200, 200, 0)]
        [InlineData(200, 200, 1)]
        [InlineData(200, 200, 2)]
        [InlineData(200, 200, 15, -1)]
        [InlineData(200, 200, 15, 0)]
        [InlineData(200, 200, 15, 1)]
        [InlineData(200, 200, 15, 2)]
        [InlineData(200, 200, 15, 3)]
        [InlineData(200, 200, 15, 4)]
        [InlineData(200, 200, 15, 5)]
        [InlineData(200, 200, 15, 6)]
        [InlineData(200, 200, 15, 7)]
        [InlineData(200, 200, 20, 1, -1)]
        [InlineData(200, 200, 20, 1, 0)]
        [InlineData(200, 200, 20, 1, 1)]
        [InlineData(200, 200, 20, 1, 2)]
        [InlineData(200, 200, 20, 1, 5)]
        public async Task ApiCanPostStepCompletionAsync(
            int statusCode,
            int httpCode = 200,
            int payloadId = 10,
            int messageId = 0,
            int statusId = 1)
        {
            var data = recordfaker.Generate();
            data.Payload = searchrequestfaker.Generate().ToJsonString();
            var error = await Record.ExceptionAsync(async () =>
            {
                var service = new MockApiWrapperService();
                var mock = service.MockClient;
                var json = payloadId switch
                {
                    0 => null,
                    1 => string.Empty,
                    2 => "    ",
                    _ => data.ToJsonString()
                };
                var message = GetMockResponse(httpCode, statusCode, json);
                mock.Setup(m => m.PostAsJsonAsync<object?>(
                    It.IsAny<HttpClient>(),
                    It.IsAny<string>(),
                    It.IsAny<object?>(),
                    It.IsAny<JsonSerializerOptions>(),
                    It.IsAny<CancellationToken>())).ReturnsAsync(message);
                await service.PostStepCompletionAsync(data, messageId, statusId);
            });
            Assert.Null(error);
        }

        [Theory]
        [InlineData(200)]
        [InlineData(400)]
        [InlineData(200, 400)]
        [InlineData(200, 401)]
        [InlineData(200, 200, 0)]
        [InlineData(200, 200, 1)]
        [InlineData(200, 200, 2)]
        public async Task ApiCanPostSaveContentAsync(int statusCode, int httpCode = 200, int messageId = 10)
        {
            var data = persistencefaker.Generate();
            var payload = recordfaker.Generate();
            payload.Payload = searchrequestfaker.Generate().ToJsonString();
            var error = await Record.ExceptionAsync(async () =>
            {
                Assert.NotNull(data.Content);
                var service = new MockApiWrapperService();
                var mock = service.MockClient;
                payload.Id = messageId switch
                {
                    0 => null,
                    1 => string.Empty,
                    _ => data.Id
                };
                var json = messageId switch
                {
                    0 => null,
                    1 => string.Empty,
                    2 => "    ",
                    _ => data.ToJsonString()
                };
                var message = GetMockResponse(httpCode, statusCode, json);
                mock.Setup(m => m.PostAsJsonAsync<object?>(
                    It.IsAny<HttpClient>(),
                    It.IsAny<string>(),
                    It.IsAny<object?>(),
                    It.IsAny<JsonSerializerOptions>(),
                    It.IsAny<CancellationToken>())).ReturnsAsync(message);
                await service.PostSaveContentAsync(payload, data.Content);
            });
            Assert.Null(error);
        }

        private static HttpResponseMessage GetMockResponse(int httpCode, int statusCode, string? json)
        {
            var response = new { StatusCode = statusCode, Message = json };
            var code = httpCode switch
            {
                400 => System.Net.HttpStatusCode.BadRequest,
                401 => System.Net.HttpStatusCode.Unauthorized,
                _ => System.Net.HttpStatusCode.OK
            };
            return new(code)
            {
                Content = new StringContent(response.ToJsonString())
            };
        }

        private static readonly Faker<QueuePersistenceRequest> persistencefaker =
            new Faker<QueuePersistenceRequest>()
            .RuleFor(x => x.Id, y => y.Random.AlphaNumeric(16))
            .RuleFor(x => x.Content, y => {
                var content = y.Lorem.Sentence(5);
                return System.Text.Encoding.UTF8.GetBytes(content);
            });

        private static readonly Faker<QueueSearchItem> searchrequestfaker =
            new Faker<QueueSearchItem>()
            .RuleFor(x => x.WebId, y => y.Random.Int(1, 500000))
            .RuleFor(x => x.State, y => y.Random.AlphaNumeric(35))
            .RuleFor(x => x.County, y => y.Random.AlphaNumeric(50))
            .RuleFor(x => x.StartDate, y => y.Date.Recent(30).ToString())
            .RuleFor(x => x.EndDate, y => y.Date.Recent(90).ToString());

        private static readonly Faker<QueuedRecord> recordfaker =
            new Faker<QueuedRecord>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString())
            .RuleFor(x => x.Name, y => y.Random.Guid().ToString())
            .RuleFor(x => x.UserId, y => y.Random.Guid().ToString())
            .RuleFor(x => x.StartDate, y => y.Date.Recent(30))
            .RuleFor(x => x.EndDate, y => y.Date.Recent(60))
            .RuleFor(x => x.ExpectedRows, y => y.Random.Int(0, 50000))
            .RuleFor(x => x.CreateDate, y => y.Date.Recent(60))
            .RuleFor(x => x.Payload, y => y.Lorem.Sentence(2));
    }
}
