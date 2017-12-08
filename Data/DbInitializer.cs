using Polyclinic.Models;
using System;
using System.Linq;

namespace Polyclinic.Data
{
    public static class DbInitializer
    {
        public static void Initialize(PolyclinicContext context)
        {
            context.Database.EnsureCreated();

            // Look for any students.
            if (context.Specialities.Any())
            {
                return;   // DB has been seeded
            }

            var Specialities = new Speciality[]
            {
            new Speciality{Name="Carson",CheckUpTime=15},
            new Speciality{Name="Carson",CheckUpTime=20},
            new Speciality{Name="Carson",CheckUpTime=30}
            };
            foreach (Speciality s in Specialities)
            {
                context.Specialities.Add(s);
            }

            
            context.SaveChanges();
        }
    }
}