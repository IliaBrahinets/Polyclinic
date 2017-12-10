using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;

using System.ComponentModel.DataAnnotations;

namespace Polyclinic.Models
{
    public class Street
    {
        public int ID { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Addresses { get; set; }
        public int RegionID { get; set; }
        public Region Region { get; set; }
    }
}
