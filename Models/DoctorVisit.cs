// File:    DoctorVisit.cs
// Author:  Андрей
// Created: 26 кастрычніка 2017 19:49:04
// Purpose: Definition of Class DoctorVisit

using System;

public class DoctorVisit
{
    public int ID { get; set; }
    public DateTime DateTime { get; set; }
    public String Diagnosis { get; set; }
    public String Medicines { get; set; }
    public bool Attendence { get; set; }

    public int PatientID { get; set; }
    public Patient Patient { get; set; }

    public Doctor Doctor { get; set; }

}