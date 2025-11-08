using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wasalnyy.DAL.Entities
{
    [Owned]
    public class Coordinate
    {
        public decimal Lat { get; set; }
        public decimal Lng { get; set; }
    }
}
