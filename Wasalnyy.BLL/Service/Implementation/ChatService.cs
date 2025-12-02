using AutoMapper;
using Microsoft.AspNetCore.Identity;
using MimeKit;
using Wasalnyy.BLL.DTO.Chat;
using Wasalnyy.BLL.Exceptions;
using Wasalnyy.DAL.Entities;
using Wasalnyy.DAL.Repo.Abstraction;

namespace Wasalnyy.BLL.Service.Implementation
{
    public class ChatService : IChatService
    {
        private readonly IChatRepo _messageRepo;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        public ChatService(IChatRepo messageRepo, UserManager<User> userManager,IMapper mapper)
        {
            _messageRepo = messageRepo;
            _userManager = userManager;
            _mapper = mapper;
        }

        // n3ml Dto
        public async Task<GetMessageDTO> SendMessageAsync(string senderId, string receiverId, string content)
        {
            // Validate users exist
            var sender = await _userManager.FindByIdAsync(senderId);
            if (sender == null)
                throw new NotFoundException($"Sender with ID {senderId} not found");

            var receiver = await _userManager.FindByIdAsync(receiverId);
            if (receiver == null)
                throw new NotFoundException($"Receiver with ID {receiverId} not found");

            // Validate content
            if (string.IsNullOrWhiteSpace(content))
                throw new ArgumentException("Message content cannot be empty");

            if (content.Length > 2000)
                throw new ArgumentException("Message content cannot exceed 2000 characters");

            // Create and save message
            var message = new Message(senderId, receiverId, content);
            await _messageRepo.CreateAsync(message);
            await _messageRepo.SaveChangesAsync();

            return _mapper.Map<GetMessageDTO>(message);

        }

        public async Task<GetMessageDTO?> GetMessageByIdAsync(int id)
        {
            var message = await _messageRepo.GetByIdAsync(id);
            if (message == null) return null;

            return _mapper.Map<GetMessageDTO>(message);
        }


        public async Task<MessagePaginationDto> GetConversationAsync(string userId1, string userId2, int pageNumber = 1, int pageSize = 50)
        {
            var u1 = await _userManager.FindByIdAsync(userId1);
            if (u1 == null)
                throw new NotFoundException($"Sender with ID {userId1} not found");

            var u2 = await _userManager.FindByIdAsync(userId2);
            if (u2 == null)
                throw new NotFoundException($"Receiver with ID {userId2} not found");
            // Validate and normalize inputs
            if (pageNumber <= 0)
                pageNumber = 1;
            if (pageSize <= 0 || pageSize > 100)
                pageSize = 50;

            // Get total pages
            var totalPageCount = await GetConversationPagesCountAsync(userId1, userId2, pageSize);


            // If no messages or page out of bounds, return empty with metadata
            if (totalPageCount == 0) 
                return new MessagePaginationDto
                {
                    CurrentPage = pageNumber,
                    PageSize = pageSize,
                    TotalPages = 0,
                    Messages = new List<GetMessageDTO>()
                };

            if (pageNumber > totalPageCount)
            {
                return new MessagePaginationDto
                {
                    CurrentPage = pageNumber,
                    PageSize = pageSize,
                    TotalPages = totalPageCount,
                    Messages = new List<GetMessageDTO>()
                };

            }

                // Fetch messages
                var messages = (await _messageRepo.GetConversationAsync(userId1, userId2, pageNumber, pageSize)).ToList();

            return new MessagePaginationDto
            {
                Messages = _mapper.Map<List<Message>, List<GetMessageDTO>>(messages),
                CurrentPage = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPageCount
            };
        }

        public async Task<int> GetConversationPagesCountAsync(string userId1, string userId2, int pageSize = 50)
        {
            // Validate users exist
            var user1 = await _userManager.FindByIdAsync(userId1);
            if (user1 == null)
                throw new NotFoundException($"Sender with ID {userId1} not found");

            var user2 = await _userManager.FindByIdAsync(userId2);
            if (user2 == null)
                throw new NotFoundException($"Receiver with ID {userId2} not found");

            if (pageSize <= 0)
                throw new ArgumentException("Page size must be greater than zero");

            var totalMessages = await _messageRepo.GetConversationCountAsync(userId1, userId2);
            double noPages = Math.Ceiling((double)totalMessages / (double)pageSize);
            return (int)noPages;
        }
        public async Task<int> GetUserMessagesPagesCountAsync(string userId, int pageSize = 50)
        {
            var totalMessages = await _messageRepo.GetUserMessagesCountAsync(userId);
            double noPages = Math.Ceiling((double)totalMessages / (double)pageSize);
            return (int)noPages;
        }
       
        public async Task<MessagePaginationDto> GetUserMessagesAsync(string userId, int pageNumber = 1, int pageSize = 50)
        {
            // Validate and normalize inputs
            if (pageNumber <= 0)
                pageNumber = 1;
            if (pageSize <= 0 || pageSize > 100)
                pageSize = 50;

            // Get total pages
            var totalPageCount = await GetUserMessagesPagesCountAsync(userId, pageSize);

            // If no messages or page out of bounds, return empty with metadata
            if (totalPageCount == 0)
                return new MessagePaginationDto
                {
                    CurrentPage = 0,
                    PageSize = 0,
                    TotalPages = 0,
                    Messages = new List<GetMessageDTO>()
                };

            if (pageNumber > totalPageCount)
            {
                return new MessagePaginationDto
                {
                    CurrentPage = pageNumber,
                    PageSize = pageSize,
                    TotalPages = totalPageCount,
                    Messages = new List<GetMessageDTO>()
                };
            }

            // Fetch messages
            var messages = (await _messageRepo.GetUserMessagesAsync(userId, pageNumber, pageSize)).ToList();

            return new MessagePaginationDto
            {
                Messages = _mapper.Map<List<Message>, List<GetMessageDTO>>(messages),
                CurrentPage = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPageCount
            };
        }

        // w deh bardo w 7ot dto
        public async Task<IEnumerable<GetMessageDTO>> GetUnreadMessagesAsync(string currentUserId)
        {


            var messages = await _messageRepo.GetUnreadMessagesAsync(currentUserId);
            return _mapper.Map<List<GetMessageDTO>>(messages);
        }
        public async Task<int> GetUnreadCountAsync(string ResverID)
        {
            return await _messageRepo.GetUnreadCountAsync(ResverID);
        }

        public async Task MarkAsReadAsync(int messageId)
        {

            var message = await GetMessageByIdAsync(messageId);
                if (message == null)
                throw new NotFoundException($"Message with ID {messageId} not found");
            await _messageRepo.MarkAsReadAsync(messageId);
            await _messageRepo.SaveChangesAsync();
        }

        public async Task MarkConversationAsReadAsync(string CurrentUser, string OtherUser)
        {
            //check if Other users exist
            
            var Other= await _userManager.FindByIdAsync(OtherUser);
            if (Other == null)
                throw new NotFoundException($"Sender with ID {OtherUser} not found");

            await _messageRepo.MarkConversationAsReadAsync(CurrentUser, OtherUser);
            await _messageRepo.SaveChangesAsync();
        }

        public async Task<GetMessageDTO?> GetLastMessageAsync(string userId1, string userId2)
        {
            var user2 = await _userManager.FindByIdAsync(userId2);
            if (user2 == null)
                throw new NotFoundException($"Receiver with ID {userId2} not found");

            var message = await _messageRepo.GetLastMessageAsync(userId1, userId2);
            if (message == null) return null;

            return _mapper.Map<GetMessageDTO>(message);
        }
        public async Task DeleteMessageAsync(int messageId)
        {
            var message = await _messageRepo.GetByIdAsync(messageId);
            if (message == null)
                throw new NotFoundException($"Message with ID {messageId} not found");

            await _messageRepo.DeleteAsync(messageId);
            await _messageRepo.SaveChangesAsync();
        }
    }
}