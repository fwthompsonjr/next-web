using Bogus;
using HtmlAgilityPack;
using next.core.entities;
using next.core.extensions;
using next.core.implementations;
namespace next.core.tests.extensions
{

    public class InvoiceExtensionsTests
    {
        [Fact]
        public void InvoiceHasContent()
        {
            var html = GetContent();
            Assert.NotEmpty(html);
        }

        [Fact]
        public void ModelCanBeGenerated()
        {
            var exception = Record.Exception(() =>
            {
                _ = faker.Generate();
            });
            Assert.Null(exception);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        [InlineData(true, null)]
        [InlineData(true, 0)]
        [InlineData(true, 1)]
        [InlineData(true, 10, true)]
        [InlineData(true, 6, false, true)]
        public void ModelCanGetHtml(
            bool hasTemplateItems,
            int? dataItems = 5,
            bool hasLowCost = false,
            bool hasClientSecret = true)
        {
            var exception = Record.Exception(() =>
            {
                var model = faker.Generate();
                if (!hasClientSecret) model.ClientSecret = null;
                var items = dataItems.HasValue ? datafaker.Generate(dataItems.Value) : null;
                if (hasLowCost && items != null)
                {
                    var smallnbr = 0.000001f;
                    items.ForEach(x => x.Price = smallnbr);
                }
                model.Data = items;
                var html = hasTemplateItems ? GetContent() : RemoveInvoiceLineItemNode(GetContent());
                var code = Guid.NewGuid().ToString();
                var converted = model.GetHtml(html, code);
                Assert.False(string.IsNullOrEmpty(converted));
            });
            Assert.Null(exception);
        }

        private static string GetContent()
        {
            if (!string.IsNullOrEmpty(_content)) return _content;
            var content = new ContentHtmlNames();
            var invoice = content.GetContent("invoice");
            if (invoice == null) return string.Empty;
            _content = invoice.Content;
            return _content;
        }

        private static string RemoveInvoiceLineItemNode(string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            var parentNode = doc.DocumentNode;
            var detailNode = parentNode.SelectSingleNode("//ul[@name='invoice-line-items']");
            var listNode = detailNode?.ParentNode;
            if (listNode == null) return html;
            listNode.RemoveChild(detailNode);
            return parentNode.OuterHtml;
        }

        private static string? _content;

        private static readonly Faker<InvoiceResponseData> datafaker
            = new Faker<InvoiceResponseData>()
            .RuleFor(x => x.LineId, y => y.Random.String(5, 500))
            .RuleFor(x => x.UserId, y => y.Random.String(5, 500))
            .RuleFor(x => x.ItemType, y => y.Random.String(5, 500))
            .RuleFor(x => x.ItemCount, y => y.Random.Int(5, 500))
            .RuleFor(x => x.UnitPrice, y => y.Random.Int(5, 500))
            .RuleFor(x => x.Price, y => y.Random.Int(5, 500))
            .RuleFor(x => x.ReferenceId, y => y.Random.String(5, 500))
            .RuleFor(x => x.ExternalId, y => y.Random.String(5, 500))
            .RuleFor(x => x.PurchaseDate, y => y.Date.Recent())
            .RuleFor(x => x.IsDeleted, y => y.Random.Bool())
            .RuleFor(x => x.CreateDate, y => y.Date.Recent());

        private static readonly Faker<GenerateInvoiceResponse> faker
            = new Faker<GenerateInvoiceResponse>()
            .RuleFor(x => x.Id, y => y.Random.String(5, 500))
            .RuleFor(x => x.PaymentIntentId, y => y.Random.String(5, 500))
            .RuleFor(x => x.Description, y => y.Random.String(5, 500))
            .RuleFor(x => x.ClientSecret, y => y.Random.String(5, 500))
            .RuleFor(x => x.ExternalId, y => y.Random.String(5, 500))
            .RuleFor(x => x.SuccessUrl, y => y.Random.String(5, 500))
            .RuleFor(x => x.Data, y =>
            {
                var nn = y.Random.Int(1, 6);
                return datafaker.Generate(nn);
            });
    }
}
