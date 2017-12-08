// File:    PatientCard.cs
// Author:  Bragas PC
// Created: 28 кастрычніка 2017 19:07:07
// Purpose: Definition of Class PatientCard

using System;
using System.Collections.Generic;

public class Patient : Man
{
    public DateTime BirthDate { get; set; }
    public bool Sex { get; set; }
    public string Address { get; set; }
    public DateTime CreationDate { get; set; }

    public Region Region { get; set; }

    public ICollection<DoctorVisit> DoctorVisits { get; set; }

    public ICollection<PatientRecord> PatientRecords { get; set; }

}