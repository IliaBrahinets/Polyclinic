using Microsoft.AspNetCore.Mvc;
using System;

using System.Collections.Generic;

using System.ComponentModel.DataAnnotations;

namespace Polyclinic.Models
{
    public class Drug
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Введите название")]
        [Remote(action: "isDrugExist", controller: "Registrator")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Введите описание")]
        public string Description { get; set; }
    }
}
