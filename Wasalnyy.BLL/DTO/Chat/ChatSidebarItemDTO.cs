using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wasalnyy.BLL.DTO.Chat
{
    public class ChatSidebarItemDTO
    {
        public string OtherUserID { get; set; }
        public string OtherUserName { get; set; }
        public string LastMessgeContet { get; set; }
        public DateTime? LastMessageDate { get; set; } 
        public int UnreadCount { get; set; } 
        public bool IsLastMessageFromMe { get; set; }
    }
}