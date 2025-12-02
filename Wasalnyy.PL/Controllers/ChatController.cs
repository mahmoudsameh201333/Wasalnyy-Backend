using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Wasalnyy.BLL.DTO.Chat;
using Wasalnyy.BLL.Service;

namespace Wasalnyy.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Driver,Rider")]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;

        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        /// <summary>
        /// Get a specific message by ID
        /// </summary>
        [HttpGet("message/{messageId}")]
        [ProducesResponseType(typeof(GetMessageDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetMessageById(int messageId)
        {
            var message = await _chatService.GetMessageByIdAsync(messageId);

            if (message == null)
                return NotFound(new { message = "Message not found" });

            return Ok(message);
        }

        /// <summary>
        /// Get conversation between current user and another user with pagination
        /// </summary>
        [HttpGet("conversation/{otherUserId}")]
        [ProducesResponseType(typeof(MessagePaginationDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetConversation(
            string otherUserId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 50)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(currentUserId))
                return Unauthorized();

            var result = await _chatService.GetConversationAsync(
                currentUserId,
                otherUserId,
                pageNumber,
                pageSize);

            return Ok(result);
        }

        /// <summary>
        /// Get all messages for current user with pagination
        /// </summary>
        [HttpGet("messages")]
        [ProducesResponseType(typeof(MessagePaginationDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserMessages(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 50)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(currentUserId))
                return Unauthorized();

            var result = await _chatService.GetUserMessagesAsync(
                currentUserId,
                pageNumber,
                pageSize);

            return Ok(result);
        }

        /// <summary>
        /// Get all unread messages for current user
        /// </summary>
        [HttpGet("unread")]
        [ProducesResponseType(typeof(IEnumerable<GetMessageDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUnreadMessages()
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(currentUserId))
                return Unauthorized();

            var messages = await _chatService.GetUnreadMessagesAsync(currentUserId);

            return Ok(messages);
        }

        /// <summary>
        /// Get count of unread messages for current user
        /// </summary>
        [HttpGet("unread/count")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUnreadCount()
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(currentUserId))
                return Unauthorized();

            var count = await _chatService.GetUnreadCountAsync(currentUserId);

            return Ok(new { unreadCount = count });
        }

        /// <summary>
        /// Get the last message between current user and another user
        /// </summary>
        [HttpGet("conversation/{otherUserId}/last")]
        [ProducesResponseType(typeof(GetMessageDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetLastMessage(string otherUserId)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(currentUserId))
                return Unauthorized();

            var message = await _chatService.GetLastMessageAsync(currentUserId, otherUserId);

            if (message == null)
                return NotFound(new { message = "No messages found in this conversation" });

            return Ok(message);
        }

        /// <summary>
        /// Mark a specific message as read
        /// </summary>
        [HttpPut("message/{messageId}/read")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> MarkMessageAsRead(int messageId)
        {
            await _chatService.MarkAsReadAsync(messageId);

            return Ok(new { message = "Message marked as read" });
        }

        /// <summary>
        /// Mark entire conversation with another user as read
        /// </summary>
        [HttpPut("conversation/{otherUserId}/read")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> MarkConversationAsRead(string otherUserId)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(currentUserId))
                return Unauthorized();

            await _chatService.MarkConversationAsReadAsync(currentUserId, otherUserId);

            return Ok(new { message = "Conversation marked as read" });
        }

       

        /// <summary>
        /// Get total pages count for a conversation
        /// </summary>
        [HttpGet("conversation/{otherUserId}/pages")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetConversationPagesCount(
            string otherUserId,
            [FromQuery] int pageSize = 50)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(currentUserId))
                return Unauthorized();

            var totalPages = await _chatService.GetConversationPagesCountAsync(
                currentUserId,
                otherUserId,
                pageSize);

            return Ok(new { totalPages });
        }

      
    }
}