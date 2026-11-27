using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using TeamLink.Api.Schema.Enum;

namespace TeamLink.Api.Model;

[Index(nameof(LastMessageAt))]
public sealed class Conversation
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    public ConversationType Type { get; set; }
    public string? Name { get; set; }
    public string? AvatarUrl { get; set; }
    public Guid CreatedById { get; set; }
    public DateTime? LastMessageAt { get; set; }
    public DateTime CreatedAt { get; set; }

    [ForeignKey(nameof(CreatedById))]
    [InverseProperty(nameof(User.CreatedConversations))]
    public User CreatedBy { get; set; } = null!;

    [InverseProperty(nameof(ConversationParticipant.Conversation))]
    public ICollection<ConversationParticipant> Participants { get; } = [];

    [InverseProperty(nameof(Message.Conversation))]
    public ICollection<Message> Messages { get; } = [];

    [InverseProperty(nameof(PinnedMessage.Conversation))]
    public ICollection<PinnedMessage> PinnedMessages { get; } = [];

    [InverseProperty(nameof(Notification.Conversation))]
    public ICollection<Notification> Notifications { get; } = [];
}
