using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer.Core.Models
{
    public class ApplicationPermissionGrant
    {
        public string Id { get; set; }
        
        /// <summary>
        /// permission name
        /// </summary>
        [MaxLength(256)]
        public string Name { get; set; }

        /// <summary>
        /// Tenant id
        /// </summary>
        [MaxLength(128)]
        public string TenantId { get; set; }

        [MaxLength(128)]
        public string ScopeId { get; set; }
        /// <summary>
        /// 被授权对象
        /// </summary>
        [MaxLength(128)]
        public string ProviderKey { get; set; }

        /// <summary>
        /// 被授权对象类型
        /// </summary>
        [MaxLength(64)]
        public string ProviderType { get; set; }

        public ApplicationPermissionGrant()
        {

        }

        public ApplicationPermissionGrant(string name, string providerKey, string providerType, string scopeId, string tenantId = null)
        {
            Id = Guid.NewGuid().ToString();
            Name = name;
            ProviderKey = providerKey;
            ProviderType = providerType;
            ScopeId = scopeId;
            TenantId = tenantId;
        }

    }
}
