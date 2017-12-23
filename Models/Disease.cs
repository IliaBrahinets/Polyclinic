using Microsoft.AspNetCore.Mvc;
using System;

using System.ComponentModel.DataAnnotations;

namespace Polyclinic.Models
{
    public class Disease
    {
        public int Id { get; set; }

        [Required(ErrorMessage ="Введите название")]
        [Remote(action: "isDiseaseExist", controller: "Registrator")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Введите описание")]
        public string Description { get; set; }
    }
}
