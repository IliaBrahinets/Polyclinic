// File:    Relieve.cs
// Author:  user
// Created: 1 лістапада 2017 14:53:06
// Purpose: Definition of Class Relieve

using System;
using System.Collections.Generic;

using Polyclinic.Models;

public class RelieveTime
{
    public int Id { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public String Description { get; set; }

    public int RelieveId;
    public Relieve Relieve;
}