using ExpenseTracker.Data.Entities;
using ExpenseTracker.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ExpenseTracker.Data
{
    public class ExpenseTrackerDbContext : IdentityDbContext<AppUser, AppRole, string>
    {
        protected readonly IConfiguration _configuration;
        protected readonly IServiceProvider _serviceProvider;

        public ExpenseTrackerDbContext(DbContextOptions options, IConfiguration configuration, IServiceProvider serviceProvider) : base(options)
        {
            _configuration = configuration;
        _serviceProvider = serviceProvider;
        }

        public ExpenseTrackerDbContext(DbContextOptions options) : base(options)
        {
        }

        public void NotifyObservers<TEntity>(TEntity newEntity, TEntity oldEntityObject, EntityState state)
        {
            if (_serviceProvider == null)
            {
                return;
            }

            using (var scope = _serviceProvider.CreateScope())
            {
                var observers = scope.ServiceProvider.GetServices<IEntityObserver>();

                foreach (var observer in observers)
                {

                    if (newEntity != null && observer.CheckType(newEntity))
                    {
                        switch (state)
                        {
                            case EntityState.Added:
                                observer.EntityCreated(newEntity);
                                break;
                            case EntityState.Modified:
                                observer.EntityUpdated(oldEntityObject, newEntity);
                                break;
                            case EntityState.Deleted:
                                observer.EntityDeleted(oldEntityObject);
                                break;

                        }
                    }
                }

            }

        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            OnBeforeSaving();

            var modifiedEntries = ChangeTracker.Entries()
           .Where(e => e.State == EntityState.Modified)
           .ToList();

            foreach (var entry in modifiedEntries)
            {
                NotifyObservers(entry.Entity, entry.OriginalValues.ToObject(), EntityState.Modified);

            }

            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            OnBeforeSaving();

            var modifiedEntries = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Modified)
                .ToList();

            foreach (var entry in modifiedEntries)
            {
                NotifyObservers(entry.Entity, entry.OriginalValues.ToObject(), EntityState.Modified);
            }


            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }


        private void OnBeforeSaving()
        {
            var entries = ChangeTracker.Entries();
            foreach (var entry in entries)
            {
                if (entry.Entity is ITimeTrackable trackableEntity)
                {
                    switch (entry.State)
                    {
                        case EntityState.Deleted:
                        case EntityState.Modified:
                            trackableEntity.UpdatedAt = DateTime.Now.ToUniversalTime();
                            break;

                        case EntityState.Added:
                            DateTime dt = DateTime.Now.ToUniversalTime();
                            trackableEntity.CreatedAt = dt;
                            trackableEntity.UpdatedAt = dt;
                            break;
                    }
                }
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // hide warning promots on migrations
            optionsBuilder.ConfigureWarnings(warnings =>
                    warnings.Ignore(CoreEventId.PossibleIncorrectRequiredNavigationWithQueryFilterInteractionWarning));
        }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<AppUser>(entity =>
            {
                entity.ToTable("Users");
            });

            builder.Entity<AppRole>(entity =>
            {
                entity.ToTable("Roles");
            });

        }

        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<AppRole> AppRoles { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Expense> Expenses { get; set; }
    }
}

