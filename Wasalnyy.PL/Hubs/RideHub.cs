using Microsoft.AspNetCore.SignalR;

namespace Wasalnyy.PL.Hubs
{
    public class RideHub:Hub
    {
        public void sendmessage(string name,string message)
        {
            //save in Db
            //broadcast to all clients
            Clients.All.SendAsync("new message",name,message);
        }
    }
}
