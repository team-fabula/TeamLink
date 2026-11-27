using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeamLink.Api.Common;
using TeamLink.Api.Model;
using TeamLink.Api.Schema.Enum;

namespace TeamLink.Api.Data.Configuration;

public sealed class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.ToTable(table => table.HasCheckConstraint(
            "CK_Messages_DeletionState",
            "(\"IsDeleted\" = false AND \"DeletedAt\" IS NULL) OR " +
            "(\"IsDeleted\" = true AND \"DeletedAt\" IS NOT NULL AND \"Content\" IS NULL)"
        ));
        builder.HasEnumCheckConstraint<Message, MessageType>(nameof(Message.Type));
        builder.HasIndex(message => message.Content)
            .HasDatabaseName("IX_Messages_Content_Active_Trigram")
            .HasMethod("gin")
            .HasOperators("gin_trgm_ops")
            .HasFilter("\"IsDeleted\" = false AND \"Content\" IS NOT NULL");
        builder.HasOne(message => message.Sender)
            .WithMany(user => user.SentMessages)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
