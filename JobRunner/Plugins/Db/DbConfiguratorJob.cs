using Quartz;

namespace JobRunner.Plugins.Db
{
    internal class DbConfiguratorJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            var mergedJobDataMap = context.MergedJobDataMap;
            var schcontext = context.Scheduler.Context;

            var contextPluginNameJobDataMap = mergedJobDataMap.GetString(DbConfiguratorPlugin.ContextPluginNameJobDataMap);
            if (string.IsNullOrEmpty(contextPluginNameJobDataMap))
                throw new JobExecutionException(string.Format("Required parameter '{0}' not found in JobDataMap.", DbConfiguratorPlugin.ContextPluginNameJobDataMap));

            var plugin = schcontext[contextPluginNameJobDataMap] as IDbConfiguratorPlugin;
            if (plugin == null)
                throw new JobExecutionException(string.Format("DbConfiguratorPlugin named '{0}' not found in SchedulerContext.", contextPluginNameJobDataMap));

            plugin.JobProcessing();
        }
    }
}
