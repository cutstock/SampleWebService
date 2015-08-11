using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Services
{
    public class ServiceContext
    {
        private readonly IDictionary<string, string> _args;

        public ServiceContext(IDictionary<string, string> args)
        {
            _args = args;
        }

        public string Get(string paramName)
        {
            var key = _args.Keys.FirstOrDefault(i => i.Equals(paramName, StringComparison.InvariantCultureIgnoreCase));
            return key == null ? null : _args[key];
        }
    }
}
