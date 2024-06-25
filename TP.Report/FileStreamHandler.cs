namespace TP.Report;

public class FileStreamHandler : FileHandler
{
    public override void GenerateFile()
    {
        string tempPath = Path.GetTempPath();
        string tempFileName = $"{Guid.NewGuid()}.pdf";
        _filePath = Path.Combine(tempPath, tempFileName);
    }

    public override FileStream GetFileStream()
    {
        return new FileStream(_filePath, FileMode.Open);
    }
}