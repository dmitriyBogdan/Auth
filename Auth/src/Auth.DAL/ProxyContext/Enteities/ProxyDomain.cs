using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Auth.DAL.ProxyContext.Enteities
{
    public class ProxyDomain
    {
        public int Id { get; set; }

        public string Domain { get; set; }

        public string Sid { get; set; }
    }
}
