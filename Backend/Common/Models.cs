namespace TeamLink.Backend.Common;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = "";
    public string Surname { get; set; } = "";
    public string Email { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public string? Bio { get; set; }
    public string? AvatarUrl { get; set; }
    public int FailedLoginCount { get; set; }
    public DateTime? LockedUntil { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
public class Session
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public string RefreshTokenHash { get; set; } = "";
    public string Platform { get; set; } = "WEB";
    public DateTime ExpiresAt { get; set; }
    public DateTime? RevokedAt { get; set; }
}
public class Conversation
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Type { get; set; } = "PRIVATE";
    public string? Name { get; set; }
    public Guid CreatedById { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastMessageAt { get; set; }
}
public class ConversationParticipant
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ConversationId { get; set; }
    public Guid UserId { get; set; }
    public string Role { get; set; } = "MEMBER";
    public Guid? AddedById { get; set; }
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    public DateTime HistoryFrom { get; set; } = DateTime.MinValue;
    public DateTime? LeftAt { get; set; }
}
