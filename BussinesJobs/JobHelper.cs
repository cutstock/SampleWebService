using System;
using Quartz;

namespace BussinesJobs
{
    public static class JobHelper
    {
        public static TRes GetRequiredParameter<TRes>(IJobExecutionContext context, string parameterName)
        {
            var obj = context.MergedJobDataMap[parameterName];
            if (obj == null)
                throw new ArgumentNullException(parameterName);
            return (TRes)obj;
        }

        public static TRes GetNonRequiredParameter<TRes>(IJobExecutionContext context, string parameterName, TRes nullValue)
        {
            var obj = context.MergedJobDataMap[parameterName];
            if (obj == null)
                return nullValue;
            return (TRes)obj;
        }
    }
}
