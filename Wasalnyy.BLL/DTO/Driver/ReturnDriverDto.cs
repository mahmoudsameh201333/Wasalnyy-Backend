using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wasalnyy.BLL.DTO.Driver
{
    public class ReturnDriverDto
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public Guid? ZoneId { get; set; }
        public string License { get; set; }
        public VehicleDto Vehicle { get; set; }
        public Coordinates? Coordinates { get; set; }

    }
}
