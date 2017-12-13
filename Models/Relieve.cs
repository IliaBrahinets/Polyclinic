using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Polyclinic.Models
{
    //Relieve = Date + RelieveTime + Cabinet 
    public class Relieve
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }

        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; }

        public RelieveTime RelieveTime { get; set; }
        public int Cabinet { get; set; }
    }
}
