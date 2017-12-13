// File:    Region.cs
// Author:  Андрей
// Created: 26 кастрычніка 2017 20:21:04
// Purpose: Definition of Class Region

using System;
using System.Collections.Generic;

using Polyclinic.Models;

public class Region
{
   public int Id { get; set; }

   public ICollection<Street> Streets { get; set; }
   public ICollection<Patient> Patients { get; set; }
   public ICollection<Doctor> Doctors { get; set; }
}