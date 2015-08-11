
namespace Common.Environment
{
    /// <summary>
    /// Хранилище данных Environment-а проекта
    /// </summary>
    public interface IEnvironmentInfoProvider
    {
        string UserName { get; }
    }
}
