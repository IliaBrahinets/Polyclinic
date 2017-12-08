// File:    PatientRecord.cs
// Author:  Bragas PC
// Created: 28 кастрычніка 2017 21:12:45
// Purpose: Definition of Class PatientRecord

using System;

public class PatientRecord
{
    public int ID { get; set; }

    public DateTime DateTime { get; set; }

    public int PatientID { get; set; }
    public Patient Patient { get; set; }

    public int DoctorID { get; set; }
    public Doctor Doctor { get; set; }

}