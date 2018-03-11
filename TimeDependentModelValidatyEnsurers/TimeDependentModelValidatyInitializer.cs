using Quartz;
using Quartz.Impl;
using Quartz.Impl.Matchers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Polyclinic.TimeDependentModelValidatyEnsurers
{
    public class TimeDependentModelValidatyManager
    {
        private  IServiceProvider _serviceProvider;

        public TimeDependentModelValidatyManager(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            InitJobs();
        }

        private async void InitJobs()
        {
            IScheduler scheduler = await StdSchedulerFactory.GetDefaultScheduler();

            
            ITrigger EverydayTrigger = TriggerBuilder.Create()
                .StartNow()
                .WithDailyTimeIntervalSchedule(s =>
                    s.WithIntervalInHours(24)
                    .OnEveryDay()
                    .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(00, 52))
                    )
              .Build();

            IJobDetail job = JobBuilder.Create<PatientsRecords_RelievesEnsurer>().Build();
            await scheduler.ScheduleJob(job, EverydayTrigger);

            await scheduler.Start();

            //Adding Servise's Scope to Jobs
            scheduler.ListenerManager.AddJobListener(new AddingServiceScopeToJob(_serviceProvider), GroupMatcher<JobKey>.AnyGroup());
        }
    }
}
