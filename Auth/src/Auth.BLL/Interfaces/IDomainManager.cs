using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Auth.BLL.Interfaces
{
    public interface IDomainManager
    {
        Task<bool> VerifiDomain(string domain, string sid);
    }
}
