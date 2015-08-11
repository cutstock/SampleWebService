using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Services
{
    public interface IAuthService
    {
        bool Authenticate(string login, string password);
    }
}
