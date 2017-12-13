// File:    PatientCard.cs
// Author:  Bragas PC
// Created: 28 кастрычніка 2017 19:07:07
// Purpose: Definition of Class PatientCard

using Polyclinic.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class Patient : Man
{
    [DataType(DataType.Date)]
    [Display(Name = "Дата рождения")]
    public DateTime BirthDate { get; set; }

    public bool Sex { get; set; }

    public string Address { get; set; }

    [DataType(DataType.Date)]
    public DateTime CreationDate { get; set; }

    public int? RegionId { get; set; }
    public Region Region { get; set; }

    public ICollection<DoctorVisit> DoctorVisits { get; set; }

    public ICollection<PatientRecord> PatientRecords { get; set; }

}