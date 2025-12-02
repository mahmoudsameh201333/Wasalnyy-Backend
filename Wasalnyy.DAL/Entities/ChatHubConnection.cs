namespace Wasalnyy.DAL.Entities
{
    public class ChatHubConnection
    {
        public string UserId { get; set; }
        public string SignalRConnectionId { get; set; }

        // Navigation property
        public User User { get; set; }
    }
}