using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TeamLink.Api.Model;

[Table("MessageReceipts")]
[Index(nameof(MessageId), nameof(UserId), IsUnique = true)]
[Index(nameof(UserId))]
public sealed class MessageReceipt
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid MessageId { get; set; }
    public Guid UserId { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public DateTime? ReadAt { get; set; }

    [ForeignKey(nameof(MessageId))]
    [InverseProperty(nameof(Message.Receipts))]
    public Message Message { get; set; } = null!;

    [ForeignKey(nameof(UserId))]
    [InverseProperty(nameof(User.MessageReceipts))]
    public User User { get; set; } = null!;
}
