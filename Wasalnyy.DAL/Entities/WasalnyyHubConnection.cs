namespace Wasalnyy.DAL.Entities
{
    public class WasalnyyHubConnection
    {
        public string SignalRConnectionId { get; set; }
        public string UserId { get; set; }
        public virtual User User { get; set; }
    }
}
