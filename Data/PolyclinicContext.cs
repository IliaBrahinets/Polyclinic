using Polyclinic.Models;
using Microsoft.EntityFrameworkCore;

namespace Polyclinic.Data
{
    public class PolyclinicContext : DbContext
    {
        public PolyclinicContext(DbContextOptions<PolyclinicContext> options) : base(options)
        {

        }

        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Relieve> Relieves { get; set; }
        public DbSet<RelieveTime> RelieveTimes { get; set; }
        public DbSet<Speciality> Specialities { get; set; }

        public DbSet<Drug> Drugs { get; set; }
        public DbSet<Disease> Diseases { get; set; }

        public DbSet<PatientRecord> PatientRecords { get; set; }
        public DbSet<Region> Regions { get; set; }
        public DbSet<Street> Streets { get; set; }

        public DbSet<Patient> Patients { get; set; }
        public DbSet<DoctorVisit> DoctorVisits { get; set; }

        public DbSet<AttendenceStatistics> AttendenceStatistics { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
         
            modelBuilder.Entity<Patient>()
                 .HasOne(e => e.Region)
                 .WithMany(e => e.Patients)
                 .Metadata.DeleteBehavior = DeleteBehavior.SetNull;

            modelBuilder.Entity<PatientRecord>()
                .HasOne(e => e.Patient)
                .WithMany(e => e.PatientRecords)
                .Metadata.DeleteBehavior = DeleteBehavior.Cascade;

            modelBuilder.Entity<Doctor>()
                  .HasOne(e => e.Region)
                  .WithMany(e => e.Doctors)
                  .Metadata.DeleteBehavior = DeleteBehavior.SetNull;

            modelBuilder.Entity<Doctor>()
                 .HasOne(e => e.Speciality)
                 .WithMany(e => e.Doctors)
                 .Metadata.DeleteBehavior = DeleteBehavior.SetNull;
        }
    }
}