using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer.Models
{
    public class ApplicationMember
    {
        public string Id { get; set; }

        /// <summary>
        /// 用户标识
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// 租户内名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 租户标识
        /// </summary>
        public string TenantId { get; set; }

        /// <summary>
        /// 所属默认节点
        /// </summary>
        public string DefaultNodeId { get; set; }

        public ApplicationMember(){}

        public ApplicationMember(string userId, string name, string tenantId, string defaultNodeId)
        {
            Id = Guid.NewGuid().ToString(); 
            UserId = userId;
            Name = name;
            TenantId = tenantId;
            DefaultNodeId = defaultNodeId;
        }
    }
}
