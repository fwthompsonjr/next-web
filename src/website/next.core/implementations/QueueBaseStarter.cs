using next.core.interfaces;
using System.Diagnostics.CodeAnalysis;

namespace next.core.implementations
{
    [ExcludeFromCodeCoverage(Justification = "Interacts with file system. Tested in integration only")]
    internal class QueueBaseStarter : IQueueStarter
    {

        protected readonly IQueueSettings settingsModel;
        protected readonly IQueueStopper stoppingService;
        public QueueBaseStarter(IQueueSettings settings, IQueueStopper stopper)
        {
            settingsModel = settings;
            stoppingService = stopper;
        }

        public virtual int PriorityId => -1;

        public virtual string Name => "Inactive";

        public virtual bool IsReady => true;

        public virtual string ServiceName => "Inactive";

        public virtual void Start()
        {
        }

        public virtual List<string> UserList()
        {
            return _userList;
        }


        protected FileInfo? GetAppInfo()
        {
            var name = settingsModel.Name;
            if (string.IsNullOrEmpty(name)) { return null; }
            var pattern = $"{name}.exe";
            var collection = GetFiles(ParentName, pattern).ToList();
            if (collection.Count == 0) return null;
            collection.Sort((a, b) => b.CreationTime.CompareTo(a.CreationTime));
            return collection[0];
        }

        protected virtual string ParentName { get; set; } = string.Empty;

        private static readonly List<string> _userList = new();

        protected static IEnumerable<FileInfo> GetFiles(string source, string pattern)
        {
            var empty = Enumerable.Empty<FileInfo>();
            if (string.IsNullOrEmpty(source)) { return empty; }
            if (!Directory.Exists(source)) { return empty; }
            var found = new DirectoryInfo(source).GetFiles(pattern, SearchOption.AllDirectories);
            return found;
        }
    }
}
