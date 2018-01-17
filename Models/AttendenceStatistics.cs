using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;


namespace Polyclinic.Models
{
    [JsonObject(MemberSerialization.OptOut)]
    public class AttendenceStatistics
    {
        [JsonIgnore]
        public int Id { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [JsonIgnore]
        public int DoctorId { get; set; }
        [JsonIgnore]
        public Doctor Doctor { get; set; }

        public int Appeared { get; set; }
        public int NonAppeared { get; set; }

    }
}
