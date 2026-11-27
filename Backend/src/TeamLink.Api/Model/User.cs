using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TeamLink.Api.Model;

[Index(nameof(Email), IsUnique = true)]
public sealed class User
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public required string FirstName { get; set; }

    [Required]
    public required string LastName { get; set; }

    [Required]
    public required string Email { get; set; }

    [Required]
    public required string PasswordHash { get; set; }

    public string? AvatarUrl { get; set; }
    public string? Bio { get; set; }
    public bool IsOnline { get; set; }
    public DateTime? LastSeenAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    [InverseProperty(nameof(Session.User))]
    public ICollection<Session> Sessions { get; } = [];

    [InverseProperty(nameof(Conversation.CreatedBy))]
    public ICollection<Conversation> CreatedConversations { get; } = [];

    [InverseProperty(nameof(ConversationParticipant.User))]
    public ICollection<ConversationParticipant> ConversationMemberships { get; } = [];

    [InverseProperty(nameof(Message.Sender))]
    public ICollection<Message> SentMessages { get; } = [];

    [InverseProperty(nameof(Reaction.User))]
    public ICollection<Reaction> Reactions { get; } = [];

    [InverseProperty(nameof(PinnedMessage.PinnedBy))]
    public ICollection<PinnedMessage> PinnedMessages { get; } = [];

    [InverseProperty(nameof(MessageReceipt.User))]
    public ICollection<MessageReceipt> MessageReceipts { get; } = [];

    [InverseProperty(nameof(Notification.Recipient))]
    public ICollection<Notification> Notifications { get; } = [];
}
