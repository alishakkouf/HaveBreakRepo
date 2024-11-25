using Microsoft.VisualBasic;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Reflection;
using System.Xml.Linq;
using HaveBreak.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace HaveBreak.Data
{
    public class HaveBreakDbContext : IdentityDbContext<UserAccount, UserRole, long>
    {
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Like> Likes { get; set; }

        private readonly IConfiguration _configuration;
        private readonly ICurrentUserService _currentUserService;
        private readonly int? _currentTenantId;
        private readonly bool _ignoreTenant;

        public HaveBreakDbContext(DbContextOptions<HaveBreakDbContext> options,
            IConfiguration configuration,
            ICurrentUserService currentUserService)
            : base(options)
        {
            _configuration = configuration;
            _currentUserService = currentUserService;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //To remove exsistd index to handle repeted indexes within tenants
            builder.Entity<UserRole>(b =>
            {
                b.Metadata.RemoveIndex([b.Property(r => r.NormalizedName).Metadata]);
            });

            builder.Entity<UserRole>()
                .HasIndex(x => new { x.NormalizedName }, "RoleNameIndex")
                .IsUnique();

            builder.Entity<UserAccount>()
                .HasMany(x => x.UserRoles)
                .WithMany(x => x.UserAccounts)
                .UsingEntity<IdentityUserRole<long>>(
                    r => r.HasOne<UserRole>().WithMany().HasForeignKey(x => x.RoleId),
                    l => l.HasOne<UserAccount>().WithMany().HasForeignKey(x => x.UserId));

            builder.Entity<UserRole>()
                .HasMany(x => x.Claims)
                .WithOne()
                .HasForeignKey(x => x.RoleId);


            foreach (var property in builder.Model.GetEntityTypes()
                         .SelectMany(t => t.GetProperties())
                         .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
            {
                property.SetPrecision(18);
                property.SetScale(2);
            }

        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker.Entries();

            foreach (var entry in entries)
            {
                if (entry.Entity is IAuditedEntity auditedEntity)
                {
                    var userId = _currentUserService.GetUserId();

                    if (entry.State == EntityState.Added)
                    {
                        auditedEntity.CreatedAt = DateTime.UtcNow;
                        auditedEntity.CreatedBy = userId;
                    }

                    if (entry.State == EntityState.Modified)
                    {
                        auditedEntity.ModifiedAt = DateTime.UtcNow;
                        auditedEntity.ModifiedBy = userId;
                    }
                }

            }

            return await base.SaveChangesAsync(cancellationToken);
        }

        private void FillAuditedAndChangeDeleted(EntityEntry entry, long? userId)
        {
            if (entry.Entity is IAuditedEntity audited)
            {
                switch (entry.State)
                {
                    case EntityState.Deleted:
                            entry.State = EntityState.Modified;
                            foreach (var entryProperty in entry.Properties)
                            {
                                if (entryProperty.Metadata.Name != nameof(audited.IsDeleted) &&
                                    entryProperty.Metadata.Name != nameof(audited.ModifiedAt) &&
                                    entryProperty.Metadata.Name != nameof(audited.ModifiedBy))
                                    entryProperty.IsModified = false;
                            }

                            audited.ModifiedAt = DateTime.UtcNow;
                            audited.ModifiedBy = userId;
                            audited.IsDeleted = true;
                        
                        break;
                    case EntityState.Modified:
                        audited.ModifiedAt = DateTime.UtcNow;
                        audited.ModifiedBy = userId;
                        Entry(audited).Property(p => p.CreatedAt).IsModified = false;
                        Entry(audited).Property(p => p.CreatedBy).IsModified = false;
                        break;
                    case EntityState.Added:
                        audited.CreatedAt = DateTime.UtcNow;
                        audited.CreatedBy = userId;
                        audited.IsDeleted = false;
                        break;
                }
            }
        }

    }
}
