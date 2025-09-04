using System;
using System.Collections.Generic;
using System.Linq;
using Core.DTO.Message;
using Core.Models.Message;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ConversationRepository : GenericRepository<Conversation>
    {
        public ConversationRepository(DbContextClass context) : base(context)
        {
        }

        public ConversationsPagedDTO GetPagedConversations(ConversationFiltersDTO filters, int page = 1, int limit = 20)
        {
            var query = _dbContext.Conversations
                .Include(c => c.Participants)
                    .ThenInclude(p => p.User)
                .Include(c => c.LastMessage)
                    .ThenInclude(m => m.Sender)
                .AsQueryable();

            if (filters.Type.HasValue)
                query = query.Where(c => c.Type == filters.Type.Value);

            if (filters.IsArchived.HasValue)
                query = query.Where(c => c.IsArchived == filters.IsArchived.Value);

            if (filters.IsMuted.HasValue)
                query = query.Where(c => c.IsMuted == filters.IsMuted.Value);

            if (filters.HasUnread.HasValue && filters.HasUnread.Value)
                query = query.Where(c => c.UnreadCount > 0);

            if (filters.ParticipantId.HasValue)
                query = query.Where(c => c.Participants.Any(p => p.UserId == filters.ParticipantId.Value));

            if (filters.EmpresasId.HasValue)
                query = query.Where(c => c.EmpresasId == filters.EmpresasId.Value);

            if (!string.IsNullOrEmpty(filters.Search))
                query = query.Where(c => c.Title.Contains(filters.Search) || c.Description.Contains(filters.Search));

            var total = query.Count();
            var conversations = query
                .OrderByDescending(c => c.UpdatedDate)
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToList();

            return new ConversationsPagedDTO
            {
                Conversations = conversations.Select(MapToConversationResponse).ToList(),
                HasMore = total > page * limit,
                Total = total,
                Page = page,
                Limit = limit
            };
        }

        public void UpdateLastMessage(int conversationId, int messageId)
        {
            var conversation = _dbContext.Conversations.FirstOrDefault(c => c.Id == conversationId);
            if (conversation != null)
            {
                conversation.LastMessageId = messageId;
                conversation.UpdatedDate = DateTime.UtcNow;
                Update(conversation);
            }
        }

        public void UpdateUnreadCount(int conversationId, int userId, int count)
        {
            var conversation = _dbContext.Conversations.FirstOrDefault(c => c.Id == conversationId);
            if (conversation != null)
            {
                conversation.UnreadCount = count;
                Update(conversation);
            }
        }

        private ConversationResponse MapToConversationResponse(Conversation conversation)
        {
            return new ConversationResponse
            {
                Id = conversation.Id,
                EmpresasId = conversation.EmpresasId,
                Type = conversation.Type,
                Title = conversation.Title,
                Description = conversation.Description,
                IsArchived = conversation.IsArchived,
                IsMuted = conversation.IsMuted,
                UnreadCount = conversation.UnreadCount,
                CreatedAt = conversation.CreatedDate,
                UpdatedAt = conversation.UpdatedDate,
                Participants = conversation.Participants.Select(p => new ConversationParticipantResponse
                {
                    UserId = p.UserId,
                    UserName = p.User?.Name ?? "",
                    Role = p.Role,
                    JoinedAt = p.JoinedAt,
                    LastSeenAt = p.LastSeenAt,
                    IsOnline = p.IsOnline,
                    Permissions = new ParticipantPermissionsResponse
                    {
                        CanSendMessages = p.Permissions.CanSendMessages,
                        CanSendAttachments = p.Permissions.CanSendAttachments,
                        CanDeleteMessages = p.Permissions.CanDeleteMessages,
                        CanAddParticipants = p.Permissions.CanAddParticipants,
                        CanRemoveParticipants = p.Permissions.CanRemoveParticipants,
                        CanEditConversation = p.Permissions.CanEditConversation
                    }
                }).ToList(),
                LastMessage = conversation.LastMessage != null ? new MessageResponse
                {
                    Id = conversation.LastMessage.Id,
                    Content = conversation.LastMessage.Content,
                    SenderName = conversation.LastMessage.Sender?.Name ?? "",
                    CreatedAt = conversation.LastMessage.CreatedDate,
                    Type = conversation.LastMessage.Type,
                    Status = conversation.LastMessage.Status
                } : null,
                Settings = new ConversationSettingsResponse
                {
                    Notifications = conversation.Settings.Notifications,
                    SoundEnabled = conversation.Settings.SoundEnabled,
                    AutoArchive = conversation.Settings.AutoArchive,
                    RetentionDays = conversation.Settings.RetentionDays
                }
            };
        }
    }
}
