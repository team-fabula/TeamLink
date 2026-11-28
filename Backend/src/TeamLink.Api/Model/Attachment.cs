using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TeamLink.Api.Model;

[Table("Attachments")]
[Index(nameof(MessageId))]
public sealed class Attachment
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid MessageId { get; set; }

    [Required]
    public required string FileName { get; set; }

    [Required]
    public required string MimeType { get; set; }

    [Range(0, long.MaxValue)]
    public long SizeBytes { get; set; }

    [Required]
    public required string StorageKey { get; set; }

    public DateTime UploadedAt { get; set; }

    [ForeignKey(nameof(MessageId))]
    [InverseProperty(nameof(Message.Attachments))]
    public Message Message { get; set; } = null!;
}
