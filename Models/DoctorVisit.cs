// File:    DoctorVisit.cs
// Author:  Андрей
// Created: 26 кастрычніка 2017 19:49:04
// Purpose: Definition of Class DoctorVisit

using System;
using System.ComponentModel.DataAnnotations;

public class DoctorVisit
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Введите диагноз!")]
    public String Diagnosis { get; set; }
    [Required(ErrorMessage = "Введите рекомендации!")]
    public String Medicines { get; set; }

    [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd.MM.yyyy HH:mm}")]
    public DateTime DateTime { get; set; }

    public int PatientId { get; set; }
    public Patient Patient { get; set; }

    public int DoctorId { get; set; }
    public Doctor Doctor { get; set; }

}