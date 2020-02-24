using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace IdentityServer.Core.Models
{
    public enum RoleType
    {
        Role,
        ScopedRole,
        TenantRole,
        TenantScopedRole
    }

    public class ApplicationRole: IdentityRole
    {
        [MaxLength(256)]
        public virtual string TenantId { get; set; }
        [MaxLength(256)]
        public virtual string ScopeId { get; set; }
        public RoleType Type { get; set; }
        public ApplicationRole()
        {

        }

        public ApplicationRole(string name, string scopeId = null, string tenantId = null)
        {
            Name = name;
            TenantId = tenantId;
            ScopeId = scopeId;
            Type = GetType(scopeId, tenantId);
        }

        public static RoleType GetType(string scopeId, string tenantId)
        {
            if (scopeId == null && tenantId == null)
            {
                return RoleType.Role;
            }
            else if (scopeId != null && tenantId != null)
            {
                return RoleType.TenantScopedRole;
            }
            else if (scopeId == null)
            {
                return RoleType.TenantRole;
            }
            else
            {
                return RoleType.ScopedRole;
            }
        }
    }
}
