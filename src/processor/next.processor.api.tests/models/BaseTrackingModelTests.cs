using Bogus;
using next.processor.api.interfaces;
using next.processor.api.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace next.processor.api.tests.models
{
    public class BaseTrackingModelTests
    {
        private static readonly Faker<TestModel> faker
            = new Faker<TestModel>()
            .RuleFor(x => x.ExpirationDate, y => y.Date.Recent(120));

        [Fact]
        public void ModelShouldInheritItrackable()
        {
            var sut = faker.Generate();
            Assert.IsAssignableFrom<ITrackable>(sut);
        }


        [Fact]
        public void ModelShouldHaveUniqueId()
        {
            var test = faker.Generate(2);
            var a = test[0];
            var b = test[1];
            Assert.NotEqual(a.Id, b.Id);
        }



        [Fact]
        public void ModelShouldHaveEspirationDate()
        {
            var test = faker.Generate(2);
            var a = test[0];
            var b = test[1];
            Assert.NotEqual(a.ExpirationDate, b.ExpirationDate);
        }
        private sealed class TestModel : BaseTrackingModel { }
    }
}
