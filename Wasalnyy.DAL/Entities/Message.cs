namespace Wasalnyy.DAL.Entities
{
    public class Message
    {
        public int Id { get; set; }
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
        public string Content { get; set; }
        public DateTime SentAt { get; set; }
        public bool IsRead { get; set; }
        public DateTime? ReadAt { get; set; }

        // Navigation properties
        public User Sender { get; set; }
        public User Receiver { get; set; }

        public Message()
        {
            SentAt = DateTime.Now;
            IsRead = false;
        }

        public Message(string senderId, string receiverId, string content)
        {
            SenderId = senderId;
            ReceiverId = receiverId;
            Content = content;
            SentAt = DateTime.Now;
            IsRead = false;
        }

        public void MarkAsRead()
        {
            IsRead = true;
            ReadAt = DateTime.Now;
        }
    }
}