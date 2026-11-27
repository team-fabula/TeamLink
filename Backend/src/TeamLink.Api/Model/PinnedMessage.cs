using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TeamLink.Api.Model;

[Table("PinnedMessages")]
[Index(nameof(MessageId), IsUnique = true)]
public sealed class PinnedMessage
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid ConversationId { get; set; }
    public Guid MessageId { get; set; }
    public Guid PinnedById { get; set; }
    public DateTime PinnedAt { get; set; }

    [ForeignKey(nameof(ConversationId))]
    [InverseProperty(nameof(Conversation.PinnedMessages))]
    public Conversation Conversation { get; set; } = null!;

    [ForeignKey(nameof(MessageId))]
    [InverseProperty(nameof(Message.PinnedMessage))]
    public Message Message { get; set; } = null!;

    [ForeignKey(nameof(PinnedById))]
    [InverseProperty(nameof(User.PinnedMessages))]
    public User PinnedBy { get; set; } = null!;
}
