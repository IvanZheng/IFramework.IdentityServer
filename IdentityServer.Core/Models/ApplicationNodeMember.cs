using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer.Core.Models
{
    public class ApplicationNodeMember
    {
        public string MemberId { get; set; }
        public string NodeId { get; set; }
        public string TenantId { get; set; }

        public ApplicationNodeMember(string memberId, string nodeId, string tenantId)
        {
            MemberId = memberId;
            NodeId = nodeId;
            TenantId = tenantId;
        }

        public ApplicationNodeMember(){}
    }
}
