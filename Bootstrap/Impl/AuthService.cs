using log4net;
using Common.Environment;
using Common.Services;
using Microsoft.Practices.Unity;

namespace Bootstrap.Impl
{
    public class AuthService : IAuthService
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(AuthService));
        private readonly IUnityContainer _container;
        private readonly IEnvironmentInfoProvider _environmentInfoProvider;

        public AuthService(IUnityContainer container, IEnvironmentInfoProvider environmentInfoProvider)
        {
            _container = container;
            _environmentInfoProvider = environmentInfoProvider;
        }

        public bool Authenticate(string login, string password)
        {
            SetUserNameIfNeed(null);
            //STUB: всегда авторизован
            return true;
        }

        private void SetUserNameIfNeed(string userName)
        {
            var userNameHandler = _environmentInfoProvider as IUserNameHandler;
            if (userNameHandler != null)
                userNameHandler.SetUserName(userName);
        }
    }
}
