// File:    PatientCard.cs
// Author:  Bragas PC
// Created: 28 кастрычніка 2017 19:07:07
// Purpose: Definition of Class PatientCard

using Microsoft.AspNetCore.Mvc;
using Polyclinic.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Patient : Man
{
    [DataType(DataType.Date)]
    [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd.MM.yyyy}")]
    public DateTime BirthDate { get; set; }

    [Required]
    public bool Sex { get; set; }

    public int? RegionId { get; set; }
    public Region Region { get; set; }

    [Required(ErrorMessage = "Введите название улицы")]
    public string StreetName { get; set; }

    //write a reg exp
    [Required(ErrorMessage = "Введите номер")]
    [RegularExpression("^[1-9]+[0-9]*[а-яА-Я]?$", ErrorMessage = "Неправильный формат")]
    [Remote(action: "IsStreetChain", controller: "Registrator", AdditionalFields = nameof(StreetName), HttpMethod ="POST")]
    public string HouseNumber { get; set; }

    [DataType(DataType.Date)]
    [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd.MM.yyyy}")]
    public DateTime CreationDate { get; set; }

    public ICollection<DoctorVisit> DoctorVisits { get; set; }

    public ICollection<PatientRecord> PatientRecords { get; set; }

    // override object.Equals
    public override bool Equals(object obj)
    {

        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        Patient patient = (Patient)obj;

        if (((BirthDate == patient.BirthDate) || patient.BirthDate.Equals(BirthDate))
            && ((Sex == patient.Sex) || patient.Sex.Equals(Sex))
            && ((StreetName == patient.StreetName) || patient.StreetName.Equals(StreetName))
            && ((HouseNumber == patient.HouseNumber) || patient.HouseNumber.Equals(HouseNumber))
            && patient.RegionId.Equals(RegionId))
            return base.Equals(obj) && true;
  
        return false;

    }

    // override object.GetHashCode
    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

}