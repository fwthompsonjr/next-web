using Bogus;
using next.core.implementations;
using next.core.interfaces;
using next.core.models;
using Newtonsoft.Json;

namespace next.core.tests.implementations
{
    public class MailboxMapperTests
    {

        [Fact]
        public void MapperCanSubstitute()
        {
            var problems = Record.Exception(() =>
            {
                var content = next.core.Properties.Resources.mailbox_base_html;
                var persistence = new MockPersistence();
                var mapper = new MailboxMapper();
                _ = mapper.Substitute(persistence, content);
            });
            Assert.Null(problems);
        }

        private static readonly Faker<MailStorageItem> itemFaker
            = new Faker<MailStorageItem>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString())
            .RuleFor(x => x.PositionId, y => y.Random.Int(1, 1000))
            .RuleFor(x => x.UserId, y => y.Random.Guid().ToString())
            .RuleFor(x => x.FromAddress, y => y.Person.Email)
            .RuleFor(x => x.ToAddress, y => y.Person.Email)
            .RuleFor(x => x.Subject, y => y.Lorem.Sentence(5))
            .RuleFor(x => x.CreateDate, y => y.Date.Recent(300).ToString("f"));

        private sealed class MockPersistence : IMailPersistence
        {
            public void Clear()
            {
                throw new NotImplementedException();
            }

            public bool DoesItemExist(string id)
            {
                throw new NotImplementedException();
            }

            public string? Fetch()
            {
                var count = new Faker().Random.Int(2, 15);
                var collection = itemFaker.Generate(count);
                return JsonConvert.SerializeObject(collection);
            }

            public string? Fetch(string id)
            {
                var lines = string.Join(Environment.NewLine, rawHtml);
                lines = lines.Replace("'", '"'.ToString());
                return lines;
            }

            public void Save(string json)
            {
                throw new NotImplementedException();
            }

            public void Save(string id, string json)
            {
                throw new NotImplementedException();
            }

            private static readonly List<string> rawHtml = new()
            {
                "<html>",
                "<body>",
                "<div id='email' style='width:750px; margin: 5px; padding: 5px; font-family: Arial; font-size: 12pt'>",
                "<table name='header' role='presentation' border='0' cellspacing='0' width='100%'>",
                "<tbody>",
                "<tr style='border-bottom-style: dashed'>",
                "<td colspan='3'>",
                "",
                "<!-- Heading -->",
                "<h1 name='span-heading' style='border-bottom: 1px dashed'>",
                "<span>Legal Lead Correspondence</span>",
                "</h1>",
                "</td>",
                "</tr>",
                "<tr style='color: blue'>",
                "<td width='5%'></td>",
                "<td width='70%'>",
                "",
                "<!-- SubHeading -->",
                "<h2 name='span-sub-heading'>Account Information</h2>",
                "</td>",
                "<td width='25%'>",
                "",
                "<!-- SubHeadingDate -->",
                "<h2 name='span-sub-heading-date'>Jun 8, 2024</h2>",
                "</td>",
                "</tr>",
                "<tr>",
                "<td width='5%'></td>",
                "<td colspan='2'>",
                "",
                "<!-- Greeting -->",
                "<h3 style='margin-left: 10px;'>",
                "<span name='span-greeting'>Hello Judge Mertz</span>,",
                "</h3>",
                "</td>",
                "</tr>",
                "</tbody>",
                "</table>",
                "<table name='body' role='presentation' border='0' cellspacing='0' width='100%'>",
                "<tbody>",
                "<tr>",
                "<td width='8%'></td>",
                "<td name='body-line-introduction'>",
                "You are receiving this email due to the following change(s) to your Legal Lead Account:",
                "</td>",
                "<td width='5%'></td>",
                "</tr>",
                "<tr>",
                "<td width='8%'></td>",
                "<td name='body-line-details'>",
                "<h4 style='color: green' name='begin-search-requested-heading'>",
                "",
                "<!-- Template Heading -->",
                "</h4>",
                "<table name='begin-search-requested-detail-table' role='presentation' border='0' cellspacing='0' style='margin-left: 15px; margin-bottom= 15px;' width='95%'>",
                "<tbody>",
                "<tr>",
                "<td style='width: 25%'>",
                "<span>User Name:</span>",
                "</td>",
                "<td style='width: 75%'>",
                "<span name='begin-search-requested-user-name'>test.account</span>",
                "</td>",
                "</tr>",
                "<tr>",
                "<td style='width: 25%'>",
                "<span>Email:</span>",
                "</td>",
                "<td style='width: 75%'>",
                "<span name='begin-search-requested-email'>user@example.com</span>",
                "</td>",
                "</tr>",
                "<tr>",
                "<td name='begin-search-requested-detail-spacer' colspan='2'>",
                "<hr style='background: #444; height: 1px; border: 0px;'>",
                "</td>",
                "</tr>",
                "<tr>",
                "<td style='padding: 10px;' name='begin-search-requested-details' colspan='2'>",
                "<table name='begin-search-requested-detail-item' role='presentation' border='0' cellspacing='0' width='95%'>",
                "<tbody>",
                "<tr>",
                "<td colspan='4'>",
                "You recently submitted a record search.<br>",
                "The details of your search request are listed below.",
                "</td>",
                "</tr>",
                "<tr>",
                "<td colspan='4' name='begin-search-requested-detail-item-spacer'>",
                "<hr style='background: #444; height: 1px; border: 0px;'>",
                "</td>",
                "</tr>",
                "<tr>",
                "<td width='20%'>State:</td>",
                "<td width='15%' name='td-begin-search-requested-state-code'>TX</td>",
                "<td width='20%'>County:</td>",
                "<td width='45%' name='td-begin-search-requested-county-name'>Denton</td>",
                "</tr>",
                "<tr>",
                "<td colspan='1'>Start Date:</td>",
                "<td colspan='3' name='td-begin-search-requested-start-date'>Oct 1, 2023</td>",
                "</tr>",
                "<tr>",
                "<td colspan='1'>End Date:</td>",
                "<td colspan='3' name='td-begin-search-requested-end-date'>Oct 7, 2023</td>",
                "</tr>",
                "</tbody>",
                "</table>",
                "</td>",
                "</tr>",
                "</tbody>",
                "</table>",
                "</td>",
                "<td width='5%'></td>",
                "</tr>",
                "<tr>",
                "<td width='8%'></td>",
                "<td name='body-line-closing'>",
                "For additional information about this change, login to your account in the Legal Lead application.",
                "</td>",
                "<td width='5%'></td>",
                "</tr>",
                "</tbody>",
                "</table>",
                "<br>",
                "<table name='footer' role='presentation' border='0' cellspacing='0' width='100%'>",
                "<tbody>",
                "<tr>",
                "<td width='5%'></td>",
                "<td width='95%'>",
                "",
                "<!-- Closing -->",
                "<span name='span-closing'>Regards,</span>",
                "<br>",
                "<span name='span-closing'>Account Services Team</span>",
                "</td>",
                "</tr>",
                "</tbody>",
                "</table>",
                "</div>",
                "</body>",
                "</html>",
            };
        }
    }
}
