using legallead.permissions.api.Model;
using legallead.records.search.Classes;
using next.processor.api.backing;
using next.processor.api.extensions;
using next.processor.api.utility;

namespace next.processor.api.services
{
    public class WebVerifyPageReadCollin(IConfiguration configuration) : WebVerifyInstall(configuration)
    {
        protected virtual int WebId => 0;
        public async override Task<bool> InstallAsync()
        {
            if (IsInstalled) return true;
            var id = WebId;
            var interactive = GetWeb(id);
            if (interactive == null) return false;
            LastErrorMessage = string.Empty;
            var sut = new ContainerizedWebInteractive(interactive);
            var response = await Task.Run(() =>
            {
                try
                {
                    return sut.Fetch();
                }
                catch (Exception ex)
                {
                    ex.Log(GetSourceName());
                    LastErrorMessage = ex.ToString();
                    return null;
                }
            });
            IsInstalled = response != null && response.PeopleList.Count != 0;
            return IsInstalled;
        }
        protected string GetSourceName()
        {
            var name = WebId switch
            {
                0 => "web-test-collin",
                1 => "web-test-denton",
                2 => "web-test-harris",
                3 => "web-test-tarrant",
                4 => "web-test-harris-jp",
                _ => "web-test"
            };
            return name;
        }
        protected virtual WebInteractive? GetWeb(int index)
        {
            var max = collection.Count - 1;
            if (index < 0 || index > max) return null;
            var payload = GetUserSearchPayload(index);
            var search = payload.ToInstance<UserSearchRequest>();
            if (search == null) return null;
            SetStartAndEndDates(search);
            return QueueMapper.MapFrom<UserSearchRequest, WebInteractive>(search);
        }

        private static void SetStartAndEndDates(UserSearchRequest search)
        {
            List<DayOfWeek> weekends = [DayOfWeek.Sunday, DayOfWeek.Saturday];
            var date = DateTime.UtcNow.AddDays(-1);
            while (weekends.Contains(date.DayOfWeek)) { date = date.AddDays(-1); }
            var dtoStart = new DateTimeOffset(date.Date);
            var dtoEnding = new DateTimeOffset(date.Date.AddDays(1).AddMinutes(-1));
            search.StartDate = dtoStart.ToUnixTimeMilliseconds();
            search.EndDate = dtoEnding.ToUnixTimeMilliseconds();
        }

        private static string GetUserSearchPayload(int index)
        {
            return collection[index];
        }
        private static readonly List<string> collection = [CollinSettings, DentonSettings, HarrisSettings, TarrantSettings, HarrisJpSettings];

        private static string? collinSettings;
        private static string? dentonSettings;
        private static string? harrisSettings;
        private static string? harrisJpSettings;
        private static string? tarrantSettings;

        private static string CollinSettings => collinSettings ??= GetCollinSettings();
        private static string DentonSettings => dentonSettings ??= GetDentonSettings();
        private static string HarrisSettings => harrisSettings ??= GetHarrisSettings();
        private static string HarrisJpSettings => harrisJpSettings ??= GetHarrisJpSettings();
        private static string TarrantSettings => tarrantSettings ??= GetTarrantSettings();
        private static string GetCollinSettings()
        {
            return Properties.Resources.payload_sample_collin;
        }

        private static string GetDentonSettings()
        {
            return Properties.Resources.payload_sample_denton;
        }

        private static string GetHarrisSettings()
        {
            return Properties.Resources.payload_sample_harris;
        }

        private static string GetHarrisJpSettings()
        {
            return Properties.Resources.payload_sample_harris_jp;
        }

        private static string GetTarrantSettings()
        {
            return Properties.Resources.payload_sample_tarrant;
        }
    }
}
