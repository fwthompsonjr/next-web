namespace next.core.interfaces
{
    public interface IFileInteraction
    {
        bool FileExists(string path);
        void DeleteFile(string path);
        bool DoesItemExist(string folderName, string id);
        string ReadAllText(string path);
        string? ReadAllText(string folder, string id);
        void WriteAllText(string path, string text);
        void WriteAllText(string folder, string id, string text);

    }
}
