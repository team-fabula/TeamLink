using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using TeamLink.Api.Schema.Enum;

namespace TeamLink.Api.Model;

[Index(nameof(ConversationId), nameof(UserId), IsUnique = true)]
[Index(nameof(UserId))]
public sealed class ConversationParticipant
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid ConversationId { get; set; }
    public Guid UserId { get; set; }
    public ConversationParticipantRole Role { get; set; }
    public Guid? AddedById { get; set; }
    public DateTime JoinedAt { get; set; }
    public DateTime? LeftAt { get; set; }
    public Guid? RemovedById { get; set; }
    public Guid? LastReadMessageId { get; set; }
    public DateTime? LastReadAt { get; set; }

    [ForeignKey(nameof(ConversationId))]
    [InverseProperty(nameof(Conversation.Participants))]
    public Conversation Conversation { get; set; } = null!;

    [ForeignKey(nameof(UserId))]
    [InverseProperty(nameof(User.ConversationMemberships))]
    public User User { get; set; } = null!;

    [ForeignKey(nameof(AddedById))]
    public User? AddedBy { get; set; }

    [ForeignKey(nameof(RemovedById))]
    public User? RemovedBy { get; set; }

    [ForeignKey(nameof(LastReadMessageId))]
    public Message? LastReadMessage { get; set; }
}
