﻿using Bogus;
using next.core.entities;

namespace next.core.tests.entities
{
    public class MyActiveSearchStatusTests
    {
        private static readonly Faker<MyActiveSearchStatus> faker
            = new Faker<MyActiveSearchStatus>()
            .RuleFor(x => x.Id, y => y.Random.String(5, 500))
            .RuleFor(x => x.Line, y => y.Random.String(5, 500))
            .RuleFor(x => x.LineNbr, y => y.Random.Int(5, 500000))
            .RuleFor(x => x.CreateDate, y => y.Random.String(5, 500))
            .RuleFor(x => x.SearchId, y => y.Random.String(5, 500));

        [Fact]
        public void ModelCanBeGenerated()
        {
            var original = new MyActiveSearchStatus();
            var test = faker.Generate();
            Assert.NotEqual(original.Id, test.Id);
            Assert.NotEqual(original.Line, test.Line);
            Assert.NotEqual(original.LineNbr, test.LineNbr);
            Assert.NotEqual(original.CreateDate, test.CreateDate);
            Assert.NotEqual(original.SearchId, test.SearchId);
        }
    }
}