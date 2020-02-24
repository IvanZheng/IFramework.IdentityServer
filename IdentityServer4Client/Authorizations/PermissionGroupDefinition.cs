using System.Collections.Generic;
using System.Collections.Immutable;

namespace IdentityServer4Client.Authorizations
{
    public class PermissionGroupDefinition
    {
        private readonly List<PermissionDefinition> _permissions;


        public PermissionGroupDefinition(string appId, string name)
        {
            AppId = appId;
            Name = name;
            _permissions = new List<PermissionDefinition>();
            Properties = new Dictionary<string, object>();
        }

        public string AppId { get; }
        public string Name { get; }
        public Dictionary<string, object> Properties { get; }
        public IReadOnlyList<PermissionDefinition> Permissions => _permissions.ToImmutableList();

        public object this[string name]
        {
            get => Properties.TryGetValue(name, out var value) ? value : default;
            set => Properties[name] = value;
        }


        public virtual PermissionDefinition AddPermission(string name, Dictionary<string, object> properties = null)
        {
            var permission = new PermissionDefinition(AppId, name, properties);

            _permissions.Add(permission);

            return permission;
        }
    }
}