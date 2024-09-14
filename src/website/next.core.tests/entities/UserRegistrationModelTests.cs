using Bogus;
using next.core.entities;
using Newtonsoft.Json;

namespace next.core.tests.entities
{
    public class UserRegistrationModelTests
    {
        private static readonly Faker<UserRegistrationModel> faker
            = new Faker<UserRegistrationModel>()
            .RuleFor(x => x.UserName, y => y.Person.UserName)
            .RuleFor(x => x.Password, y => y.Random.AlphaNumeric(12))
            .RuleFor(x => x.Email, y => y.Person.Email);

        [Fact]
        public void ModelCanBeGenerated()
        {
            var original = new UserRegistrationModel();
            var test = faker.Generate();
            Assert.NotEqual(original.UserName, test.UserName);
            Assert.NotEqual(original.Password, test.Password);
            Assert.NotEqual(original.Email, test.Email);
        }
        [Fact]
        public void ModelConvertToApiModel()
        {
            var original = faker.Generate();
            var test = original.ToApiModel();
            var actual =
                JsonConvert.DeserializeObject<TempApiModel>(
                JsonConvert.SerializeObject(test));
            Assert.NotNull(actual);
            Assert.Equal(original.UserName, actual.UserName);
            Assert.Equal(original.Password, actual.Password);
            Assert.Equal(original.Email, actual.Email);
        }
        private class TempApiModel
        {
            public string UserName { get; set; } = string.Empty;

            public string Password { get; set; } = string.Empty;

            public string Email { get; set; } = string.Empty;
        }
    }
}