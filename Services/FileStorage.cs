namespace FERW.Services;

public interface IFileStorage
{
    // Saves the file under the given folder and returns its path relative to the storage root.
    Task<string> SaveAsync(IFormFile file, string folder, CancellationToken ct = default);
    void Delete(string relativePath);

    // Every stored file with its last-write time, for the orphan cleanup job.
    IEnumerable<(string RelativePath, DateTime LastWriteUtc)> ListFiles();
}

// Stores uploads on disk, outside wwwroot, so documents are never served publicly.
// The root comes from "Storage:UploadsPath". Outside Development it must be set to durable storage that
// every app instance shares (for example a mounted network volume); in Development it defaults to App_Data/uploads.
public class LocalFileStorage : IFileStorage
{
    private readonly string _root;

    public LocalFileStorage(IWebHostEnvironment env, IConfiguration config)
    {
        var configured = config["Storage:UploadsPath"];
        if (string.IsNullOrWhiteSpace(configured))
        {
            if (!env.IsDevelopment())
                throw new InvalidOperationException(
                    "Storage:UploadsPath is not configured. Point it at durable storage shared by all app instances.");
            configured = Path.Combine(env.ContentRootPath, "App_Data", "uploads");
        }

        _root = Path.TrimEndingDirectorySeparator(Path.GetFullPath(configured)) + Path.DirectorySeparatorChar;
        Directory.CreateDirectory(_root);
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

    public IEnumerable<(string RelativePath, DateTime LastWriteUtc)> ListFiles()
    {
        foreach (var full in Directory.EnumerateFiles(_root, "*", SearchOption.AllDirectories))
        {
            var relative = Path.GetRelativePath(_root, full).Replace(Path.DirectorySeparatorChar, '/');
            yield return (relative, File.GetLastWriteTimeUtc(full));
        }
    }
}
