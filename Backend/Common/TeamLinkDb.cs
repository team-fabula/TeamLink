using Microsoft.EntityFrameworkCore;
namespace TeamLink.Backend.Common;
public class TeamLinkDb(DbContextOptions<TeamLinkDb> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Session> Sessions => Set<Session>();
    public DbSet<Conversation> Conversations => Set<Conversation>();
    public DbSet<ConversationParticipant> Participants => Set<ConversationParticipant>();
    protected override void OnModelCreating(ModelBuilder model)
    {
        model.Entity<User>().HasIndex(x => x.Email).IsUnique();
        model.Entity<Session>().HasIndex(x => x.RefreshTokenHash).IsUnique();
        model.Entity<ConversationParticipant>().HasIndex(x => new { x.ConversationId, x.UserId }).IsUnique();
        model.Entity<ConversationParticipant>().HasIndex(x => new { x.UserId, x.LeftAt });
    }
}
