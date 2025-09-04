using System;
using System.Collections.Generic;
using System.Linq;
using Core.DTO.Message;
using Core.Models.Message;
using Infrastructure.Repositories;

namespace Services
{
    public class MessageService : IMessageService
    {
        private readonly IUnitOfWork _unitOfWork;

        public MessageService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // ========== Conversations ==========

        public ConversationsPagedDTO GetConversations(ConversationFiltersDTO filters, int page = 1, int limit = 20)
        {
            var repo = (ConversationRepository)_unitOfWork.Conversations;
            return repo.GetPagedConversations(filters, page, limit);
        }

        public ConversationResponse GetConversationById(int id)
        {
            var list = _unitOfWork.Conversations.GetAll().GetAwaiter().GetResult();
            var conversation = list.FirstOrDefault(c => c.Id == id);
            if (conversation == null)
                throw new ArgumentException("Conversation not found");

            return MapToConversationResponse(conversation);
        }

        public ConversationResponse CreateConversation(CreateConversationRequest request)
        {
            var conversation = new Conversation
            {
                EmpresasId = request.EmpresasId,
                Type = request.Type,
                Title = request.Title,
                Description = request.Description,
                Settings = new ConversationSettings
                {
                    Notifications = request.Settings?.Notifications ?? true,
                    SoundEnabled = request.Settings?.SoundEnabled ?? true,
                    AutoArchive = request.Settings?.AutoArchive ?? false,
                    RetentionDays = request.Settings?.RetentionDays
                }
            };

            _unitOfWork.Conversations.Add(conversation);
            _unitOfWork.SaveAsync().GetAwaiter().GetResult();

            if (request.ParticipantIds != null)
            {
                foreach (var participantId in request.ParticipantIds)
                {
                    var participant = new ConversationParticipant
                    {
                        ConversationId = conversation.Id,
                        UserId = participantId,
                        Role = Core.Enums.ParticipantRole.Member,
                        JoinedAt = DateTime.UtcNow,
                        Permissions = new ParticipantPermissions()
                    };
                    _unitOfWork.ConversationParticipants.Add(participant);
                }
                _unitOfWork.SaveAsync().GetAwaiter().GetResult();
            }

            return GetConversationById(conversation.Id);
        }

        public ConversationResponse UpdateConversation(int id, UpdateConversationRequest request)
        {
            var list = _unitOfWork.Conversations.GetAll().GetAwaiter().GetResult();
            var conversation = list.FirstOrDefault(c => c.Id == id);
            if (conversation == null)
                throw new ArgumentException("Conversation not found");

            if (request.Title != null) conversation.Title = request.Title;
            if (request.Description != null) conversation.Description = request.Description;
            if (request.IsArchived.HasValue) conversation.IsArchived = request.IsArchived.Value;
            if (request.IsMuted.HasValue) conversation.IsMuted = request.IsMuted.Value;

            if (request.Settings != null)
            {
                conversation.Settings ??= new ConversationSettings();
                if (request.Settings.Notifications.HasValue)
                    conversation.Settings.Notifications = request.Settings.Notifications.Value;
                if (request.Settings.SoundEnabled.HasValue)
                    conversation.Settings.SoundEnabled = request.Settings.SoundEnabled.Value;
                if (request.Settings.AutoArchive.HasValue)
                    conversation.Settings.AutoArchive = request.Settings.AutoArchive.Value;
                if (request.Settings.RetentionDays.HasValue)
                    conversation.Settings.RetentionDays = request.Settings.RetentionDays.Value;
            }

            _unitOfWork.Conversations.Update(conversation);
            _unitOfWork.SaveAsync().GetAwaiter().GetResult();

            return GetConversationById(id);
        }

        public void DeleteConversation(int id)
        {
            var list = _unitOfWork.Conversations.GetAll().GetAwaiter().GetResult();
            var conversation = list.FirstOrDefault(c => c.Id == id);
            if (conversation == null)
                throw new ArgumentException("Conversation not found");

            _unitOfWork.Conversations.Delete(conversation);
            _unitOfWork.SaveAsync().GetAwaiter().GetResult();
        }

        public void AddParticipant(int conversationId, AddParticipantRequest request)
        {
            var participant = new ConversationParticipant
            {
                ConversationId = conversationId,
                UserId = request.UserId,
                Role = request.Role,
                JoinedAt = DateTime.UtcNow,
                Permissions = new ParticipantPermissions
                {
                    CanSendMessages = request.Permissions?.CanSendMessages ?? true,
                    CanSendAttachments = request.Permissions?.CanSendAttachments ?? true,
                    CanDeleteMessages = request.Permissions?.CanDeleteMessages ?? false,
                    CanAddParticipants = request.Permissions?.CanAddParticipants ?? false,
                    CanRemoveParticipants = request.Permissions?.CanRemoveParticipants ?? false,
                    CanEditConversation = request.Permissions?.CanEditConversation ?? false
                }
            };

            _unitOfWork.ConversationParticipants.Add(participant);
            _unitOfWork.SaveAsync().GetAwaiter().GetResult();
        }

        public void RemoveParticipant(int conversationId, int userId)
        {
            var list = _unitOfWork.ConversationParticipants.GetAll().GetAwaiter().GetResult();
            var participant = list.FirstOrDefault(p => p.ConversationId == conversationId && p.UserId == userId);
            if (participant != null)
            {
                _unitOfWork.ConversationParticipants.Delete(participant);
                _unitOfWork.SaveAsync().GetAwaiter().GetResult();
            }
        }

        public void UpdateParticipant(int conversationId, int userId, UpdateParticipantRequest request)
        {
            var list = _unitOfWork.ConversationParticipants.GetAll().GetAwaiter().GetResult();
            var participant = list.FirstOrDefault(p => p.ConversationId == conversationId && p.UserId == userId);
            if (participant == null)
                throw new ArgumentException("Participant not found");

            if (request.Role.HasValue) participant.Role = request.Role.Value;

            if (request.Permissions != null)
            {
                participant.Permissions ??= new ParticipantPermissions();
                if (request.Permissions.CanSendMessages.HasValue)
                    participant.Permissions.CanSendMessages = request.Permissions.CanSendMessages.Value;
                if (request.Permissions.CanSendAttachments.HasValue)
                    participant.Permissions.CanSendAttachments = request.Permissions.CanSendAttachments.Value;
                if (request.Permissions.CanDeleteMessages.HasValue)
                    participant.Permissions.CanDeleteMessages = request.Permissions.CanDeleteMessages.Value;
                if (request.Permissions.CanAddParticipants.HasValue)
                    participant.Permissions.CanAddParticipants = request.Permissions.CanAddParticipants.Value;
                if (request.Permissions.CanRemoveParticipants.HasValue)
                    participant.Permissions.CanRemoveParticipants = request.Permissions.CanRemoveParticipants.Value;
                if (request.Permissions.CanEditConversation.HasValue)
                    participant.Permissions.CanEditConversation = request.Permissions.CanEditConversation.Value;
            }

            _unitOfWork.ConversationParticipants.Update(participant);
            _unitOfWork.SaveAsync().GetAwaiter().GetResult();
        }

        public void ArchiveConversation(int id)
        {
            var list = _unitOfWork.Conversations.GetAll().GetAwaiter().GetResult();
            var conversation = list.FirstOrDefault(c => c.Id == id);
            if (conversation != null)
            {
                conversation.IsArchived = true;
                _unitOfWork.Conversations.Update(conversation);
                _unitOfWork.SaveAsync().GetAwaiter().GetResult();
            }
        }

        public void UnarchiveConversation(int id)
        {
            var list = _unitOfWork.Conversations.GetAll().GetAwaiter().GetResult();
            var conversation = list.FirstOrDefault(c => c.Id == id);
            if (conversation != null)
            {
                conversation.IsArchived = false;
                _unitOfWork.Conversations.Update(conversation);
                _unitOfWork.SaveAsync().GetAwaiter().GetResult();
            }
        }

        public void MuteConversation(int id)
        {
            var list = _unitOfWork.Conversations.GetAll().GetAwaiter().GetResult();
            var conversation = list.FirstOrDefault(c => c.Id == id);
            if (conversation != null)
            {
                conversation.IsMuted = true;
                _unitOfWork.Conversations.Update(conversation);
                _unitOfWork.SaveAsync().GetAwaiter().GetResult();
            }
        }

        public void UnmuteConversation(int id)
        {
            var list = _unitOfWork.Conversations.GetAll().GetAwaiter().GetResult();
            var conversation = list.FirstOrDefault(c => c.Id == id);
            if (conversation != null)
            {
                conversation.IsMuted = false;
                _unitOfWork.Conversations.Update(conversation);
                _unitOfWork.SaveAsync().GetAwaiter().GetResult();
            }
        }

        // ========== Messages ==========

        public MessagesPagedDTO GetMessages(MessageFiltersDTO filters, int page = 1, int limit = 20)
        {
            var repo = (MessageRepository)_unitOfWork.Messages;
            return repo.GetPagedMessages(filters, page, limit);
        }

        public MessagesPagedDTO GetConversationMessages(int conversationId, int page = 1, int limit = 20)
        {
            var repo = (MessageRepository)_unitOfWork.Messages;
            var items = repo.GetConversationMessages(conversationId, page, limit);
            return new MessagesPagedDTO
            {
                Messages = items.Select(MapToMessageResponse).ToList(),
                HasMore = items.Count == limit,
                Total = items.Count,
                Page = page,
                Limit = limit
            };
        }

        public MessageResponse GetMessageById(int id)
        {
            var list = _unitOfWork.Messages.GetAll().GetAwaiter().GetResult();
            var message = list.FirstOrDefault(m => m.Id == id);
            if (message == null)
                throw new ArgumentException("Message not found");

            return MapToMessageResponse(message);
        }

        public MessageResponse CreateMessage(CreateMessageRequest request)
        {
            var message = new Message
            {
                ConversationId = request.ConversationId,
                SenderId = request.SenderId,
                ReceiverId = request.ReceiverId,
                Type = request.Type,
                Status = Core.Enums.MessageStatus.Sent,
                Content = request.Content,
                ReplyToId = request.ReplyToId,
                DeliveredAt = DateTime.UtcNow,
                Metadata = new MessageMetadata
                {
                    SystemAction = request.Metadata?.SystemAction,
                    CustomData = request.Metadata?.CustomData
                }
            };

            _unitOfWork.Messages.Add(message);
            _unitOfWork.SaveAsync().GetAwaiter().GetResult();

            if (request.Attachments != null && request.Attachments.Any())
            {
                foreach (var a in request.Attachments)
                {
                    var attachment = new MessageAttachment
                    {
                        MessageId = message.Id,
                        Type = a.Type,
                        Name = a.Name,
                        Url = a.Url,
                        Size = a.Size,
                        MimeType = a.MimeType,
                        Thumbnail = a.Thumbnail,
                        Metadata = new AttachmentMetadata
                        {
                            Width = a.Metadata?.Width,
                            Height = a.Metadata?.Height,
                            Duration = a.Metadata?.Duration,
                            Description = a.Metadata?.Description,
                            CustomData = a.Metadata?.CustomData
                        }
                    };
                    _unitOfWork.MessageAttachments.Add(attachment);
                }
                _unitOfWork.SaveAsync().GetAwaiter().GetResult();
            }

            ((ConversationRepository)_unitOfWork.Conversations).UpdateLastMessage(request.ConversationId, message.Id);
            _unitOfWork.SaveAsync().GetAwaiter().GetResult();

            return GetMessageById(message.Id);
        }

        public MessageResponse UpdateMessage(int id, UpdateMessageRequest request)
        {
            var list = _unitOfWork.Messages.GetAll().GetAwaiter().GetResult();
            var message = list.FirstOrDefault(m => m.Id == id);
            if (message == null)
                throw new ArgumentException("Message not found");

            if (request.Content != null)
                message.Content = request.Content;

            if (request.Metadata != null)
            {
                message.Metadata ??= new MessageMetadata();
                message.Metadata.Edited = true;
                message.Metadata.EditReason = "User edited";
                message.Metadata.SystemAction = request.Metadata.SystemAction;
                message.Metadata.CustomData = request.Metadata.CustomData;
            }

            _unitOfWork.Messages.Update(message);
            _unitOfWork.SaveAsync().GetAwaiter().GetResult();

            return GetMessageById(id);
        }

        public void DeleteMessage(int id)
        {
            var list = _unitOfWork.Messages.GetAll().GetAwaiter().GetResult();
            var message = list.FirstOrDefault(m => m.Id == id);
            if (message == null)
                throw new ArgumentException("Message not found");

            _unitOfWork.Messages.Delete(message);
            _unitOfWork.SaveAsync().GetAwaiter().GetResult();
        }

        public void MarkMessagesAsRead(int conversationId, MarkMessagesAsReadRequest request)
        {
            ((MessageRepository)_unitOfWork.Messages).MarkMessagesAsRead(request.MessageIds, request.ReaderId);
            ((ConversationRepository)_unitOfWork.Conversations).UpdateUnreadCount(conversationId, request.ReaderId, 0);
            _unitOfWork.SaveAsync().GetAwaiter().GetResult();
        }

        public void MarkMessageAsDelivered(int messageId)
        {
            var list = _unitOfWork.Messages.GetAll().GetAwaiter().GetResult();
            var message = list.FirstOrDefault(m => m.Id == messageId);
            if (message != null)
            {
                message.Status = Core.Enums.MessageStatus.Delivered;
                message.DeliveredAt = DateTime.UtcNow;
                _unitOfWork.Messages.Update(message);
                _unitOfWork.SaveAsync().GetAwaiter().GetResult();
            }
        }

        // ========== Reactions ==========

        public void AddReaction(int messageId, CreateMessageReactionRequest request)
        {
            var reaction = new MessageReaction
            {
                MessageId = messageId,
                UserId = request.UserId,
                Emoji = request.Emoji
            };

            _unitOfWork.MessageReactions.Add(reaction);
            _unitOfWork.SaveAsync().GetAwaiter().GetResult();
        }

        public void RemoveReaction(int messageId, int reactionId)
        {
            var list = _unitOfWork.MessageReactions.GetAll().GetAwaiter().GetResult();
            var reaction = list.FirstOrDefault(r => r.Id == reactionId && r.MessageId == messageId);
            if (reaction != null)
            {
                _unitOfWork.MessageReactions.Delete(reaction);
                _unitOfWork.SaveAsync().GetAwaiter().GetResult();
            }
        }

        // ========== Templates ==========

        public List<MessageTemplateResponse> GetTemplates(Core.Enums.TemplateCategory? category = null, int? empresasId = null)
        {
            var templates = _unitOfWork.MessageTemplates.GetAll().GetAwaiter().GetResult().ToList();

            if (category.HasValue)
                templates = templates.Where(t => t.Category == category.Value).ToList();

            if (empresasId.HasValue)
                templates = templates.Where(t => t.EmpresasId == empresasId.Value || t.IsPublic).ToList();

            return templates.Select(MapToTemplateResponse).ToList();
        }

        public MessageTemplateResponse GetTemplateById(int id)
        {
            var templates = _unitOfWork.MessageTemplates.GetAll().GetAwaiter().GetResult();
            var template = templates.FirstOrDefault(t => t.Id == id);
            if (template == null)
                throw new ArgumentException("Template not found");

            return MapToTemplateResponse(template);
        }

        public MessageTemplateResponse CreateTemplate(CreateMessageTemplateRequest request)
        {
            var template = new MessageTemplate
            {
                EmpresasId = request.EmpresasId,
                Name = request.Name,
                Content = request.Content,
                Category = request.Category,
                IsPublic = request.IsPublic,
                UsageCount = 0,
                CreatedBy = 1, // TODO: usuário atual
                Variables = new TemplateVariables { Variables = request.Variables }
            };

            _unitOfWork.MessageTemplates.Add(template);
            _unitOfWork.SaveAsync().GetAwaiter().GetResult();

            return GetTemplateById(template.Id);
        }

        public MessageTemplateResponse UpdateTemplate(int id, UpdateMessageTemplateRequest request)
        {
            var list = _unitOfWork.MessageTemplates.GetAll().GetAwaiter().GetResult();
            var template = list.FirstOrDefault(t => t.Id == id);
            if (template == null)
                throw new ArgumentException("Template not found");

            if (request.Name != null) template.Name = request.Name;
            if (request.Content != null) template.Content = request.Content;
            if (request.Category.HasValue) template.Category = request.Category.Value;
            if (request.Variables != null)
            {
                template.Variables ??= new TemplateVariables();
                template.Variables.Variables = request.Variables;
            }
            if (request.IsPublic.HasValue) template.IsPublic = request.IsPublic.Value;

            _unitOfWork.MessageTemplates.Update(template);
            _unitOfWork.SaveAsync().GetAwaiter().GetResult();

            return GetTemplateById(id);
        }

        public void DeleteTemplate(int id)
        {
            var list = _unitOfWork.MessageTemplates.GetAll().GetAwaiter().GetResult();
            var template = list.FirstOrDefault(t => t.Id == id);
            if (template == null)
                throw new ArgumentException("Template not found");

            _unitOfWork.MessageTemplates.Delete(template);
            _unitOfWork.SaveAsync().GetAwaiter().GetResult();
        }

        // >>>>>>> MÉTODO QUE FALTAVA NA INTERFACE <<<<<<<
        public string RenderTemplate(int templateId, RenderTemplateRequest request)
        {
            var repo = (MessageTemplateRepository)_unitOfWork.MessageTemplates;
            var vars = request?.Variables ?? new Dictionary<string, string>();
            return repo.RenderTemplate(templateId, vars);
        }

        // ========== Search / Stats ==========

        public MessagesPagedDTO SearchMessages(MessageFiltersDTO filters, int page = 1, int limit = 20)
        {
            return GetMessages(filters, page, limit);
        }

        public MessageStatsDTO GetMessageStats(int? empresasId = null, DateTime? from = null, DateTime? to = null)
        {
            var repo = (MessageRepository)_unitOfWork.Messages;
            return repo.GetMessageStats(empresasId, from, to);
        }

        public ConversationStatsDTO GetConversationStats(int conversationId)
        {
            var repo = (MessageRepository)_unitOfWork.Messages;
            return repo.GetConversationStats(conversationId);
        }

        // ========== Mappers ==========

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
                Settings = new ConversationSettingsResponse
                {
                    Notifications = conversation.Settings?.Notifications ?? true,
                    SoundEnabled = conversation.Settings?.SoundEnabled ?? true,
                    AutoArchive = conversation.Settings?.AutoArchive ?? false,
                    RetentionDays = conversation.Settings?.RetentionDays
                }
            };
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
                Metadata = new MessageMetadataResponse
                {
                    Edited = message.Metadata?.Edited ?? false,
                    EditReason = message.Metadata?.EditReason,
                    SystemAction = message.Metadata?.SystemAction,
                    CustomData = message.Metadata?.CustomData
                }
            };
        }

        private MessageTemplateResponse MapToTemplateResponse(MessageTemplate template)
        {
            return new MessageTemplateResponse
            {
                Id = template.Id,
                EmpresasId = template.EmpresasId,
                Name = template.Name,
                Content = template.Content,
                Category = template.Category,
                Variables = template.Variables?.Variables ?? new List<string>(),
                IsPublic = template.IsPublic,
                UsageCount = template.UsageCount,
                CreatedBy = template.CreatedBy,
                CreatedByName = template.Creator?.Name ?? "",
                CreatedAt = template.CreatedDate,
                UpdatedAt = template.UpdatedDate
            };
        }
    }
}
