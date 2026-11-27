using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using TeamLink.Api.Schema.Enum;

namespace TeamLink.Api.Model;

[Index(nameof(ConversationId), nameof(CreatedAt))]
[Index(nameof(SenderId))]
public sealed class Message
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid ConversationId { get; set; }
    public Guid SenderId { get; set; }
    public MessageType Type { get; set; }

    [Column(TypeName = "text")]
    public string? Content { get; set; }

    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? EditedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    [ForeignKey(nameof(ConversationId))]
    [InverseProperty(nameof(Conversation.Messages))]
    public Conversation Conversation { get; set; } = null!;

    [ForeignKey(nameof(SenderId))]
    [InverseProperty(nameof(User.SentMessages))]
    public User Sender { get; set; } = null!;

    [InverseProperty(nameof(Attachment.Message))]
    public ICollection<Attachment> Attachments { get; } = [];

    [InverseProperty(nameof(Reaction.Message))]
    public ICollection<Reaction> Reactions { get; } = [];

    [InverseProperty(nameof(MessageReceipt.Message))]
    public ICollection<MessageReceipt> Receipts { get; } = [];

    [InverseProperty(nameof(PinnedMessage.Message))]
    public PinnedMessage? PinnedMessage { get; set; }

    [InverseProperty(nameof(Notification.Message))]
    public ICollection<Notification> Notifications { get; } = [];
}
