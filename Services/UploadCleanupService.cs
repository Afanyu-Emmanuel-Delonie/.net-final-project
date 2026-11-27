using FERW.Data;
using Microsoft.EntityFrameworkCore;

namespace FERW.Services;

// Submissions write their files before the database save. If the process stops in between, the files have no
// database row pointing at them. This job periodically deletes stored files that nothing references, once they
// are older than a grace period, so in-flight submissions are never touched.
public class UploadCleanupService : BackgroundService
{
    public static readonly TimeSpan GracePeriod = TimeSpan.FromHours(6);
    private static readonly TimeSpan Interval = TimeSpan.FromHours(1);

    private readonly IServiceScopeFactory _scopes;
    private readonly IFileStorage _storage;
    private readonly TimeProvider _clock;
    private readonly ILogger<UploadCleanupService> _log;

    public UploadCleanupService(IServiceScopeFactory scopes, IFileStorage storage, TimeProvider clock, ILogger<UploadCleanupService> log)
    {
        _scopes = scopes;
        _storage = storage;
        _clock = clock;
        _log = log;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(Interval, _clock);
        do
        {
            try
            {
                await RunOnceAsync(stoppingToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _log.LogError(ex, "Upload cleanup failed; will retry next interval.");
            }
        }
        while (await timer.WaitForNextTickAsync(stoppingToken));
    }

    public async Task<int> RunOnceAsync(CancellationToken ct)
    {
        var cutoff = _clock.GetUtcNow().UtcDateTime - GracePeriod;
        var candidates = _storage.ListFiles().Where(f => f.LastWriteUtc < cutoff).Select(f => f.RelativePath).ToList();
        if (candidates.Count == 0)
            return 0;

        using var scope = _scopes.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var referenced = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        referenced.UnionWith(await db.PlayerDocuments.Select(d => d.StoredPath).ToListAsync(ct));
        referenced.UnionWith(await db.Players.Where(p => p.PhotoPath != null).Select(p => p.PhotoPath!).ToListAsync(ct));

        var deleted = 0;
        foreach (var path in candidates.Where(p => !referenced.Contains(p)))
        {
            _storage.Delete(path);
            deleted++;
        }
        if (deleted > 0)
            _log.LogInformation("Upload cleanup removed {Count} unreferenced file(s).", deleted);
        return deleted;
    }
}
