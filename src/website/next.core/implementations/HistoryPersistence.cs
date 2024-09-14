using next.core.interfaces;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace next.core.implementations
{
    internal class HistoryPersistence : IHistoryPersistence
    {
        private readonly IFileInteraction _fileService;
        public HistoryPersistence(IFileInteraction? fileService)
        {
            _fileService = fileService ?? new FileInteraction();
        }
        public void Clear()
        {
            var fileNames = new List<string> { HistoryFile, RestrictionFile };
            fileNames.ForEach(ClearFileContent);
        }

        public void Save(string json)
        {
            var fileName = HistoryFile;
            lock (sync)
            {
                _fileService.WriteAllText(fileName, json);
            }
        }

        public void SaveRestriction(string json)
        {
            var fileName = RestrictionFile;
            lock (sync)
            {
                _fileService.WriteAllText(fileName, json);
            }
        }

        public void SaveFilter(string json)
        {
            var fileName = SearchFilterFile;
            if (string.IsNullOrEmpty(fileName)) return;
            lock (sync)
            {
                _fileService.WriteAllText(fileName, json);
            }
        }

        public string? Fetch()
        {
            var fileName = HistoryFile;
            lock (sync)
            {
                return _fileService.ReadAllText(fileName);
            }
        }
        public string? Restriction()
        {
            var fileName = RestrictionFile;
            lock (sync)
            {
                return _fileService.ReadAllText(fileName);
            }
        }
        public string? Filter()
        {
            var fileName = SearchFilterFile;
            if (string.IsNullOrEmpty(fileName)) return null;
            lock (sync)
            {
                return _fileService.ReadAllText(fileName);
            }
        }

        private static readonly object sync = new();
        private static string AppFolder => appFolder ??= GetFolder();
        private static string HistoryFolder => historyFolder ??= GetHistoryFolder();
        private static string HistoryFile => historyFile ??= GetHistoryFile();
        private static string RestrictionFile => restrictionFile ??= GetRestrictionFile();
        private static string SearchFilterFile => filterSettingFile ??= GetFilterFile();

        private static string? appFolder;
        private static string? historyFolder;
        private static string? historyFile;
        private static string? restrictionFile;
        private static string? filterSettingFile;

        [ExcludeFromCodeCoverage(Justification = "Performs file i/o operations")]
        private static string GetFolder()
        {
            var exeName = Assembly.GetExecutingAssembly().Location;
            var exePath = Path.GetDirectoryName(exeName);
            if (string.IsNullOrEmpty(exePath)) return string.Empty;
            if (!Directory.Exists(exePath)) return string.Empty;
            return exePath;
        }
        [ExcludeFromCodeCoverage(Justification = "Performs file i/o operations")]
        private static string GetHistoryFolder()
        {
            const string folderName = "_history";
            var parent = AppFolder;
            if (!Directory.Exists(parent)) return string.Empty;
            var child = Path.Combine(parent, folderName);
            if (!Directory.Exists(child)) Directory.CreateDirectory(child);
            return child;
        }

        [ExcludeFromCodeCoverage(Justification = "Performs file i/o operations")]
        private static string GetHistoryFile()
        {
            const string fileName = "user-history.txt";
            var parent = HistoryFolder;
            if (!Directory.Exists(parent)) return string.Empty;
            var child = Path.Combine(parent, fileName);
            if (!File.Exists(child)) return string.Empty;
            return child;
        }

        [ExcludeFromCodeCoverage(Justification = "Performs file i/o operations")]
        private static string GetRestrictionFile()
        {
            const string fileName = "user-restriction.txt";
            var parent = HistoryFolder;
            if (!Directory.Exists(parent)) return string.Empty;
            var child = Path.Combine(parent, fileName);
            if (!File.Exists(child)) return string.Empty;
            return child;
        }

        [ExcludeFromCodeCoverage(Justification = "Performs file i/o operations")]
        private static string GetFilterFile()
        {
            const string fileName = "user-search-filter.txt";
            var parent = HistoryFolder;
            if (!Directory.Exists(parent)) return string.Empty;
            var child = Path.Combine(parent, fileName);
            if (!File.Exists(child)) return string.Empty;
            return child;
        }

        [ExcludeFromCodeCoverage(Justification = "Performs file i/o operations")]
        private static void ClearFileContent(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) { return; }
            lock (sync)
            {
                File.Delete(fileName);
                File.WriteAllText(fileName, string.Empty);
            }
        }
    }
}
