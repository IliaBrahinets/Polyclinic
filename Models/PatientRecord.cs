// File:    PatientRecord.cs
// Author:  Bragas PC
// Created: 28 кастрычніка 2017 21:12:45
// Purpose: Definition of Class PatientRecord

using System;

public class PatientRecord
{
    public int Id { get; set; }

    public DateTime DateTime { get; set; }

    public int PatientId { get; set; }
    public Patient Patient { get; set; }

    public int DoctorId { get; set; }
    public Doctor Doctor { get; set; }

}