using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Polyclinic.Models
{
    public class Street
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Addresses { get; set; }

        public int RegionID { get; set; }
        public Region Region { get; set; }
    }
}
