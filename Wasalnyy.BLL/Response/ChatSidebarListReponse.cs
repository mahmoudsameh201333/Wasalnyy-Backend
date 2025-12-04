using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wasalnyy.BLL.DTO.Chat;

namespace Wasalnyy.BLL.Response
{
    public class ChatSidebarListReponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public List<ChatSidebarItemDTO> ChatBarList { get; set; } 

        public ChatSidebarListReponse(bool isSuccess, string message)
        {
            this.IsSuccess = isSuccess;
            this.Message = message;
            this.ChatBarList = new List<ChatSidebarItemDTO>();
        }

        public ChatSidebarListReponse(List<ChatSidebarItemDTO> chats) 
        {
            this.IsSuccess = true;
            this.Message = "List Filled and returned Succesfully";
            this.ChatBarList = chats;
        }
    }
}