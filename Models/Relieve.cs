using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Polyclinic.JsonConverters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Polyclinic.Models
{
    //Relieve = Date + RelieveTime + Cabinet 
    [JsonObject(MemberSerialization.OptOut)]
    public class Relieve
    {
        public int Id { get; set; }

        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime Date { get; set; }

        [JsonIgnore]
        public int DoctorId { get; set; }
        [JsonIgnore]
        public Doctor Doctor { get; set; }

        [JsonConverter(typeof(CustomJsonDateConverter), "HH:mm")]
        [DataType(DataType.Time)]
        [Required(ErrorMessage = "Введите начало смены")]
        public DateTime StartTime { get; set; }


        [JsonConverter(typeof(CustomJsonDateConverter), "HH:mm")]
        [DataType(DataType.Time)]
        [Required(ErrorMessage = "Введите конец смены")]
        public DateTime EndTime { get; set; }

        //public int Cabinet { get; set; }

        [JsonIgnore]
        public ICollection<PatientRecord> PatientRecords { get; set; }


    }
}
