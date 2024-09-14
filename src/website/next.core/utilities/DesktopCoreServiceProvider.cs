using next.core.entities;
using next.core.implementations;
using next.core.interfaces;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace next.core.utilities
{
    public static class DesktopCoreServiceProvider
    {
        public static IServiceProvider Provider => _serviceProvider ??= GetProvider();
        private static IServiceProvider? _serviceProvider;

        private static IServiceProvider GetProvider()
        {
            var builder = new ServiceCollection();
            builder.AddSingleton<IContentHtmlNames, ContentHtmlNames>();
            builder.AddSingleton<IErrorContentProvider, ErrorContentProvider>();
            builder.AddScoped<IContentParser, ContentParser>();
            builder.AddSingleton<IInternetStatus, InternetStatus>();
            builder.AddSingleton<IUserProfileMapper, UserProfileMapper>();
            builder.AddSingleton<IUserPermissionsMapper, UserPermissionsMapper>();
            builder.AddSingleton<IUserMailboxMapper, MailboxMapper>();
            builder.AddSingleton<ICopyrightBuilder>(new CopyrightBuilder());
            builder.AddSingleton<IMailPersistence>(new MailPersistence(null));
            builder.AddSingleton<IMailReader>(new MailReader());
            builder.AddSingleton<IHistoryReader>(new HistoryReader());
            builder.AddSingleton<IHistoryPersistence>(new HistoryPersistence(null));
            builder.AddSingleton(new CommonMessageList());
            builder.AppendQueueServices();
            var menucontent = Properties.Resources.contextmenu;
            if (string.IsNullOrEmpty(menucontent)) return builder.BuildServiceProvider();
            builder.AddSingleton(s =>
            {
                return ObjectExtensions.TryGet<MenuConfiguration>(menucontent);
            });
            return builder.BuildServiceProvider();
        }

        private static void AppendQueueServices(this IServiceCollection services)
        {
            var setting = GetQueueSetting();
            var stopper = new QueueStopper(setting);
            var fallback = new QueueBaseStarter(setting, stopper);
            services.AddSingleton<IQueueSettings>(q => setting);
            services.AddSingleton<IQueueStopper>(q => stopper);
            var collection = new List<IQueueStarter>()
            {
                fallback,
                new QueueLocalStarter(setting, stopper),
                new QueueStarter(setting, stopper)
            };
            var instances = collection.FindAll(w => w.IsReady);
            var mxid = instances.Max(x => x.PriorityId);
            var selection = instances.Find(x => x.PriorityId == mxid) ?? fallback;
            services.AddSingleton(q => selection);
            var filter = new QueueFilter(selection, stopper);
            services.AddSingleton<IQueueFilter>(q => filter);
        }
        private static QueueSettings GetQueueSetting()
        {
            var fallback = new QueueSettings { IsEnabled = false, FolderName = "", Name = "Default" };
            try
            {
                var config = Properties.Resources.queuesetting_json;
                var setting = JsonConvert.DeserializeObject<QueueSettings>(config) ?? fallback;
                return setting;
            }
            catch
            {
                return fallback;
            }
        }
    }
}