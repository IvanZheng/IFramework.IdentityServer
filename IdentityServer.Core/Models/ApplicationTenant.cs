using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer.Core.Models
{
    public class ApplicationTenant
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string NodeId { get; set; }
        public ApplicationTenant()
        {

        }

        public ApplicationTenant(string name)
        {
            Id = Guid.NewGuid().ToString();
            Name = name;
        }
    }
}
