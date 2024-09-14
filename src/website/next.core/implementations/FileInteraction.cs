using next.core.interfaces;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace next.core.implementations
{
    [ExcludeFromCodeCoverage(Justification = "Performs file i/o operations")]
    internal class FileInteraction : IFileInteraction
    {
        public void DeleteFile(string path)
        {
            if (File.Exists(path)) File.Delete(path);
        }
        public bool DoesItemExist(string folderName, string id)
        {
            if (string.IsNullOrEmpty(folderName)) { return false; }
            if (!Guid.TryParse(id, out var indx)) { return false; }
            var suffix = $"{indx:D}.txt";
            var fileName = Path.Combine(folderName, suffix);
            if (string.IsNullOrEmpty(fileName) || !File.Exists(fileName)) { return false; }
            return true;
        }

        public bool FileExists(string path)
        {
            return File.Exists(path);
        }

        public string ReadAllText(string path)
        {
            try
            {
                if (string.IsNullOrEmpty(path)) { return string.Empty; }
                var content = File.ReadAllText(path);
                var array = Convert.FromBase64String(content);
                var converted = Encoding.UTF8.GetString(array);
                return converted;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public string? ReadAllText(string folder, string id)
        {
            if (string.IsNullOrEmpty(folder)) { return null; }
            if (!Guid.TryParse(id, out var indx)) { return null; }
            var suffix = $"{indx:D}.txt";
            var fileName = Path.Combine(folder, suffix);
            if (string.IsNullOrEmpty(fileName) || !File.Exists(fileName)) { return null; }
            try
            {
                using var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using var reader = new StreamReader(stream);
                var content = reader.ReadToEnd();
                var array = Convert.FromBase64String(content);
                var converted = Encoding.UTF8.GetString(array);
                return converted;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public void WriteAllText(string path, string text)
        {
            if (string.IsNullOrEmpty(path)) { return; }
            var array = Encoding.UTF8.GetBytes(text);
            var content = Convert.ToBase64String(array);
            if (File.Exists(path)) File.Delete(path);
            File.WriteAllText(path, content);
        }

        public void WriteAllText(string folder, string id, string text)
        {
            if (string.IsNullOrEmpty(folder)) { return; }
            if (!Guid.TryParse(id, out var indx)) { return; }
            var suffix = $"{indx:D}.txt";
            var fileName = Path.Combine(folder, suffix);
            WriteAllText(fileName, text);
        }
    }
}
