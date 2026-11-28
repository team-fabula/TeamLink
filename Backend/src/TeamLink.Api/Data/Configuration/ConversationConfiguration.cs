using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeamLink.Api.Common;
using TeamLink.Api.Model;
using TeamLink.Api.Schema.Enum;

namespace TeamLink.Api.Data.Configuration;

public sealed class ConversationConfiguration : IEntityTypeConfiguration<Conversation>
{
    public void Configure(EntityTypeBuilder<Conversation> builder)
    {
        builder.ToTable(table => table.HasCheckConstraint(
            "CK_Conversations_GroupFields",
            "\"Type\" = 'GROUP' OR (\"Name\" IS NULL AND \"AvatarUrl\" IS NULL)"
        ));
        builder.HasEnumCheckConstraint<Conversation, ConversationType>(nameof(Conversation.Type));
        builder.HasOne(conversation => conversation.CreatedBy)
            .WithMany(user => user.CreatedConversations)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
