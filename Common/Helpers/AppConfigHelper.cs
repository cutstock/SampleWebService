using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Helpers
{
    public static class AppConfigHelper
    {
        public static T GetSettings<T>(string name, bool isRequared = true, T defaultValue = default(T))
        {
            var value = ConfigurationManager.AppSettings[name];
            if (string.IsNullOrEmpty(value))
            {
                if (isRequared)
                    throw new SettingsPropertyNotFoundException(string.Format("Requared parameter '{0}' is not set", name));

                return defaultValue;
            }

            return (T)Convert.ChangeType(value, typeof(T));
        }
    }
}
