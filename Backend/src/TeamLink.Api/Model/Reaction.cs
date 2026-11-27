using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using TeamLink.Api.Schema.Enum;

namespace TeamLink.Api.Model;

[Table("Reactions")]
[Index(nameof(MessageId), nameof(UserId), IsUnique = true)]
public sealed class Reaction
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid MessageId { get; set; }
    public Guid? UserId { get; set; }
    public ReactionType ReactionType { get; set; }
    public DateTime CreatedAt { get; set; }

    [ForeignKey(nameof(MessageId))]
    [InverseProperty(nameof(Message.Reactions))]
    public Message Message { get; set; } = null!;

    [ForeignKey(nameof(UserId))]
    [InverseProperty(nameof(User.Reactions))]
    public User? User { get; set; }
}
