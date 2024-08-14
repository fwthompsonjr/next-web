using Bogus;
using legallead.desktop.entities;
using next.web.core.models;
using next.web.core.reponses;
using next.web.core.util;
using next.web.Models;

namespace next.web.tests
{
    internal static class MockObjectProvider
    {
        public static T GetSingle<T>() where T : class, new()
        {
            var faker = GetFaker<T>();
            if (faker == null) return new();
            return faker.Generate();
        }

        public static List<T>? GetList<T>(int count) where T : class, new()
        {
            var faker = GetFaker<T>();
            if (faker == null) return [];
            return faker.Generate(count);
        }

        public static string GetContent(string pageName)
        {
            var content = ContentHandler.GetLocalContent(pageName);
            return content?.Content ?? string.Empty;
        }

        private static Faker<T>? GetFaker<T>() where T : class, new()
        {
            var requested = typeof(T);
            if (!Supported.Contains(requested)) return null;
            var indx = Supported.IndexOf(requested);
            object? faker = indx switch
            {
                0 => fkMailItem,
                1 => fkUserSearchQueryBo,
                2 => fkMyPurchaseBo,
                3 => fkUserIdentityBo,
                4 => fkMySearchRestrictions,
                5 => fkMailItemBody,
                6 => fkPermissionChangedItem,
                7 => fkPermissionChangedResponse,
                8 => fkFormSubmissionResponse,
                9 => fkUserSearchFilterBo,
                10 => fkFetchIntentResponse,
                11 => fkFetchIntentRequest,
                12 => fkErrorViewModel,
                13 => fkDownloadJsResponse,
                14 => fkCacheUpdateRequest,
                _ => null
            };
            if (faker is not Faker<T> actual) return null;
            return actual;
        }

        private static readonly Faker<MailItem> fkMailItem = new Faker<MailItem>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString())
            .RuleFor(x => x.UserId, y => y.Random.Guid().ToString())
            .RuleFor(x => x.FromAddress, y => y.Person.Email)
            .RuleFor(x => x.ToAddress, y => y.Person.Email)
            .RuleFor(x => x.Subject, y => y.Lorem.Sentence(5, 3))
            .RuleFor(x => x.StatusId, y => y.PickRandom(StatusIndexes))
            .RuleFor(x => x.CreateDate, y => y.Date.Recent(180))
            .FinishWith((f, a) =>
            {
                a.CreateDt = a.CreateDate.GetValueOrDefault().ToString("f");
            });

        private static readonly Faker<MailItemBody> fkMailItemBody = new Faker<MailItemBody>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString())
            .RuleFor(x => x.Body, y => y.Lorem.Sentence(10, 15));

        private static readonly Faker<UserSearchQueryBo> fkUserSearchQueryBo =
            new Faker<UserSearchQueryBo>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString())
            .RuleFor(x => x.Name, y => y.Random.Guid().ToString())
            .RuleFor(x => x.UserId, y => y.Random.Guid().ToString())
            .RuleFor(x => x.StartDate, y => y.Date.Recent(180))
            .RuleFor(x => x.EndDate, y => y.Date.Recent(180))
            .RuleFor(x => x.EstimatedRowCount, y => y.Random.Int(1, 500))
            .RuleFor(x => x.StateCode, y => "TX")
            .RuleFor(x => x.CountyName, y => y.PickRandom(CountyNames))
            .RuleFor(x => x.CreateDate, y => y.Date.Recent(180))
            .FinishWith((f, a) =>
            {
                var rng = f.Random.Int(1, 7);
                var indx = f.PickRandom(StatusIndexes);
                a.EndDate = a.StartDate.GetValueOrDefault().AddDays(rng);
                a.SearchProgress = GetStatus(indx);
            });

        private static readonly Faker<MyPurchaseBo> fkMyPurchaseBo =
            new Faker<MyPurchaseBo>()
            .RuleFor(x => x.ReferenceId, y => y.Random.Guid().ToString())
            .RuleFor(x => x.ExternalId, y => y.Random.Guid().ToString())
            .RuleFor(x => x.ItemType, y => y.Random.Guid().ToString())
            .RuleFor(x => x.ItemCount, y => y.Random.Int(1, 180))
            .RuleFor(x => x.Price, y => y.Random.Int(100, 10000))
            .RuleFor(x => x.PurchaseDate, y => y.Date.Recent(180))
            .FinishWith((f, a) =>
            {
                var indx = f.PickRandom(StatusIndexes);
                a.StatusText = GetStatus(indx);
            });

        private static readonly Faker<UserIdentityBo> fkUserIdentityBo =
            new Faker<UserIdentityBo>()
            .RuleFor(x => x.UserName, y => y.Person.UserName)
            .RuleFor(x => x.Email, y => y.Person.Email)
            .RuleFor(x => x.Created, y => y.Date.Recent(180).ToString("s"))
            .RuleFor(x => x.Role, y => y.Random.Int(1, 10).ToString())
            .RuleFor(x => x.RoleDescription, y => y.Hacker.Phrase())
            .RuleFor(x => x.FullName, y => y.Person.FullName);

        private static readonly Faker<MySearchRestrictions> fkMySearchRestrictions =
            new Faker<MySearchRestrictions>()
            .RuleFor(x => x.IsLocked, y => false)
            .RuleFor(x => x.Reason, y => y.Hacker.Phrase())
            .RuleFor(x => x.MaxPerMonth, y => y.Random.Int(50, 100))
            .RuleFor(x => x.MaxPerYear, y => y.Random.Int(5000, 10000))
            .RuleFor(x => x.ThisMonth, y => y.Random.Int(1, 40))
            .RuleFor(x => x.ThisYear, y => y.Random.Int(1000, 4500));

        private static readonly Faker<PermissionChangedItem> fkPermissionChangedItem =
            new Faker<PermissionChangedItem>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString())
            .RuleFor(x => x.UserId, y => y.Random.Guid().ToString())
            .RuleFor(x => x.CustomerId, y => y.Random.Guid().ToString())
            .RuleFor(x => x.ExternalId, y => y.Random.Guid().ToString())
            .RuleFor(x => x.InvoiceUri, y => y.Random.AlphaNumeric(75))
            .RuleFor(x => x.LevelName, y => y.Lorem.Word())
            .RuleFor(x => x.SessionId, y => y.Random.Int(1, 5000).ToString())
            .RuleFor(x => x.IsPaymentSuccess, y => y.Random.Bool())
            .RuleFor(x => x.CompletionDate, y => y.Date.Recent(180))
            .RuleFor(x => x.CreateDate, y => y.Date.Recent(180));

        private static readonly Faker<PermissionChangedResponse> fkPermissionChangedResponse =
            new Faker<PermissionChangedResponse>()
            .RuleFor(x => x.Name, y => y.Random.Guid().ToString())
            .RuleFor(x => x.Email, y => y.Person.Email)
            .RuleFor(x => x.Request, y => y.Random.Guid().ToString())
            .FinishWith((f, a) =>
            {
                a.Dto = fkPermissionChangedItem.Generate();
            });

        private static readonly Faker<FormSubmissionResponse> fkFormSubmissionResponse =
            new Faker<FormSubmissionResponse>()
            .RuleFor(x => x.StatusCode, y => y.Random.Int(100, 600))
            .RuleFor(x => x.Message, y => y.Hacker.Phrase())
            .RuleFor(x => x.RedirectTo, y => y.Internet.Url())
            .RuleFor(x => x.OriginalFormName, y => y.Person.FullName);

        private static readonly Faker<UserSearchFilterBo> fkUserSearchFilterBo =
            new Faker<UserSearchFilterBo>()
            .RuleFor(x => x.Index, y => y.PickRandom(StatusIndexes))
            .RuleFor(x => x.County, y => y.PickRandom(CountyNames));

        private static readonly Faker<FetchIntentResponse> fkFetchIntentResponse =
            new Faker<FetchIntentResponse>()
            .RuleFor(x => x.ClientSecret, y => y.Random.Guid().ToString());

        private static readonly Faker<FetchIntentRequest> fkFetchIntentRequest =
            new Faker<FetchIntentRequest>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString());

        private static readonly Faker<ErrorViewModel> fkErrorViewModel =
            new Faker<ErrorViewModel>()
            .RuleFor(x => x.RequestId, y => y.Random.Guid().ToString());

        private static readonly Faker<DownloadJsResponse> fkDownloadJsResponse =
            new Faker<DownloadJsResponse>()
            .RuleFor(x => x.ExternalId, y => y.Random.Int(100, 600).ToString())
            .RuleFor(x => x.Description, y => y.Hacker.Phrase())
            .RuleFor(x => x.Content, y => y.Internet.Url())
            .RuleFor(x => x.Error, y => y.Internet.Url())
            .RuleFor(x => x.CreateDate, y => y.Date.Future().ToString("s"));

        private static readonly Faker<CacheUpdateRequest> fkCacheUpdateRequest =
            new Faker<CacheUpdateRequest>()
            .RuleFor(x => x.Name, y => y.Person.FullName);

        private static readonly List<Type> Supported =
        [
            typeof(MailItem), // -- 0
            typeof(UserSearchQueryBo), // -- 1
            typeof(MyPurchaseBo), // -- 2
            typeof(UserIdentityBo), // -- 3
            typeof(MySearchRestrictions), // -- 4
            typeof(MailItemBody), // -- 5
            typeof(PermissionChangedItem), // -- 6
            typeof(PermissionChangedResponse), // -- 7
            typeof(FormSubmissionResponse), // -- 8
            typeof(UserSearchFilterBo), // -- 9
            typeof(FetchIntentResponse), // -- 10
            typeof(FetchIntentRequest), // -- 11
            typeof(ErrorViewModel), // -- 12
            typeof(DownloadJsResponse), // -- 13
            typeof(CacheUpdateRequest), // -- 14
        ];


        private static string GetStatus(int indx)
        {
            return indx switch
            {
                1 => "1 - Submitted",
                2 => "2 - Processing",
                3 => "3 - Completed",
                4 => "4 - Purchased",
                5 => "5 - Downloaded",
                10 => "10 - Error",
                _ => ""
            };
        }
        private static readonly List<int> StatusIndexes = [1, 2, 3, 4, 5, 10];
        private static readonly List<string> CountyNames = ["Collin", "Denton", "Harris", "Tarrant"];

    }
}
