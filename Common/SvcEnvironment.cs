using System;
using Common.Environment;

namespace Common
{
    public static class SvcEnvironment
    {
        private static IEnvironmentInfoProvider _provider;

        public static void Init(IEnvironmentInfoProvider provider)
        {
            _provider = provider;
        }

        /// <summary>
        /// Current logged user name
        /// </summary>
        public static string UserName
        {
            get
            {
                CheckInitialization();
                return _provider.UserName;
            }
        }

        private static void CheckInitialization()
        {
            if (_provider == null)
                throw new Exception("You must init OmsEnvironment before use it");
        }
    }
}
