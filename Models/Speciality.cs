// File:    Speciality.cs
// Author:  Андрей
// Created: 26 кастрычніка 2017 20:33:56
// Purpose: Definition of Class Speciality

using System;
using System.Collections.Generic;

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Polyclinic.CustomValidators;

public class Speciality
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Требуется название")]
    [RegularExpression("^[A-Яа-я]+$",ErrorMessage = "Должно состоять только из букв")]
    [Remote(action: "isSpecialityExist", controller: "Registrator")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Требуется длительность приема")]
    [MinValue(0, "Должен быть больше либо равен 0")]
    public int CheckUpTime { get; set; }

    public ICollection<Doctor> Doctors { get; set; }
}