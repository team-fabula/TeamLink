using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using TeamLink.Api.Schema.Enum;

namespace TeamLink.Api.Model;

[Table("Notifications")]
[Index(nameof(RecipientId), nameof(IsRead), nameof(CreatedAt))]
public sealed class Notification
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid RecipientId { get; set; }
    public Guid? ConversationId { get; set; }
    public Guid? MessageId { get; set; }
    public NotificationType Type { get; set; }

    [Required]
    public required string Text { get; set; }

    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ReadAt { get; set; }

    [ForeignKey(nameof(RecipientId))]
    [InverseProperty(nameof(User.Notifications))]
    public User Recipient { get; set; } = null!;

    [ForeignKey(nameof(ConversationId))]
    [InverseProperty(nameof(Conversation.Notifications))]
    public Conversation? Conversation { get; set; }

    [ForeignKey(nameof(MessageId))]
    [InverseProperty(nameof(Message.Notifications))]
    public Message? Message { get; set; }
}
