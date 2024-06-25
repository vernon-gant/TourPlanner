namespace TP.Report;

public abstract class FileHandler
{
    protected string _filePath = string.Empty;

    public string FilePath => _filePath;

    public abstract void GenerateFile();

    public abstract FileStream GetFileStream();

    public void DeleteFile()
    {
        File.Delete(_filePath);
    }
}