﻿using Bogus;
using next.core.entities;

namespace next.core.tests.entities
{
    public class MySearchItemTests
    {
        private static readonly Faker<MySearchItem> faker
            = new Faker<MySearchItem>()
            .RuleFor(x => x.Id, y => y.Random.String(5, 500))
            .RuleFor(x => x.EndDate, y => y.Random.String(5, 500))
            .RuleFor(x => x.StartDate, y => y.Random.String(5, 500))
            .RuleFor(x => x.CreateDate, y => y.Random.String(5, 500))
            .RuleFor(x => x.ExpectedRows, y => y.Random.Int(5, 500));

        [Fact]
        public void ModelCanBeGenerated()
        {
            var original = new MySearchItem();
            var test = faker.Generate();
            Assert.NotEqual(original.Id, test.Id);
            Assert.NotEqual(original.EndDate, test.EndDate);
            Assert.NotEqual(original.StartDate, test.StartDate);
            Assert.NotEqual(original.CreateDate, test.CreateDate);
            Assert.NotEqual(original.ExpectedRows, test.ExpectedRows);
        }
    }
}