using Wasalnyy.DAL.Entities;

namespace Wasalnyy.DAL.Repo.Abstraction
{
    public interface IChatRepo
    {
        public Task<int> GetUserMessagesCountAsync(string userId);
        Task CreateAsync(Message message);
        Task<Message?> GetByIdAsync(int id);
        Task<IEnumerable<Message>> GetConversationAsync(string userId1, string userId2, int pageNumber = 1, int pageSize = 50);
        Task<IEnumerable<Message>> GetUserMessagesAsync(string userId, int pageNumber = 1, int pageSize = 50);
        Task<IEnumerable<Message>> GetUnreadMessagesAsync(string RiecverID);
        Task<int> GetUnreadCountAsync(string RiecverID);
        Task MarkAsReadAsync(int messageId);
        Task MarkConversationAsReadAsync(string CurrentUser, string OtherUser);
        Task<Message?> GetLastMessageAsync(string userId1, string userId2);
        Task DeleteAsync(int messageId);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task<int> GetConversationCountAsync(string userId1, string userId2);
    }
}