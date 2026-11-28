using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeamLink.Api.Common;
using TeamLink.Api.Model;
using TeamLink.Api.Schema.Enum;

namespace TeamLink.Api.Data.Configuration;

public sealed class ConversationParticipantConfiguration : IEntityTypeConfiguration<ConversationParticipant>
{
    public void Configure(EntityTypeBuilder<ConversationParticipant> builder)
    {
        builder.HasEnumCheckConstraint<ConversationParticipant, ConversationParticipantRole>(nameof(ConversationParticipant.Role));
        builder.HasOne(participant => participant.AddedBy)
            .WithMany()
            .OnDelete(DeleteBehavior.SetNull);
        builder.HasOne(participant => participant.RemovedBy)
            .WithMany()
            .OnDelete(DeleteBehavior.SetNull);
        builder.HasOne(participant => participant.LastReadMessage)
            .WithMany()
            .OnDelete(DeleteBehavior.SetNull);
    }
}
