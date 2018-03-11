using System;
using System.Linq;

using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

using Quartz;
using Quartz.Impl;
using System.Collections.Specialized;
using System.Threading.Tasks;

using Polyclinic.Models;

namespace Polyclinic.Data
{
    public static class DbInitializer
    {
        public static void Initialize(PolyclinicContext context)
        {
            context.Database.EnsureCreated();
            InitData(context);

        }

        public static void InitData(PolyclinicContext context)
        {

            // Look for any students.
            if (context.Specialities.Any())
            {
                return;   // DB has been seeded
            }
            if (context.Drugs.Any())
            {
                return;   // DB has been seeded
            }
            if (context.Diseases.Any())
            {
                return;   // DB has been seeded
            }
            if (context.Regions.Any())
            {
                return;   // DB has been seeded
            }
            var Specialities = new Speciality[]
            {
            new Speciality{Name="Хирург",CheckUpTime=15},
            new Speciality{Name="Стоматолог",CheckUpTime=30},
            new Speciality{Name="Офтальмолог",CheckUpTime=20}
            };
            var Drugs = new Drug[]
            {
                 new Drug{Name="Ибуклин",Description="Описание"},
                 new Drug{Name="Боярышник",Description="Пей"}
            };
            var Diseases = new Disease[]
            {
                 new Disease{Name="Рак",Description="Всё плохо"},
                 new Disease{Name="Простуда",Description="Пей чай"}
            };

            var Region = new Region { };
            var Street = new Street
            {
                Name = "улица Пролетарская",
                Addresses = "1,2"
            };
            Region.Streets = new List<Street>();

            Region.Streets.Add(Street);


            foreach (Disease s in Diseases)
            {
                context.Diseases.Add(s);
            }
            foreach (Speciality s in Specialities)
            {
                context.Specialities.Add(s);
            }
            foreach (Drug s in Drugs)
            {
                context.Drugs.Add(s);
            }


            context.Regions.Add(Region);

            context.Streets.Add(Street);

            context.SaveChanges();
        }

    }
}