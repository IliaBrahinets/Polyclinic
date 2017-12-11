// File:    Speciality.cs
// Author:  Андрей
// Created: 26 кастрычніка 2017 20:33:56
// Purpose: Definition of Class Speciality

using System;
using System.Collections.Generic;

using System.ComponentModel.DataAnnotations;

public class Speciality
{
    public int ID { get; set; }
    [Required(ErrorMessage = "Требуется название")]
    public string Name { get; set; }
    [Required(ErrorMessage = "Требуется длительность приема")]
    public int CheckUpTime { get; set; }

    public ICollection<Doctor> Doctors { get; set; }

    public override bool Equals(object obj)
    {
        Speciality spec = obj as Speciality;

        return (this.Name.ToLower() == spec.Name.ToLower());

    }
}