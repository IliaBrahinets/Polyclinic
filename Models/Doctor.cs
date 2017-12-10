// File:    Doctor.cs
// Author:  Андрей
// Created: 26 кастрычніка 2017 20:43:46
// Purpose: Definition of Class Doctor

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


using Polyclinic.Models;

public class Doctor : Man
{
    
    public int ChainedCabinet { get; set; }

    public Region Region { get; set; }


    public Speciality Speciality { get; set; }

    public ICollection<Relieve> Schedule { get; set; }

    public ICollection<PatientRecord> PatientRecords { get; set; }
}