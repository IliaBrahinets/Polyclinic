// File:    Man.cs
// Author:  Bragas PC
// Created: 28 кастрычніка 2017 17:07:25
// Purpose: Definition of Class Man

using System;

using System.ComponentModel.DataAnnotations;

public class Man
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Введите имя")]
    [RegularExpression("^[А-Я][а-я]*(-[А-Я][а-я]*)?$", ErrorMessage = "Имя должно состоять из букв и начинаться с большой буквы")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Введите фамилию")]
    [RegularExpression("^[А-Я][а-я]*(-[А-Я][а-я]*)?$", ErrorMessage = "Фамилия должна состоять из букв и начинаться с большой буквы")]
    public string Surname { get; set; }

    [Required(ErrorMessage = "Введите отчество")]
    [RegularExpression("^[А-Я][а-я]*", ErrorMessage = "Отчество должно состоять из букв и начинаться с большой буквы")]
    public string Lastname { get; set; }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        Man man = (Man)obj;
        if ((Name == man.Name) || man.Name.Equals(Name))
            if ((Surname == man.Surname) || man.Surname.Equals(Surname))
                if ((Lastname == man.Lastname) || man.Lastname.Equals(Lastname))
                    return true;

        return false;

    }
}