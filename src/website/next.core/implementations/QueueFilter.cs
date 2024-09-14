using next.core.interfaces;
using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;

namespace next.core.implementations
{
    [ExcludeFromCodeCoverage(Justification = "Interacts with file system. Tested in integration only")]
    internal class QueueFilter : IQueueFilter
    {
        private readonly IQueueStarter startingService;
        private readonly IQueueStopper stoppingService;
        public QueueFilter(IQueueStarter starter, IQueueStopper stopper)
        {
            startingService = starter;
            stoppingService = stopper;
        }

        public void Append(string userId)
        {
            if (!Guid.TryParse(userId, out var _)) return;
            stoppingService.Stop();
            var current = startingService.UserList();
            if (!current.Contains(userId) && startingService is QueueLocalStarter instance)
            {
                instance.Append(userId);
            }
            startingService.Start();
        }
        public void Clear()
        {
            lock (sync)
            {
                try
                {
                    stoppingService.Stop();
                    var current = startingService.UserList();
                    if (current.Count <= 1 || startingService is not QueueLocalStarter instance) return;
                    var path = instance.ConfigPath();
                    if (string.IsNullOrEmpty(path)) { return; }
                    var found = new DirectoryInfo(path).GetFiles("*.json.txt");
                    if (found.Any())
                    {
                        found.ToList().ForEach(SafeDelete);
                    }
                    const string basefile = "account-index.json.txt";
                    var destination = Path.Combine(path, basefile);
                    File.WriteAllText(destination, GetBaseContent());
                }
                finally
                {

                    startingService.Start();
                }
            }
        }

        private static string GetBaseContent()
        {
            var item = new[] { Guid.Empty.ToString() };
            var json = JsonConvert.SerializeObject(item);
            return json;
        }
        private static void SafeDelete(FileInfo fileInfo)
        {
            try
            {
                File.Delete(fileInfo.FullName);
            }
            catch
            {
                // this item is intentionally blacnk
            }
        }

        private static readonly object sync = new();
    }
}