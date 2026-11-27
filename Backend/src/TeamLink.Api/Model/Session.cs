using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using TeamLink.Api.Schema.Enum;

namespace TeamLink.Api.Model;

[Index(nameof(UserId), nameof(ExpiresAt))]
public sealed class Session
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid UserId { get; set; }

    [Required]
    public required string RefreshTokenHash { get; set; }

    public SessionPlatform Platform { get; set; }

    [Required]
    public required string DeviceInfo { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime? RevokedAt { get; set; }

    [ForeignKey(nameof(UserId))]
    [InverseProperty(nameof(User.Sessions))]
    public User User { get; set; } = null!;
}
