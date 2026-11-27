using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeamLink.Api.Common;
using TeamLink.Api.Model;
using TeamLink.Api.Schema.Enum;

namespace TeamLink.Api.Data.Configuration;

public sealed class ReactionConfiguration : IEntityTypeConfiguration<Reaction>
{
    public void Configure(EntityTypeBuilder<Reaction> builder)
    {
        builder.HasEnumCheckConstraint<Reaction, ReactionType>(nameof(Reaction.ReactionType));
        builder.HasOne(reaction => reaction.User)
            .WithMany(user => user.Reactions)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
