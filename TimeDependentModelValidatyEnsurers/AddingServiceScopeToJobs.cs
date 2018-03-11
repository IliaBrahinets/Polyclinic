using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Polyclinic.TimeDependentModelValidatyEnsurers
{
    public class AddingServiceScopeToJob : IJobListener
    {
        public string Name
        {
            get
            {
                return "AddingServiceScopeToJobs";
            }
        }

        private IServiceProvider _serviceProvider;

        public AddingServiceScopeToJob(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task JobExecutionVetoed(IJobExecutionContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
            return null;
        }

        public async Task JobToBeExecuted(IJobExecutionContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
            context.JobDetail.JobDataMap.Put("ServiceScope", _serviceProvider.CreateScope());
        }

        public async Task JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException, CancellationToken cancellationToken = default(CancellationToken))
        {
            var ServiceScope = (IServiceScope)context.JobDetail.JobDataMap.Get("ServiceScope");

            if (jobException != null)
            {
                var ServiceProvider = ServiceScope.ServiceProvider;
                var logger = ServiceProvider.GetRequiredService<ILogger<AddingServiceScopeToJob>>();
                logger.LogError(jobException.ToString());
            }

           ServiceScope.Dispose();

        }
    }
}
