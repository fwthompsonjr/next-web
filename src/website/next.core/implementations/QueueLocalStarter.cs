using next.core.interfaces;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace next.core.implementations
{
    [ExcludeFromCodeCoverage(Justification = "Interacts with file system. Tested in integration only")]
    internal class QueueLocalStarter : QueueBaseStarter
    {
        public QueueLocalStarter(IQueueSettings settings, IQueueStopper stopper) : base(settings, stopper)
        {
            ParentName = TargetFolder(settings.FolderName);
        }
        public override int PriorityId => 100;
        public override string Name => "Local";

        public override string ServiceName => stoppingService.ServiceName;
        public override bool IsReady
        {
            get
            {
                if (!settingsModel.IsEnabled) { return false; }
                if (string.IsNullOrEmpty(ServiceName)) { return false; }
                if (string.IsNullOrEmpty(ParentName)) { return false; }
                return true;
            }
        }

        public override void Start()
        {
            if (!settingsModel.IsEnabled) { return; }
            if (string.IsNullOrEmpty(ServiceName)) { return; }
            if (string.IsNullOrEmpty(ParentName)) { return; }
            var targetFile = GetAppInfo();
            if (targetFile == null || IsProcRunning(settingsModel.Name)) { return; }
            LaunchExe(targetFile);
        }

        public override List<string> UserList()
        {
            const string extn = "json.txt";
            var pattern = $"*.{extn}";
            var targetFile = GetAppInfo();
            if (targetFile == null) return _userList;
            var parent = Path.GetDirectoryName(targetFile.FullName);
            if (!Directory.Exists(parent)) return _userList;
            var child = Path.Combine(parent, "_configuration");
            if (!Directory.Exists(child)) return _userList;
            var collection = GetFiles(child, pattern).ToList();
            if (collection.Count == 0) return _userList;
            var items = new List<string>();
            collection.ForEach(c =>
            {
                var contents = File.ReadAllText(c.FullName);
                var obj = ObjectExtensions.TryGet<List<string>>(contents);
                if (obj != null)
                {
                    obj = obj.FindAll(x => !items.Contains(x, StringComparer.OrdinalIgnoreCase));
                    items.AddRange(obj);
                }
            });
            items = items.Distinct().OrderBy(x => x).ToList();
            return items;
        }

        public string ConfigPath()
        {
            var targetFile = GetAppInfo();
            if (targetFile == null) return string.Empty;
            var parent = Path.GetDirectoryName(targetFile.FullName);
            if (!Directory.Exists(parent)) return string.Empty;
            var child = Path.Combine(parent, "_configuration");
            if (!Directory.Exists(child)) return string.Empty;
            return child;
        }

        public void Append(string userId)
        {
            if (!Guid.TryParse(userId, out var _)) return;
            lock (sync)
            {
                var list = UserList();
                if (list.Contains(userId, StringComparer.OrdinalIgnoreCase)) return;

                var targetFile = GetAppInfo();
                if (targetFile == null) return;
                var parent = Path.GetDirectoryName(targetFile.FullName);
                if (!Directory.Exists(parent)) return;
                var child = Path.Combine(parent, "_configuration");
                if (!Directory.Exists(child)) return;
                var data = new List<string> { userId };
                var fileItem = "applicationuser_0.json.txt";
                var current = 0;
                var shortName = fileItem.Replace("_0", $"_{current:D5}");
                var fullName = Path.Combine(child, shortName);
                while (File.Exists(fullName))
                {
                    current++;
                    shortName = fileItem.Replace("_0", $"_{current:D5}");
                    fullName = Path.Combine(child, shortName);
                }
                var serialized = JsonConvert.SerializeObject(data, Formatting.Indented);
                File.WriteAllText(fullName, serialized);
            }
        }


        private static bool IsProcRunning(string? processName)
        {
            if (string.IsNullOrEmpty(processName)) return false;
            var processes = Process.GetProcessesByName(processName).ToList();
            return processes.Count > 0;
        }

        private static void LaunchExe(FileInfo target)
        {
            var path = target.FullName;
            if (!File.Exists(path)) return;
            Process myProcess = new();
            var info = myProcess.StartInfo;
            info.WorkingDirectory = Path.GetDirectoryName(path);
            info.WindowStyle = ProcessWindowStyle.Normal;
            info.FileName = path;
            info.CreateNoWindow = false;
            info.UseShellExecute = true;
            myProcess.Start();
        }


        private static string TargetFolder(string? subDirectory)
        {
            var parent = AppFolder;
            if (string.IsNullOrWhiteSpace(subDirectory)) { return string.Empty; }
            var sourceDir = Path.Combine(parent, subDirectory);
            if (!Directory.Exists(sourceDir)) return string.Empty;
            return sourceDir;
        }

        private static string? _appFolder;
        private static string AppFolder => _appFolder ??= GetAppFolder();
        private static string GetAppFolder()
        {
            var localPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            if (string.IsNullOrEmpty(localPath)) { return string.Empty; }
            if (!Directory.Exists(localPath)) { return string.Empty; }
            return localPath;
        }

        private static readonly List<string> _userList = new();
        private static readonly object sync = new();
    }
}