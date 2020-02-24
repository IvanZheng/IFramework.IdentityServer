using System;
using System.ComponentModel.DataAnnotations;

namespace IdentityServer.Core.Models
{
    public class ApplicationNode
    {
        public ApplicationNode() { }

        public ApplicationNode(string name, string tenantId, string type = null, string parentId = null, string nodeId = null)
        {
            Id = nodeId ?? Guid.NewGuid().ToString();
            Name = name;
            ParentId = parentId;
            TenantId = tenantId;
            Type = type;
        }

        public string Id { get; set; }
        public string ParentId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string TenantId { get; set; }
    }
}