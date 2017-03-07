using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Auth.BLL.Interfaces;
using Auth.DAL.ProxyContext;
using Microsoft.EntityFrameworkCore;

namespace Auth.BLL
{
    public class DomainManager : IDomainManager
    {
        private readonly ProxyContext context;

        public DomainManager(ProxyContext context)
        {
            this.context = context;
        }

        public async Task<bool> VerifiDomain(string domain, string sid)
        {
            return await this.context.Domains.AnyAsync(x => x.Domain == domain && x.Sid == sid);
        }
    }
}
