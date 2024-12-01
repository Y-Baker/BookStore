namespace BookStore.Utils;

public class FileService
{
    public string AddPhoto(IFormFile file, string uploadPath)
    {
        Guid guid = Guid.NewGuid();
        string fileExtension = Path.GetExtension(file.FileName);
        string fileName = $"{guid.ToString()}{fileExtension}";
        string path = Path.Combine(uploadPath, fileName);

        FileStream st = new(path, FileMode.Create, FileAccess.Write);
        file.CopyToAsync(st).Wait();

        return fileName;
    }

    public void RemovePhoto(string id, string uploadPath)
    {
        string path = Path.Combine(uploadPath, id.ToString());
        if (path is not null && System.IO.File.Exists(path))
            System.IO.File.Delete(path);
    }
}
