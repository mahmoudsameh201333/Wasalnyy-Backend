using Microsoft.EntityFrameworkCore;
using Wasalnyy.DAL.Database;
using Wasalnyy.DAL.Entities;
using Wasalnyy.DAL.Repo.Abstraction;

namespace Wasalnyy.DAL.Repo.Implementation
{
    public class ChatRepo : IChatRepo
    {
        private readonly WasalnyyDbContext _context;

        public ChatRepo(WasalnyyDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(Message message)
        {
            await _context.Messages.AddAsync(message);
        }

        public async Task<Message?> GetByIdAsync(int id)
        {
            return await _context.Messages
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<IEnumerable<Message>> GetConversationAsync(string userId1, string userId2, int pageNumber = 1, int pageSize = 50)
        {
            return await _context.Messages
                .AsNoTracking()  
                .Include(m => m.Sender)    
                .Include(m => m.Receiver)
                .Where(m => (m.SenderId == userId1 && m.ReceiverId == userId2) ||
                           (m.SenderId == userId2 && m.ReceiverId == userId1))
                .OrderByDescending(m => m.SentAt)
                .Skip((pageNumber - 1) * pageSize)  
                .Take(pageSize)
                .ToListAsync();
        }

        

        public async Task<IEnumerable<Message>> GetUnreadMessagesAsync(string RiecverID)
        {
            return await _context.Messages
                .Where(m => m.ReceiverId == RiecverID && !m.IsRead)
                .OrderBy(m => m.SentAt)
                .Include(m => m.Sender)
                .ToListAsync();
        }

        public async Task<int> GetUnreadCountAsync(string RiecverID)
        {
            return await _context.Messages
                .CountAsync(m => m.ReceiverId == RiecverID && !m.IsRead);
        }

        public async Task MarkAsReadAsync(int messageId)
        {
            var message = await _context.Messages.FindAsync(messageId);
            if (message != null)
            {
                message.MarkAsRead();
            }
        }

        public async Task MarkConversationAsReadAsync(string CurrentUser, string OtherUser)
        {
            var unreadMessages = await _context.Messages
                .Where(m => m.SenderId == OtherUser && m.ReceiverId == CurrentUser && !m.IsRead)
                .ToListAsync();

            foreach (var message in unreadMessages)
            {
                message.MarkAsRead();
            }
        }
        public async Task<IEnumerable<Message>> GetUserMessagesAsync(string userId, int pageNumber = 1, int pageSize = 50)
        {
            return await _context.Messages
                .AsNoTracking()  // Add this
                .Where(m => m.SenderId == userId || m.ReceiverId == userId)
                .OrderByDescending(m => m.SentAt)
                .Include(m => m.Sender)    // Move before Skip/Take
                .Include(m => m.Receiver)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
        public async Task<Message?> GetLastMessageAsync(string userId1, string userId2)
        {
            return await _context.Messages
                .Where(m => (m.SenderId == userId1 && m.ReceiverId == userId2) ||
                           (m.SenderId == userId2 && m.ReceiverId == userId1))
                .OrderByDescending(m => m.SentAt)
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .FirstOrDefaultAsync();
        }

        public async Task DeleteAsync(int messageId)
        {
            var message = await _context.Messages.FindAsync(messageId);
            if (message != null)
            {
                _context.Messages.Remove(message);
            }
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<int> GetUnreadCountFromUserAsync(string receiverId, string senderId)
        {
            return await _context.Messages
                .CountAsync(m => m.ReceiverId == receiverId && m.SenderId == senderId && !m.IsRead);
        }

        // Repository Layer - Add count method
        public async Task<int> GetConversationCountAsync(string userId1, string userId2)
        {
            return await _context.Messages
                .Where(m => (m.SenderId == userId1 && m.ReceiverId == userId2) ||
                           (m.SenderId == userId2 && m.ReceiverId == userId1))
                .CountAsync();
        }

        public async Task<IEnumerable<string>> GetChatParticipantsAsync(string userId)
        {
            return await _context.Messages
                .Where(m => m.SenderId == userId || m.ReceiverId == userId)
                .Select(m => m.SenderId == userId ? m.ReceiverId : m.SenderId)
                .Distinct()
                .ToListAsync();
        }
        public async Task<int> GetUserMessagesCountAsync(string userId)
        {
            return await _context.Messages
                .Where(m => m.SenderId == userId || m.ReceiverId == userId)
                .CountAsync();
        }
    }
}