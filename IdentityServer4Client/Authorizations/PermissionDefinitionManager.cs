using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Api.Authorizations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace IdentityServer4Client.Authorizations
{
    public class PermissionDefinitionManager : IPermissionDefinitionManager
    {
        private readonly Lazy<Dictionary<string, PermissionDefinition>> _lazyPermissionDefinitions;
        private readonly Lazy<Dictionary<string, PermissionGroupDefinition>> _lazyPermissionGroupDefinitions;

        private readonly IServiceProvider _serviceProvider;

        public PermissionDefinitionManager(IOptions<PermissionOptions> options,
                                           IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            Options = options.Value;

            _lazyPermissionDefinitions = new Lazy<Dictionary<string, PermissionDefinition>>(
                                                                                            CreatePermissionDefinitions,
                                                                                            true
                                                                                           );

            _lazyPermissionGroupDefinitions = new Lazy<Dictionary<string, PermissionGroupDefinition>>(
                                                                                                      CreatePermissionGroupDefinitions,
                                                                                                      true
                                                                                                     );
        }

        protected IDictionary<string, PermissionGroupDefinition> PermissionGroupDefinitions => _lazyPermissionGroupDefinitions.Value;

        protected IDictionary<string, PermissionDefinition> PermissionDefinitions => _lazyPermissionDefinitions.Value;

        protected PermissionOptions Options { get; }

        public virtual PermissionDefinition Get(string name)
        {
            var permission = GetOrNull(name);

            if (permission == null)
            {
                throw new Exception("Undefined permission: " + name);
            }

            return permission;
        }

        public virtual PermissionDefinition GetOrNull(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
            }

            return PermissionDefinitions.TryGetValue(name, out var value) ? value : default;
        }

        public virtual IReadOnlyList<PermissionDefinition> GetPermissions()
        {
            return PermissionDefinitions.Values.ToImmutableList();
        }

        public IReadOnlyList<PermissionGroupDefinition> GetGroups()
        {
            return PermissionGroupDefinitions.Values.ToImmutableList();
        }

        protected virtual Dictionary<string, PermissionDefinition> CreatePermissionDefinitions()
        {
            var permissions = new Dictionary<string, PermissionDefinition>();

            foreach (var groupDefinition in PermissionGroupDefinitions.Values)
            {
                foreach (var permission in groupDefinition.Permissions)
                {
                    AddPermissionToDictionary(permissions, permission);
                }
            }

            return permissions;
        }

        protected virtual void AddPermissionToDictionary(Dictionary<string, PermissionDefinition> permissions,
                                                         PermissionDefinition permission)
        {
            if (permissions.ContainsKey(permission.Name))
            {
                throw new Exception("Duplicate permission name: " + permission.Name);
            }

            permissions[permission.Name] = permission;
        }

        protected virtual Dictionary<string, PermissionGroupDefinition> CreatePermissionGroupDefinitions()
        {
            var groups = new Dictionary<string, PermissionGroupDefinition>();
            using (var scope = _serviceProvider.CreateScope())
            {
                var providers = Options.DefinitionProviders
                                       .Select(p => scope.ServiceProvider.GetRequiredService(p) as IPermissionDefinitionProvider)
                                       .ToList();

                foreach (var provider in providers)
                {
                    provider.Define(groups);
                }
            }

            return groups;
        }
    }
}