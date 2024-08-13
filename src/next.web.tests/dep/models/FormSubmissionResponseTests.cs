using next.web.core.models;
using next.web.core.reponses;

namespace next.web.tests.dep.models
{
    public class FormSubmissionResponseTests
    {
        [Fact]
        public void ModelCanBeGenerated()
        {
            var error = Record.Exception(() =>
            {
                var sut = MockObjectProvider.GetList<FormSubmissionResponse>(2) ?? [];
                Assert.NotEmpty(sut);
                Assert.NotEqual(sut[0].StatusCode, sut[1].StatusCode);
                Assert.NotEqual(sut[0].Message, sut[1].Message);
                Assert.NotEqual(sut[0].RedirectTo, sut[1].RedirectTo);
                Assert.NotEqual(sut[0].OriginalFormName, sut[1].OriginalFormName);
            });
            Assert.Null(error);
        }
    }
}