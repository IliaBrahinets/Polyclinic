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
            new Speciality{Name="Carson",CheckUpTime=DateTime.Parse("Thu, 01 May 2008 07:34:42 GMT")},
            new Speciality{Name="Carson",CheckUpTime=DateTime.Parse("Thu, 01 May 2008 07:34:42 GMT")},
            new Speciality{Name="Carson",CheckUpTime=DateTime.Parse("Thu, 01 May 2008 07:34:42 GMT")}
            };
            foreach (Speciality s in Specialities)
            {
                context.Specialities.Add(s);
            }

            
            context.SaveChanges();
        }
    }
}