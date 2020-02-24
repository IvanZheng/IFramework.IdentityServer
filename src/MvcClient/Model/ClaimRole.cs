using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MvcClient.Model
{
    public class ClaimRole
    {
        public string Name { get; set; }
        public string ScopeId { get; set; }
        public string TenantId { get; set; }
    }
}
