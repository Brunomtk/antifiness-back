using System;
using System.Collections.Generic;
using System.Linq;
using Core.DTO.Message;
using Core.Models.Message;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class MessageRepository : GenericRepository<Message>
    {
        public MessageRepository(DbContextClass context) : base(context)
        {
        }

        public MessagesPagedDTO GetPagedMessages(MessageFiltersDTO filters, int page = 1, int limit = 20)
        {
            var query = _dbContext.Messages
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .Include(m => m.Attachments)
                .Include(m => m.Reactions)
                    .ThenInclude(r => r.User)
                .Include(m => m.ReplyTo)
                .AsQueryable();

            if (filters.ConversationId.HasValue)
                query = query.Where(m => m.ConversationId == filters.ConversationId.Value);

            if (filters.SenderId.HasValue)
                query = query.Where(m => m.SenderId == filters.SenderId.Value);

            if (filters.Type.HasValue)
                query = query.Where(m => m.Type == filters.Type.Value);

            if (filters.Status.HasValue)
                query = query.Where(m => m.Status == filters.Status.Value);

            if (filters.HasAttachments.HasValue)
            {
                if (filters.HasAttachments.Value)
                    query = query.Where(m => m.Attachments.Any());
                else
                    query = query.Where(m => !m.Attachments.Any());
            }

            if (filters.DateFrom.HasValue)
                query = query.Where(m => m.CreatedDate >= filters.DateFrom.Value);

            if (filters.DateTo.HasValue)
                query = query.Where(m => m.CreatedDate <= filters.DateTo.Value);

            if (!string.IsNullOrEmpty(filters.Search))
                query = query.Where(m => m.Content.Contains(filters.Search));

            var total = query.Count();
            var messages = query
                .OrderByDescending(m => m.CreatedDate)
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToList();

            return new MessagesPagedDTO
            {
                Messages = messages.Select(MapToMessageResponse).ToList(),
                HasMore = total > page * limit,
                Total = total,
                Page = page,
                Limit = limit
            };
        }

        public List<Message> GetConversationMessages(int conversationId, int page = 1, int limit = 20)
        {
            return _dbContext.Messages
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .Include(m => m.Attachments)
                .Include(m => m.Reactions)
                    .ThenInclude(r => r.User)
                .Include(m => m.ReplyTo)
                .Where(m => m.ConversationId == conversationId)
                .OrderBy(m => m.CreatedDate)
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToList();
        }

        public void MarkMessagesAsRead(List<int> messageIds, int readerId)
        {
            var messages = _dbContext.Messages
                .Where(m => messageIds.Contains(m.Id))
                .ToList();

            var now = DateTime.UtcNow;
            foreach (var message in messages)
            {
                message.Status = Core.Enums.MessageStatus.Read;
                message.ReadAt = now;
                Update(message);
            }
        }

        public MessageStatsDTO GetMessageStats(int? empresasId = null, DateTime? from = null, DateTime? to = null)
        {
            var query = _dbContext.Messages.AsQueryable();

            if (empresasId.HasValue)
                query = query.Where(m => m.Conversation.EmpresasId == empresasId.Value);

            if (from.HasValue)
                query = query.Where(m => m.CreatedDate >= from.Value);

            if (to.HasValue)
                query = query.Where(m => m.CreatedDate <= to.Value);

            var messages = query.ToList();
            var conversations = _dbContext.Conversations.AsQueryable();

            if (empresasId.HasValue)
                conversations = conversations.Where(c => c.EmpresasId == empresasId.Value);

            var conversationsList = conversations.ToList();

            var weekAgo = DateTime.UtcNow.AddDays(-7);
            var messagesThisWeek = messages.Count(m => m.CreatedDate >= weekAgo);

            return new MessageStatsDTO
            {
                TotalMessages = messages.Count,
                TotalConversations = conversationsList.Count,
                UnreadMessages = messages.Count(m => m.Status != Core.Enums.MessageStatus.Read),
                ActiveConversations = conversationsList.Count(c => !c.IsArchived),
                MessagesThisWeek = messagesThisWeek,
                AverageResponseTime = CalculateAverageResponseTime(messages),
                MostActiveHour = GetMostActiveHour(messages),
                AttachmentsSent = messages.Sum(m => m.Attachments.Count)
            };
        }

        public ConversationStatsDTO GetConversationStats(int conversationId)
        {
            var messages = _dbContext.Messages
                .Include(m => m.Attachments)
                .Include(m => m.Reactions)
                .Where(m => m.ConversationId == conversationId)
                .ToList();

            var conversation = _dbContext.Conversations
                .Include(c => c.Participants)
                .FirstOrDefault(c => c.Id == conversationId);

            return new ConversationStatsDTO
            {
                MessageCount = messages.Count,
                ParticipantCount = conversation?.Participants.Count ?? 0,
                AverageResponseTime = CalculateAverageResponseTime(messages),
                LastActivity = messages.OrderByDescending(m => m.CreatedDate).FirstOrDefault()?.CreatedDate,
                AttachmentCount = messages.Sum(m => m.Attachments.Count),
                ReactionCount = messages.Sum(m => m.Reactions.Count)
            };
        }

        private double CalculateAverageResponseTime(List<Message> messages)
        {
            // Simple calculation - in real implementation, you'd calculate time between messages in conversations
            return messages.Any() ? 15.5 : 0; // Default 15.5 minutes
        }

        private int GetMostActiveHour(List<Message> messages)
        {
            if (!messages.Any()) return 12;

            return messages
                .GroupBy(m => m.CreatedDate.Hour)
                .OrderByDescending(g => g.Count())
                .First()
                .Key;
        }

        private MessageResponse MapToMessageResponse(Message message)
        {
            return new MessageResponse
            {
                Id = message.Id,
                ConversationId = message.ConversationId,
                SenderId = message.SenderId,
                SenderName = message.Sender?.Name ?? "",
                ReceiverId = message.ReceiverId,
                ReceiverName = message.Receiver?.Name,
                Type = message.Type,
                Status = message.Status,
                Content = message.Content,
                ReplyToId = message.ReplyToId,
                CreatedAt = message.CreatedDate,
                UpdatedAt = message.UpdatedDate,
                ReadAt = message.ReadAt,
                DeliveredAt = message.DeliveredAt,
                Attachments = message.Attachments.Select(a => new MessageAttachmentResponse
                {
                    Id = a.Id,
                    Type = a.Type,
                    Name = a.Name,
                    Url = a.Url,
                    Size = a.Size,
                    MimeType = a.MimeType,
                    Thumbnail = a.Thumbnail,
                    Metadata = new AttachmentMetadataResponse
                    {
                        Width = a.Metadata.Width,
                        Height = a.Metadata.Height,
                        Duration = a.Metadata.Duration,
                        Description = a.Metadata.Description,
                        CustomData = a.Metadata.CustomData
                    }
                }).ToList(),
                Reactions = message.Reactions.Select(r => new MessageReactionResponse
                {
                    Id = r.Id,
                    UserId = r.UserId,
                    UserName = r.User?.Name ?? "",
                    Emoji = r.Emoji,
                    CreatedAt = r.CreatedDate
                }).ToList(),
                Metadata = new MessageMetadataResponse
                {
                    Edited = message.Metadata.Edited,
                    EditReason = message.Metadata.EditReason,
                    SystemAction = message.Metadata.SystemAction,
                    CustomData = message.Metadata.CustomData
                },
                ReplyTo = message.ReplyTo != null ? new MessageResponse
                {
                    Id = message.ReplyTo.Id,
                    Content = message.ReplyTo.Content,
                    SenderName = message.ReplyTo.Sender?.Name ?? "",
                    CreatedAt = message.ReplyTo.CreatedDate
                } : null
            };
        }
    }
}
