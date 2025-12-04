using Wasalnyy.BLL.DTO.Chat;
using Wasalnyy.DAL.Entities;

namespace Wasalnyy.BLL.Service
{
    public interface IChatService
    {
        public Task<GetMessageDTO?> GetLastMessageAsync(string userId1, string userId2);

        public Task<IEnumerable<GetMessageDTO>> GetUnreadMessagesAsync(string receiverId);

        public Task<int> GetUserMessagesPagesCountAsync(string userId, int pageSize = 50);

        public Task<int> GetConversationPagesCountAsync(string userId1, string userId2, int pageSize = 50);
        Task<GetMessageDTO> SendMessageAsync(string senderId, string receiverId, string content);
        Task<GetMessageDTO?> GetMessageByIdAsync(int id);
        public Task<MessagePaginationDto> GetConversationAsync(string userId1, string userId2, int pageNumber = 1, int pageSize = 50);

        Task<MessagePaginationDto> GetUserMessagesAsync(string userId, int pageNumber = 1, int pageSize = 50);
        Task<int> GetUnreadCountAsync(string userId);
        Task MarkAsReadAsync(int messageId);
        Task MarkConversationAsReadAsync(string userId, string otherUserId);
        Task DeleteMessageAsync(int messageId);

        Task <ChatSidebarListReponse> GetChatSidebarList(string userId );
    }
}