
namespace Common.Environment.Impl
{
    public class SvcEnvironmentInfoProvider : IEnvironmentInfoProvider, IUserNameHandler
    {
        public string UserName { get; private set; }

        public void SetUserName(string userName)
        {
            UserName = userName;
        }
    }
}
