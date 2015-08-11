using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Quartz;

namespace JobRunner.Core
{
    [ContractClass(typeof(IConfigurationProviderContract))]
    public interface IConfigurationProvider
    {
        IDictionary<IJobDetail, Quartz.Collection.ISet<ITrigger>> GetJobs(string schedulerName);

        void GetChangedJobs(string schedulerName, DateTime lastCheckDate, out JobKey[] deletedJobs,
            out IDictionary<IJobDetail, Quartz.Collection.ISet<ITrigger>> changedJobs);
    }

    [ContractClassFor(typeof(IConfigurationProvider))]
    abstract class IConfigurationProviderContract : IConfigurationProvider
    {
        IDictionary<IJobDetail, Quartz.Collection.ISet<ITrigger>> IConfigurationProvider.GetJobs(string schedulerName)
        {
            Contract.Requires(!string.IsNullOrEmpty(schedulerName));
            Contract.Ensures(Contract.Result<IDictionary<IJobDetail, Quartz.Collection.ISet<ITrigger>>>() != null);
            throw new NotImplementedException();
        }

        void IConfigurationProvider.GetChangedJobs(string schedulerName, DateTime lastCheckDate, out JobKey[] deletedJobs, out IDictionary<IJobDetail, Quartz.Collection.ISet<ITrigger>> changedJobs)
        {
            Contract.Requires(!string.IsNullOrEmpty(schedulerName));
            Contract.Ensures(Contract.ValueAtReturn(out changedJobs) != null);
            throw new NotImplementedException();
        }
    }
}
