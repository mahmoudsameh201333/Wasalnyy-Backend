using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wasalnyy.BLL.DTO.Driver
{
    public class DriverUpdateDto
    {
        public string? FullName { get; set; }
        public string? Image { get; set; }
        public string? PhoneNumber { get; set; }
        public string? License { get; set; }

        // update vec 

        public string? VehicleModel { get; set; }
        public string? VehiclePlate { get; set; }
        public string? Color { get; set; }

        public string? Make { get; set; }
        public int? Year { get; set; }
        public Capacity? Capacity { get; set; }
    }
}
