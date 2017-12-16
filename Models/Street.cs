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
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Addresses { get; set; }

        public int RegionId { get; set; }
        public Region Region { get; set; }

        public ICollection<Patient> Patients { get; set; }
    }
}
