using System.Collections.Generic;
using Core.DTO.Message;

namespace Services
{
    public interface IMessageService
    {
        // Conversations
        ConversationsPagedDTO GetConversations(ConversationFiltersDTO filters, int page = 1, int limit = 20);
        ConversationResponse GetConversationById(int id);
        ConversationResponse CreateConversation(CreateConversationRequest request);
        ConversationResponse UpdateConversation(int id, UpdateConversationRequest request);
        void DeleteConversation(int id);
        void AddParticipant(int conversationId, AddParticipantRequest request);
        void RemoveParticipant(int conversationId, int userId);
        void UpdateParticipant(int conversationId, int userId, UpdateParticipantRequest request);
        void ArchiveConversation(int id);
        void UnarchiveConversation(int id);
        void MuteConversation(int id);
        void UnmuteConversation(int id);

        // Messages
        MessagesPagedDTO GetMessages(MessageFiltersDTO filters, int page = 1, int limit = 20);
        MessagesPagedDTO GetConversationMessages(int conversationId, int page = 1, int limit = 20);
        MessageResponse GetMessageById(int id);
        MessageResponse CreateMessage(CreateMessageRequest request);
        MessageResponse UpdateMessage(int id, UpdateMessageRequest request);
        void DeleteMessage(int id);
        void MarkMessagesAsRead(int conversationId, MarkMessagesAsReadRequest request);
        void MarkMessageAsDelivered(int messageId);

        // Reactions
        void AddReaction(int messageId, CreateMessageReactionRequest request);
        void RemoveReaction(int messageId, int reactionId);

        // Templates
        List<MessageTemplateResponse> GetTemplates(Core.Enums.TemplateCategory? category = null, int? empresasId = null);
        MessageTemplateResponse GetTemplateById(int id);
        MessageTemplateResponse CreateTemplate(CreateMessageTemplateRequest request);
        MessageTemplateResponse UpdateTemplate(int id, UpdateMessageTemplateRequest request);
        void DeleteTemplate(int id);
        string RenderTemplate(int templateId, RenderTemplateRequest request);

        // Stats
        MessageStatsDTO GetMessageStats(int? empresasId = null, System.DateTime? from = null, System.DateTime? to = null);
        ConversationStatsDTO GetConversationStats(int conversationId);

        // Search
        MessagesPagedDTO SearchMessages(MessageFiltersDTO filters, int page = 1, int limit = 20);
    }
}
