using next.core.interfaces;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace next.core.implementations
{
    internal class MailPersistence : IMailPersistence
    {
        private readonly IFileInteraction _fileService;
        public MailPersistence(IFileInteraction? fileService)
        {
            _fileService = fileService ?? new FileInteraction();
        }
        public void Clear()
        {
            var fileName = MailFile;
            var folderName = MailSubFolder;
            ClearFileContent(fileName);
            ClearChildContent(folderName);
        }

        public void Save(string json)
        {
            var fileName = MailFile;
            lock (sync)
            {
                _fileService.WriteAllText(fileName, json);
            }
        }
        public void Save(string id, string json)
        {
            var folderName = MailSubFolder;
            _fileService.WriteAllText(folderName, id, json);
        }

        public bool DoesItemExist(string id)
        {
            var folderName = MailSubFolder;
            return _fileService.DoesItemExist(folderName, id);
        }

        public string? Fetch()
        {
            var fileName = MailFile;
            lock (sync)
            {
                return _fileService.ReadAllText(fileName);
            }
        }

        public string? Fetch(string id)
        {
            var folderName = MailSubFolder;
            return _fileService.ReadAllText(folderName, id);
        }

        private static readonly object sync = new();
        private static string AppFolder => appFolder ??= GetFolder();
        private static string MailFolder => mailFolder ??= GetMailFolder();
        private static string MailSubFolder => mailSubFolder ??= GetMailSubFolder();
        private static string MailFile => mailFile ??= GetMailFile();

        private static string? appFolder;
        private static string? mailFolder;
        private static string? mailFile;
        private static string? mailSubFolder;

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
        private static string GetMailFolder()
        {
            const string folderName = "_mailbox";
            var parent = AppFolder;
            if (!Directory.Exists(parent)) return string.Empty;
            var child = Path.Combine(parent, folderName);
            if (!Directory.Exists(child)) return string.Empty;
            return child;
        }
        [ExcludeFromCodeCoverage(Justification = "Performs file i/o operations")]
        private static string GetMailSubFolder()
        {
            const string folderName = "_letters";
            var parent = MailFolder;
            if (!Directory.Exists(parent)) return string.Empty;
            var child = Path.Combine(parent, folderName);
            if (!Directory.Exists(child)) Directory.CreateDirectory(child);
            return child;
        }
        [ExcludeFromCodeCoverage(Justification = "Performs file i/o operations")]
        private static string GetMailFile()
        {
            const string folderName = "user-data.txt";
            var parent = MailFolder;
            if (!Directory.Exists(parent)) return string.Empty;
            var child = Path.Combine(parent, folderName);
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
        [ExcludeFromCodeCoverage(Justification = "Performs file i/o operations")]
        private static void ClearChildContent(string folderName)
        {
            if (string.IsNullOrEmpty(folderName)) { return; }
            lock (sync)
            {
                var di = new DirectoryInfo(folderName);
                di.Delete(true);
                if (!Directory.Exists(folderName)) Directory.CreateDirectory(folderName);
            }
        }
    }
}
