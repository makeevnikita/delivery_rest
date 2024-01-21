namespace delivery.Helpers;

public class FileHelper
{
    public static readonly string ImagesFolder = "images";

    public static readonly string ProductFolder = "products";

    public async Task CreateFileAsync(IFormFile file, string folderPath, string fileName)
    {
        DirectoryInfo directory = new DirectoryInfo(folderPath);
        if (!directory.Exists)
        {
            directory.Create();
        }

        using (
            FileStream stream = new FileStream(Path.Combine(folderPath, fileName),
            FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }
    }

    public async Task DeleteFileAsync(string filePath)
    {
        FileInfo file = new FileInfo(filePath);
        if (file.Exists)
        {
            file.Delete();
        }
    }
}