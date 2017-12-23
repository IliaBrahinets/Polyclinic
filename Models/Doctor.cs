// File:    Doctor.cs
// Author:  Андрей
// Created: 26 кастрычніка 2017 20:43:46
// Purpose: Definition of Class Doctor

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Polyclinic.Models;
using System.ComponentModel.DataAnnotations.Schema;

using Polyclinic.CustomValidators;
using Microsoft.AspNetCore.Mvc;

public class Doctor : Man
{
    [Required(ErrorMessage = "Введите кабинет")]
    [MinValue(0,"Должен быть больше либо равен 0")]
    [Remote( action: "IsCabinetAvaliable", controller: "Registrator",AdditionalFields = nameof(Id) )]
    public int ChainedCabinet { get; set; }

    public int? RegionId { get; set; }
    public Region Region { get; set; }

    public int? SpecialityId { get; set; }
    public Speciality Speciality { get; set; }

    public ICollection<Relieve> Schedule { get; set; }

    public ICollection<PatientRecord> PatientRecords { get; set; }
}