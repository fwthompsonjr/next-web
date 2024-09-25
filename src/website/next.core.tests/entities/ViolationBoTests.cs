using Bogus;
using next.core.entities;
using next.core.extensions;
using next.core.implementations;

namespace next.core.tests.entities
{
    public class ViolationBoTests
    {
        private static readonly Faker<ViolationBo> faker =
            new Faker<ViolationBo>()
            .RuleFor(x => x.IpAddress, y => y.Random.Guid().ToString())
            .RuleFor(x => x.Count, y => y.Random.Int(1, 500000))
            .RuleFor(x => x.SessionId, y => y.Random.Guid().ToString())
            .RuleFor(x => x.Email, y => y.Person.Email)
            .RuleFor(x => x.CreateDate, y => y.Date.Future());

        [Fact]
        public void ModelCanBeCreated()
        {
            var error = Record.Exception(() =>
            {
                _ = new ViolationBo();
            });
            Assert.Null(error);
        }

        [Fact]
        public void ModelCanBeGenerated()
        {
            var error = Record.Exception(() =>
            {
                _ = faker.Generate();
            });
            Assert.Null(error);
        }

        [Fact]
        public void ModelCanGetFields()
        {
            var error = Record.Exception(() =>
            {
                var a = new ViolationBo();
                var test = faker.Generate();
                Assert.NotEqual(a.IpAddress, test.IpAddress);
                Assert.NotEqual(a.Count, test.Count);
                Assert.NotEqual(a.SessionId, test.SessionId);
                Assert.NotEqual(a.Email, test.Email);
                Assert.NotEqual(a.CreateDate, test.CreateDate);
                Assert.NotEqual(a.ExpiryDate, test.ExpiryDate);
            });
            Assert.Null(error);
        }

        [Theory]
        [InlineData(0, 0, false)]
        [InlineData(2, 2, false)]
        [InlineData(4, 4, false)]
        [InlineData(5, 2, true)]
        [InlineData(2, 5, true)]
        [InlineData(5, 5, true)]
        public void ListCanVerifyViolations(int sessionCount, int ipCount, bool expected)
        {
            var list = GetList(sessionCount, ipCount, out var model);
            var actual = list.Check(model.IpAddress, model.SessionId);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(2, 2)]
        [InlineData(5, 2)]
        [InlineData(2, 5)]
        [InlineData(5, 5)]
        [InlineData(4, 4)]
        public void ListCanVerifyExpiration(int sessionCount, int ipCount)
        {
            var createDate = DateTime.UtcNow.AddDays(-1);
            var list = GetList(sessionCount, ipCount, out var model);
            list.ForEach(a => a.CreateDate = createDate);
            var actual = list.Check(model.IpAddress, model.SessionId);
            Assert.False(actual);
        }


        [Theory]
        [InlineData(0, 0, false)]
        [InlineData(2, 2, false)]
        [InlineData(4, 4, false)]
        [InlineData(5, 2, true)]
        [InlineData(2, 5, true)]
        [InlineData(5, 5, true)]
        public void ServiceCanVerifyViolations(int sessionCount, int ipCount, bool expected)
        {
            var service = new ViolationService();
            var list = GetList(sessionCount, ipCount, out var model);
            list.ForEach(service.Append);
            var actual = service.IsViolation(model);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(2, 2)]
        [InlineData(5, 2)]
        [InlineData(2, 5)]
        [InlineData(5, 5)]
        [InlineData(4, 4)]
        public void ServiceCanExpireViolations(int sessionCount, int ipCount)
        {
            var createDate = DateTime.UtcNow.AddDays(-1);
            var service = new ViolationService(true);
            var list = GetList(sessionCount, ipCount, out var model);
            list.ForEach(a => a.CreateDate = createDate);
            list.ForEach(service.Append);
            var actual = service.IsViolation(model);
            Assert.False(actual);
        }

        private static List<ViolationBo> GetList(int sessionCount, int ipCount, out ViolationBo model)
        {
            var list = new List<ViolationBo>();
            var sessionId = string.Empty;
            var ipaddress = string.Empty;
            var session = faker.Generate(sessionCount);
            var ip = faker.Generate(ipCount);
            if (sessionCount > 0)
            {
                sessionId = session[0].SessionId;
                session.ForEach(x => x.SessionId = sessionId);
            }
            if (ipCount > 0)
            {
                ipaddress = ip[0].IpAddress;
                ip.ForEach(x => x.IpAddress = ipaddress);
            }
            list.AddRange(ip);
            list.AddRange(session);
            model = new ViolationBo { IpAddress = ipaddress, SessionId = sessionId };
            return list;

        }
    }
}
