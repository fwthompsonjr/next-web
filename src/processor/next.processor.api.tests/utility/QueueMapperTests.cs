using legallead.permissions.api.Model;
using legallead.records.search.Classes;
using next.processor.api.extensions;
using next.processor.api.utility;

namespace next.processor.api.tests.utility
{
    public class QueueMapperTests
    {
        [Fact]
        public void MapperCanTranslateUserSearchRequest()
        {
            const int iterations = 15;
            var error = Record.Exception(() =>
            {
                for (var i = 0; i < iterations; i++)
                {
                    var payload = MockObjProvider.GetUserSearchPayload();
                    var search = payload.ToInstance<UserSearchRequest>();
                    Assert.NotNull(search);
                    var mapped = QueueMapper.MapFrom<UserSearchRequest, WebInteractive>(search);
                    Assert.NotNull(mapped);
                }
            });
            Assert.Null(error);
        }
    }
}
