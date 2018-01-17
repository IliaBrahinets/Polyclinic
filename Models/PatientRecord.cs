// File:    PatientRecord.cs
// Author:  Bragas PC
// Created: 28 кастрычніка 2017 21:12:45
// Purpose: Definition of Class PatientRecord

using System;
using Newtonsoft.Json;
using Polyclinic.Models;
using Polyclinic.JsonConverters;

[JsonObject(MemberSerialization.OptIn)]
public class PatientRecord
{
    [JsonProperty]
    public int Id { get; set; }
    [JsonProperty]
    public int? PatientId { get; set; }
    public Patient Patient { get; set; }
  
    public int DoctorId { get; set; }
    public Doctor Doctor { get; set; }
    [JsonProperty]
    [JsonConverter(typeof(CustomJsonDateConverter), "HH:mm")]
    public DateTime DateTime { get; set; }
}