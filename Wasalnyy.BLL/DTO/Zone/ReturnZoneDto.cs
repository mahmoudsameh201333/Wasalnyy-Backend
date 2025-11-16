using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wasalnyy.DAL.Entities;

namespace Wasalnyy.BLL.DTO.Zone
{
    public class ReturnZoneDto
    {
        public Guid Id { get; set; }
        public IEnumerable<Coordinates> Coordinates { get; set; } = Enumerable.Empty<Coordinates>();
    }
}
