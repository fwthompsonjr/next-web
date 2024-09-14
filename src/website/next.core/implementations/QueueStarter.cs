using next.core.interfaces;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace next.core.implementations
{
    [ExcludeFromCodeCoverage(Justification = "Interacts with file system. Tested in integration only")]
    internal class QueueStarter : QueueLocalStarter
    {
        public QueueStarter(IQueueSettings settings, IQueueStopper stopper) : base(settings, stopper)
        {
            ParentName = TargetFolder(settings.FolderName);
        }
        public override int PriorityId => 0;
        public override string Name => "Application";


        private static string TargetFolder(string? subDirectory)
        {
            var parent = AppFolder;
            if (string.IsNullOrWhiteSpace(subDirectory)) { return string.Empty; }
            if (!parent.Contains(subDirectory, StringComparison.OrdinalIgnoreCase)) return string.Empty;
            var a = subDirectory.Length;
            var b = parent.IndexOf(subDirectory, StringComparison.OrdinalIgnoreCase);
            if (b == -1) return string.Empty;
            var sourceDir = parent.Substring(0, b + a);
            if (!Directory.Exists(sourceDir)) return string.Empty;
            return sourceDir;
        }

        private static string? _appFolder;
        private static string AppFolder => _appFolder ??= GetAppFolder();
        private static string GetAppFolder()
        {
            var exeFile = Assembly.GetExecutingAssembly().Location;
            if (string.IsNullOrEmpty(exeFile)) { return string.Empty; }
            var exePath = Path.GetDirectoryName(exeFile);
            if (string.IsNullOrEmpty(exePath) ||
                !Directory.Exists(exePath)) { return string.Empty; }
            return exePath;
        }
    }
}
