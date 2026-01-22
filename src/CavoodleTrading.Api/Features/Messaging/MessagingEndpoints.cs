using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using CavoodleTrading.Api.Domain.Entities;
using CavoodleTrading.Api.Infrastructure.Data;

namespace CavoodleTrading.Api.Features.Messaging;

public static class MessagingEndpoints
{
    public static void MapMessagingEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/messages").WithTags("Messaging");

        group.MapGet("/conversations", GetConversations).RequireAuthorization();
        group.MapGet("/conversations/{id:guid}", GetConversation).RequireAuthorization();
        group.MapPost("/conversations", StartConversation).RequireAuthorization();
        group.MapPost("/conversations/{id:guid}/messages", SendMessage).RequireAuthorization();
    }

    private static async Task<IResult> GetConversations(
        ClaimsPrincipal principal,
        CavoodleDbContext db)
    {
        var userId = Guid.Parse(principal.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var conversations = await db.ConversationParticipants
            .Where(cp => cp.UserId == userId)
            .Include(cp => cp.Conversation)
                .ThenInclude(c => c.Listing)
            .Include(cp => cp.Conversation)
                .ThenInclude(c => c.Participants)
                    .ThenInclude(p => p.User)
            .Include(cp => cp.Conversation)
                .ThenInclude(c => c.Messages.OrderByDescending(m => m.CreatedAt).Take(1))
            .Select(cp => new ConversationDto
            {
                Id = cp.Conversation.Id,
                ListingName = cp.Conversation.Listing != null ? cp.Conversation.Listing.Name : "Deleted Listing",
                OtherParticipant = cp.Conversation.Participants
                    .Where(p => p.UserId != userId)
                    .Select(p => p.User.DisplayName)
                    .FirstOrDefault() ?? "Unknown",
                LastMessage = cp.Conversation.Messages
                    .OrderByDescending(m => m.CreatedAt)
                    .Select(m => m.Content)
                    .FirstOrDefault(),
                LastMessageAt = cp.Conversation.LastMessageAt,
                UnreadCount = cp.Conversation.Messages.Count(m => !m.IsRead && m.SenderId != userId)
            })
            .OrderByDescending(c => c.LastMessageAt)
            .ToListAsync();

        return Results.Ok(conversations);
    }

    private static async Task<IResult> GetConversation(
        Guid id,
        ClaimsPrincipal principal,
        CavoodleDbContext db)
    {
        var userId = Guid.Parse(principal.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var isParticipant = await db.ConversationParticipants
            .AnyAsync(cp => cp.ConversationId == id && cp.UserId == userId);

        if (!isParticipant)
        {
            return Results.Forbid();
        }

        var messages = await db.Messages
            .Where(m => m.ConversationId == id)
            .OrderBy(m => m.CreatedAt)
            .Select(m => new MessageDto
            {
                Id = m.Id,
                SenderId = m.SenderId,
                SenderName = m.Sender.DisplayName,
                Content = m.Content,
                IsRead = m.IsRead,
                CreatedAt = m.CreatedAt,
                IsMine = m.SenderId == userId
            })
            .ToListAsync();

        // Mark unread messages as read
        await db.Messages
            .Where(m => m.ConversationId == id && !m.IsRead && m.SenderId != userId)
            .ExecuteUpdateAsync(m => m.SetProperty(x => x.IsRead, true));

        return Results.Ok(messages);
    }

    private static async Task<IResult> StartConversation(
        StartConversationRequest request,
        ClaimsPrincipal principal,
        CavoodleDbContext db)
    {
        var userId = Guid.Parse(principal.FindFirstValue(ClaimTypes.NameIdentifier)!);

        // Check if conversation already exists for this listing between these users
        var existingConversation = await db.ConversationParticipants
            .Where(cp => cp.UserId == userId)
            .Select(cp => cp.Conversation)
            .Where(c => c.ListingId == request.ListingId)
            .Where(c => c.Participants.Any(p => p.UserId == request.SellerId))
            .FirstOrDefaultAsync();

        if (existingConversation != null)
        {
            return Results.Ok(new { ConversationId = existingConversation.Id });
        }

        // Create new conversation
        var conversation = new Conversation
        {
            Id = Guid.NewGuid(),
            ListingId = request.ListingId,
            CreatedAt = DateTime.UtcNow,
            LastMessageAt = DateTime.UtcNow
        };

        db.Conversations.Add(conversation);

        // Add participants
        db.ConversationParticipants.Add(new ConversationParticipant
        {
            ConversationId = conversation.Id,
            UserId = userId
        });

        db.ConversationParticipants.Add(new ConversationParticipant
        {
            ConversationId = conversation.Id,
            UserId = request.SellerId
        });

        // Add initial message if provided
        if (!string.IsNullOrEmpty(request.InitialMessage))
        {
            db.Messages.Add(new Message
            {
                Id = Guid.NewGuid(),
                ConversationId = conversation.Id,
                SenderId = userId,
                Content = request.InitialMessage,
                CreatedAt = DateTime.UtcNow
            });
        }

        await db.SaveChangesAsync();

        return Results.Created($"/api/messages/conversations/{conversation.Id}", new { ConversationId = conversation.Id });
    }

    private static async Task<IResult> SendMessage(
        Guid id,
        SendMessageRequest request,
        ClaimsPrincipal principal,
        CavoodleDbContext db,
        IHubContext<MessagingHub> hubContext)
    {
        var userId = Guid.Parse(principal.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var isParticipant = await db.ConversationParticipants
            .AnyAsync(cp => cp.ConversationId == id && cp.UserId == userId);

        if (!isParticipant)
        {
            return Results.Forbid();
        }

        var message = new Message
        {
            Id = Guid.NewGuid(),
            ConversationId = id,
            SenderId = userId,
            Content = request.Content,
            CreatedAt = DateTime.UtcNow
        };

        db.Messages.Add(message);

        // Update conversation last message time
        await db.Conversations
            .Where(c => c.Id == id)
            .ExecuteUpdateAsync(c => c.SetProperty(x => x.LastMessageAt, DateTime.UtcNow));

        await db.SaveChangesAsync();

        // Get sender name for SignalR notification
        var senderName = await db.Users
            .Where(u => u.Id == userId)
            .Select(u => u.DisplayName)
            .FirstOrDefaultAsync();

        // Notify other participants via SignalR
        await hubContext.Clients.Group($"conversation_{id}").SendAsync("NewMessage", new
        {
            message.Id,
            message.SenderId,
            SenderName = senderName,
            message.Content,
            message.CreatedAt
        });

        return Results.Ok(new { MessageId = message.Id });
    }
}

public class MessagingHub : Hub
{
    public async Task JoinConversation(string conversationId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"conversation_{conversationId}");
    }

    public async Task LeaveConversation(string conversationId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"conversation_{conversationId}");
    }
}

public record ConversationDto
{
    public Guid Id { get; init; }
    public string ListingName { get; init; } = string.Empty;
    public string OtherParticipant { get; init; } = string.Empty;
    public string? LastMessage { get; init; }
    public DateTime LastMessageAt { get; init; }
    public int UnreadCount { get; init; }
}

public record MessageDto
{
    public Guid Id { get; init; }
    public Guid SenderId { get; init; }
    public string SenderName { get; init; } = string.Empty;
    public string Content { get; init; } = string.Empty;
    public bool IsRead { get; init; }
    public DateTime CreatedAt { get; init; }
    public bool IsMine { get; init; }
}

public record StartConversationRequest(Guid ListingId, Guid SellerId, string? InitialMessage = null);
public record SendMessageRequest(string Content);
