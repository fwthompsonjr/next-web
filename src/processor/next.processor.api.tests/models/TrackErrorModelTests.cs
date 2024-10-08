﻿using Bogus;
using next.processor.api.extensions;
using next.processor.api.models;
using next.processor.api.utility;
using System.Text;

namespace next.processor.api.tests.models
{
    public class TrackErrorModelTests
    {

        private static readonly Faker<QueueReportIssueRequest> faker
            = new Faker<QueueReportIssueRequest>()
            .RuleFor(x => x.Id, y => y.Random.AlphaNumeric(20))
            .RuleFor(x => x.Message, y => y.Lorem.Sentence(5, 2))
            .RuleFor(x => x.Data, y =>
            {
                var data = y.Lorem.Paragraphs(4);
                return Encoding.UTF8.GetBytes(data);
            })
            .RuleFor(x => x.CreateDate, y => y.Date.Recent(120));

        [Fact]
        public void ModelCanBeCreated()
        {
            var model = faker.Generate(2);
            var a = model[0];
            var b = model[1];
            Assert.NotEqual(a.Id, b.Id);
            Assert.NotEqual(a.Message, b.Message);
            Assert.NotEqual(a.Data, b.Data);
            Assert.NotEqual(a.CreateDate, b.CreateDate);
        }

        [Fact]
        public void ModelCanBeLogged()
        {
            lock (locker)
            {
                try
                {
                    var errors = Record.Exception(() =>
                    {
                        const int nbr = 3;
                        const string logName = Constants.ErrorLogName;
                        var model = faker.Generate(nbr);
                        model.ForEach(m => m.Log());
                        _ = GetErrorModels(logName);
                    });
                    Assert.Null(errors);
                }
                finally
                {
                    TrackEventService.Clear();
                }
            }
        }

        [Fact]
        public void ModelCanBeLoggedAndUpdated()
        {
            lock (locker)
            {
                try
                {
                    const string logName = Constants.ErrorLogName;
                    var model = faker.Generate(3);
                    model.ForEach(m => m.Log());
                    var items = GetErrorModels(logName);
                    if (items == null) return;
                    Assert.NotEmpty(items);
                    var selection = items[0];
                    var now = DateTime.Now;
                    selection.Data.CreateDate = now;
                    TrackEventService.AppendItem(logName, selection, TimeSpan.FromMinutes(1));
                    items = GetErrorModels(logName);
                    if (items == null) return;
                    Assert.NotEmpty(items);
                    Assert.Equal(now, items[0].Data.CreateDate);
                }
                finally
                {
                    TrackEventService.Clear();
                }
            }
        }



        private static List<TrackErrorModel>? GetErrorModels(string logName)
        {
            var list = TrackEventService.Get(logName);
            if (list == null) return null;
            var items = list.ToInstance<List<TrackErrorModel>>();
            return items;
        }
        private static readonly object locker = new();
    }
}