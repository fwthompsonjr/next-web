using Bogus;
using next.web.core.models;
using legallead.desktop.entities;

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
        private static Faker<T>? GetFaker<T>() where T : class, new()
        {
            var requested = typeof(T);
            if (Supported.Contains(requested)) return null;
            var indx = Supported.IndexOf(requested);
            object? faker = indx switch
            {
                0 => fkMailItem,
                1 => fkUserSearchQueryBo,
                2 => fkMyPurchaseBo,
                3 => fkUserIdentityBo,
                4 => fkMySearchRestrictions,
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

        private static readonly List<Type> Supported =
        [
            typeof(MailItem),
            typeof(UserSearchQueryBo),
            typeof(MyPurchaseBo),
            typeof(UserIdentityBo),
            typeof(MySearchRestrictions),
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
