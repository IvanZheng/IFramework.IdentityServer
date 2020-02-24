using IdentityServer.Core.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer.Core.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<ApplicationTenant> Tenants { get; set; }
        public DbSet<ApplicationNode> Nodes { get; set; }
        public DbSet<ApplicationPermissionGrant> PermissionGrants { get; set; }
        public DbSet<ApplicationMember> Members { get; set; }
        public DbSet<ApplicationNodeMember> NodeMembers { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

            var applicationRoleBuilder = builder.Entity<ApplicationRole>();
            var index = applicationRoleBuilder.HasIndex(u => new {u.NormalizedName}).Metadata;
            applicationRoleBuilder.Metadata.RemoveIndex(index.Properties);

            applicationRoleBuilder.HasIndex(r => new {r.NormalizedName, r.ScopeId})
                                  .HasName("RoleNameIndex")
                                  .HasFilter("([NormalizedName] IS NOT NULL)")
                                  .IsUnique();

            builder.Entity<ApplicationPermissionGrant>()
                   .HasIndex(g => new {g.Name, g.ProviderType, g.ProviderKey, g.ScopeId, g.TenantId})
                   .HasName("IX_ApplicationPermissionGrant_Name_ProviderType_ProviderKey");

            builder.Entity<ApplicationMember>()
                   .HasIndex(m => new {m.UserId, m.TenantId})
                   .IsUnique();

            builder.Entity<ApplicationNodeMember>()
                   .HasKey(e => new {e.MemberId, e.NodeId});
        }
    }
}