using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Polyclinic.Models
{
    public class Street
    {
        public int Id { get; set; }

        [Required(ErrorMessage ="Введите название")]
        public string Name { get; set; }

        private string addresses;
        [Required(ErrorMessage = "Введите адреса через запятую")]
        [RegularExpression("^([1-9][0-9]*[а-яА-Я]?,)*[1-9][0-9]*[а-яА-Я]?$", ErrorMessage = "Неправильный формат")]
        [Remote(action: "isStreetExist", controller: "Registrator", AdditionalFields = "Name,Id")]
        public string Addresses { get => addresses; set => addresses = value.ToUpper(); }

        public int RegionId { get; set; }
        public Region Region { get; set; }
    }
}
