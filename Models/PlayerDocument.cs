namespace FERW.Models;

public class PlayerDocument
{
    public int Id { get; set; }

    public int PlayerRegistrationId { get; set; }
    public PlayerRegistration PlayerRegistration { get; set; } = default!;

    public DocumentType Type { get; set; }
    public string OriginalFileName { get; set; } = "";
    public string StoredPath { get; set; } = "";
    public string ContentType { get; set; } = "";
    public long SizeBytes { get; set; }
    public DateTime UploadedAt { get; set; }
}
