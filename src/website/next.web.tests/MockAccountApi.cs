using Bogus;
using next.core.entities;
using next.core.interfaces;
using System.Text;

namespace next.web.tests
{
    internal sealed class MockAccountApi(int statusCode) : IPermissionApi
    {
        private readonly int _statusCode = statusCode;


        public IInternetStatus? InternetUtility => throw new NotImplementedException();

        public KeyValuePair<bool, ApiResponse> CanGet(string name)
        {
            return new(true, new());
        }

        public KeyValuePair<bool, ApiResponse> CanPost(string name, object payload, UserBo user)
        {
            return new(true, new());
        }

        public ApiResponse CheckAddress(string name)
        {
            throw new();
        }

        public Task<ApiResponse> Get(string name, UserBo user)
        {
            return Task.FromResult(GenericHtmlResponse(_statusCode));
        }

        public Task<ApiResponse> Get(string name)
        {
            return Task.FromResult(GenericHtmlResponse(_statusCode));
        }

        public Task<ApiResponse> Get(string name, Dictionary<string, string> parameters)
        {
            return Task.FromResult(GenericHtmlResponse(_statusCode));
        }

        public Task<ApiResponse> Get(string name, UserBo user, Dictionary<string, string> parameters)
        {
            return Task.FromResult(GenericHtmlResponse(_statusCode));
        }

        public Task<ApiResponse> Post(string name, object payload, UserBo user)
        {
            var response = new ApiResponse { StatusCode = _statusCode };
            var message = GetResponse(name);
            response.Message = message;
            return Task.FromResult(response);
        }


        public static string GetResponse(string name)
        {
            var manager = Properties.Resources.ResourceManager;
            var find = string.Concat("response_", name.Replace("-", "_"));
            var content = manager.GetString(find) ?? string.Empty;
            return content;
        }

        public static string GetPayload(string name)
        {
            var manager = Properties.Resources.ResourceManager;
            var find = string.Concat("request_", name.Replace("-", "_"));
            var content = manager.GetString(find) ?? string.Empty;
            var faker = new Faker();
            var keys = new Dictionary<string, string>()
            {
                { "{{$PersonalEmail}}", faker.Person.Email },
                { "{{$BusinessEmail}}", faker.Person.Email },
                { "{{$OtherEmail}}", faker.Person.Email },
                { "{{DefaultUserName}}", faker.Person.UserName },
                { "{{DefaultPassword}}", faker.Random.AlphaNumeric(20) },
                { "{{$randomStreetAddress}}", faker.Person.Address.Street },
                { "{{$randomCity}}", faker.Person.Address.City },
                { "{{$randomCountry}}", faker.Person.Address.State },
                { "{{$randomInt}}", faker.Person.Address.ZipCode },
                { "{{$randomFirstName}}", faker.Person.FirstName },
                { "{{$randomLastName}}", faker.Person.LastName },
                { "{{$randomCompanyName}}", faker.Company.CompanyName() },
                { "{{$randomPhoneNumber1}}", faker.Person.Phone },
                { "{{$randomPhoneNumber2}}", faker.Person.Phone },
                { "{{$randomPhoneNumber3}}", faker.Person.Phone },
            };
            var builder = new StringBuilder(content);
            keys.Keys.ToList().ForEach(key =>
            {
                var replacement = keys[key];
                builder.Replace(key, replacement);
            });
            return builder.ToString();
        }

        private static ApiResponse GenericHtmlResponse(int status = 200)
        {
            return new()
            {
                StatusCode = status,
                Message = "<html><body></body></html>"
            };
        }
    }

}
