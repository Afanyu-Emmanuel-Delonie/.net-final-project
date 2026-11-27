namespace FERW.Services;

public interface IFileStorage
{
    // Saves the file under the given folder and returns its path relative to the storage root.
    Task<string> SaveAsync(IFormFile file, string folder, CancellationToken ct = default);
    void Delete(string relativePath);
}

// Stores uploads under App_Data, outside wwwroot, so documents are never served publicly.
public class LocalFileStorage : IFileStorage
{
    private readonly string _root;

    public LocalFileStorage(IWebHostEnvironment env)
    {
        _root = Path.Combine(env.ContentRootPath, "App_Data", "uploads");
    }

    public async Task<string> SaveAsync(IFormFile file, string folder, CancellationToken ct = default)
    {
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        var relative = Path.Combine(folder, $"{Guid.NewGuid():N}{ext}");
        var full = Path.Combine(_root, relative);
        Directory.CreateDirectory(Path.GetDirectoryName(full)!);

        try
        {
            await using var stream = new FileStream(full, FileMode.CreateNew);
            await file.CopyToAsync(stream, ct);
        }
        catch
        {
            // Don't leave a partial file behind; the caller never learns this path.
            File.Delete(full);
            throw;
        }
        return relative.Replace(Path.DirectorySeparatorChar, '/');
    }

    public void Delete(string relativePath)
    {
        var full = Path.GetFullPath(Path.Combine(_root, relativePath));
        if (full.StartsWith(_root, StringComparison.OrdinalIgnoreCase) && File.Exists(full))
            File.Delete(full);
    }
}
