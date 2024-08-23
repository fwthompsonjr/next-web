using Bogus;
using legallead.records.search.Models;
using next.processor.api.extensions;
using next.processor.api.models;

namespace next.processor.api.tests
{
    internal static class MockObjProvider
    {

        public static string GetUserSearchPayload()
        {
            var fkr = new Faker();
            List<string> collection = [CollinSettings, DentonSettings, HarrisSettings, TarrantSettings];
            return fkr.PickRandom(collection);
        }

        public static QueueProcessResponses GetQueueResponse(int queueSize)
        {
            var collection = RecordFaker.Generate(queueSize);
            return new QueueProcessResponses(collection);
        }

        public static WebFetchResult GetWebFetchResult(int size)
        {
            var fkr = new Faker();
            List<int> counties = [0, 10, 20, 30];
            var collection = PersonFaker.Generate(size);
            return new WebFetchResult
            {
                WebsiteId = fkr.PickRandom(counties),
                CaseList = collection.ToJsonString(),
                PeopleList = collection,
                Result = fkr.System.FilePath()
            };
        }

        public static readonly Faker<PersonAddress> PersonFaker =
            new Faker<PersonAddress>()
            .RuleFor(x => x.Name, y => y.Person.FullName)
            .RuleFor(x => x.Zip, y => y.Person.Address.ZipCode)
            .RuleFor(x => x.Address1, y => y.Person.Address.Street)
            .RuleFor(x => x.Address2, y => {
                if (y.Random.Int(0, 100) < 75) return string.Empty;
                return y.Person.Address.Suite;
            })
            .RuleFor(x => x.Address3, y => $"{y.Address.City}, TX")
            .RuleFor(x => x.CaseNumber, y => y.Random.AlphaNumeric(10))
            .RuleFor(x => x.DateFiled, y => y.Random.Guid().ToString())
            .RuleFor(x => x.Court, y => y.Random.Int(100, 500).ToString())
            .RuleFor(x => x.CaseType, y => y.Random.Guid().ToString())
            .RuleFor(x => x.CaseStyle, y => y.Random.Guid().ToString())
            .RuleFor(x => x.FirstName, y => y.Person.FirstName)
            .RuleFor(x => x.LastName, y => y.Person.LastName)
            .RuleFor(x => x.Plantiff, y => y.Hacker.Phrase())
            .RuleFor(x => x.Status, y => y.Random.Int(1, 10).ToString());

        public static readonly Faker<QueueSearchItem> ItemFaker =
            new Faker<QueueSearchItem>()
            .RuleFor(x => x.WebId, y => y.Random.Int(1, 500000))
            .RuleFor(x => x.State, y => y.Random.AlphaNumeric(35))
            .RuleFor(x => x.County, y => y.Random.AlphaNumeric(50))
            .RuleFor(x => x.StartDate, y => y.Date.Recent(30).ToString())
            .RuleFor(x => x.EndDate, y => y.Date.Recent(90).ToString());

        public static readonly Faker<QueuedRecord> RecordFaker =
            new Faker<QueuedRecord>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString())
            .RuleFor(x => x.Name, y => y.Random.Guid().ToString())
            .RuleFor(x => x.UserId, y => y.Random.Guid().ToString())
            .RuleFor(x => x.StartDate, y => y.Date.Recent(30))
            .RuleFor(x => x.EndDate, y => y.Date.Recent(60))
            .RuleFor(x => x.ExpectedRows, y => y.Random.Int(0, 50000))
            .RuleFor(x => x.CreateDate, y => y.Date.Recent(60))
            .FinishWith((a, b) => b.Payload = GetUserSearchPayload());


        private static string? collinSettings;
        private static string? dentonSettings;
        private static string? harrisSettings;

        private static string CollinSettings => collinSettings ??= GetCollinSettings();
        private static string DentonSettings => dentonSettings ??= GetDentonSettings();
        private static string HarrisSettings => harrisSettings ??= GetHarrisSettings();

        private static string GetCollinSettings()
        {
            return Properties.Resources.user_payload_sample_collin;
        }

        private static string GetDentonSettings()
        {
            return Properties.Resources.user_payload_sample_denton;
        }

        private static string GetHarrisSettings()
        {
            return Properties.Resources.user_payload_sample_harris;
        }



        private static string? tarrantSettings;

        private static string TarrantSettings => tarrantSettings ??= GetTarrantSettings();

        private static string GetTarrantSettings()
        {
            return Properties.Resources.user_payload_sample_tarrant;
        }
    }
}
