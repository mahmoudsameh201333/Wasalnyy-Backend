using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wasalnyy.DAL.Entities
{
    public class WasalnyyHubConnection
    {
        public string SignalRConnectionId { get; set; }
        public string UserId { get; set; }
        public virtual User User { get; set; }
    }
}
