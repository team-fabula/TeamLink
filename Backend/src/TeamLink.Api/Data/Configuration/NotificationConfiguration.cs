using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeamLink.Api.Common;
using TeamLink.Api.Model;
using TeamLink.Api.Schema.Enum;

namespace TeamLink.Api.Data.Configuration;

public sealed class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.HasEnumCheckConstraint<Notification, NotificationType>(nameof(Notification.Type));
        builder.HasOne(notification => notification.Conversation)
            .WithMany(conversation => conversation.Notifications)
            .OnDelete(DeleteBehavior.SetNull);
        builder.HasOne(notification => notification.Message)
            .WithMany(message => message.Notifications)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
