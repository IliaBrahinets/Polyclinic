// File:    Speciality.cs
// Author:  Андрей
// Created: 26 кастрычніка 2017 20:33:56
// Purpose: Definition of Class Speciality

using System;
using System.Collections.Generic;

public class Speciality
{
   public int ID { get; set; }
   public string Name { get; set; }
   public int CheckUpTime { get; set; }

    public ICollection<Doctor> Doctors { get; set; }
}