using Microsoft.EntityFrameworkCore;
using TeamLink.Backend.Common;

namespace TeamLink.Backend.Conversations;

public record CreatePrivateConversationRequest(Guid RecipientId);

public sealed class ConversationService(TeamLinkDb db)
{
    public async Task<object> CreatePrivate(Guid userId, CreatePrivateConversationRequest request)
    {
        if (userId == request.RecipientId)
            throw new ApiException(400, "INVALID_RECIPIENT", "You cannot start a conversation with yourself.");
        if (!await db.Users.AnyAsync(x => x.Id == request.RecipientId))
            throw new ApiException(404, "USER_NOT_FOUND", "Recipient does not exist.");

        var existing = await db.Conversations
            .Where(c => c.Type == "PRIVATE"
                && db.Participants.Any(p => p.ConversationId == c.Id && p.UserId == userId && p.LeftAt == null)
                && db.Participants.Any(p => p.ConversationId == c.Id && p.UserId == request.RecipientId && p.LeftAt == null))
            .FirstOrDefaultAsync();
        if (existing is not null) return new { existing.Id, existing.Type, Created = false };

        var conversation = new Conversation { Type = "PRIVATE", CreatedById = userId };
        db.Conversations.Add(conversation);
        db.Participants.AddRange(
            new ConversationParticipant { ConversationId = conversation.Id, UserId = userId, AddedById = userId },
            new ConversationParticipant { ConversationId = conversation.Id, UserId = request.RecipientId, AddedById = userId });
        await db.SaveChangesAsync();
        return new { conversation.Id, conversation.Type, Created = true };
    }

    public async Task<object> List(Guid userId) => await db.Participants
        .Where(p => p.UserId == userId && p.LeftAt == null)
        .Join(db.Conversations, p => p.ConversationId, c => c.Id,
            (p, c) => new { c.Id, c.Type, c.CreatedAt })
        .OrderByDescending(c => c.CreatedAt).ToListAsync();

    public async Task<object> Participants(Guid conversationId, Guid userId)
    {
        if (!await db.Participants.AnyAsync(p => p.ConversationId == conversationId && p.UserId == userId && p.LeftAt == null))
            throw new ApiException(403, "NOT_PARTICIPANT", "You are not a participant of this conversation.");
        return await db.Participants.Where(p => p.ConversationId == conversationId && p.LeftAt == null)
            .Join(db.Users, p => p.UserId, u => u.Id,
                (p, u) => new { u.Id, u.Name, u.Surname }).ToListAsync();
    }
}
