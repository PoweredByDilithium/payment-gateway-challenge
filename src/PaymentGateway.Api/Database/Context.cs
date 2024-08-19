using Microsoft.EntityFrameworkCore;
using PaymentGateway.Api.Entities;

namespace PaymentGateway.Api.Database
{
    public class Context(DbContextOptions<Context> options)
        : DbContext(options)
    {
        public DbSet<PaymentRequest> PaymentRequests { get; set; }
        public DbSet<PaymentResponse> PaymentResponses { get; set; }

        // Save created and updated timestamps for auditing purposes, can also include logs of users who made changes
        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            var addedEntities = ChangeTracker.Entries().Where(e => e.State == EntityState.Added).ToList();
            addedEntities.ForEach(e => e.Property("CreatedDateTime").CurrentValue = DateTime.UtcNow);

            var editedEntities = ChangeTracker.Entries().Where(e => e.State == EntityState.Modified).ToList();
            editedEntities.ForEach(e => e.Property("UpdatedDateTime").CurrentValue = DateTime.UtcNow);

            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            optionsBuilder.UseInMemoryDatabase("InMemoryDb");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<PaymentResponse>(entity => entity.HasKey(e => e.Id));
        }
    }
}
