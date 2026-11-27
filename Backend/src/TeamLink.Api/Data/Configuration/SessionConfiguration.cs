using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeamLink.Api.Model;
using TeamLink.Api.Schema.Enum;
using TeamLink.Api.Common;

namespace TeamLink.Api.Data.Configuration;

public sealed class SessionConfiguration : IEntityTypeConfiguration<Session>
{
    public void Configure(EntityTypeBuilder<Session> builder)
    {
        builder.HasEnumCheckConstraint<Session, SessionPlatform>(nameof(Session.Platform));
    }
}
