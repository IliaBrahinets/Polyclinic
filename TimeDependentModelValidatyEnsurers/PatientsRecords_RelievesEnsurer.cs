using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polyclinic.Data;
using Polyclinic.Models;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Polyclinic.TimeDependentModelValidatyEnsurers
{
    public class PatientsRecords_RelievesEnsurer : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            //Retrive service's scope and needed serices
            JobDataMap dataMap = context.JobDetail.JobDataMap;

            IServiceProvider serviceProvider = ((IServiceScope)dataMap.Get("ServiceScope")).ServiceProvider;

            PolyclinicContext db = serviceProvider.GetRequiredService<PolyclinicContext>();

            ILogger logger = serviceProvider.GetRequiredService<ILogger<PatientsRecords_RelievesEnsurer>>();

            await Task(db);

            logger.LogInformation("PatientsRecords_RelievesEnsurer Executed!");
        }

        private async Task Task(PolyclinicContext db)
        {
            List<Relieve> OutOfDate = await db.Relieves.Where(x => (x.Date < DateTime.UtcNow)).ToListAsync();

            foreach (Relieve Relieve in OutOfDate)
            {
                bool NotConfirmedRecordsExist = await db.PatientRecords.AnyAsync(x => (x.PatientId != null
                                                                                && x.DoctorId == Relieve.DoctorId
                                                                                && x.DateTime >= Relieve.StartTime
                                                                                && x.DateTime < Relieve.EndTime));

                if (!NotConfirmedRecordsExist)
                {
                    db.Relieves.Remove(Relieve);
                }

                db.PatientRecords.RemoveRange(
                    db.PatientRecords.Where(x => (x.PatientId == null
                                                && x.DoctorId == Relieve.DoctorId
                                                && x.DateTime >= Relieve.StartTime
                                                && x.DateTime < Relieve.EndTime)));


            }

            await db.SaveChangesAsync();
        }
    }
}
