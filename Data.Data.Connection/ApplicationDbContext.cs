using Data.Models.Interfaces;
using Data.Models.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Data.Data.Connection
{
    public class ApplicationDbContext : DbContext
    {
        private IConfigurationRoot configurationRoot;
        public DbSet<User> Users { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Seller> Sellers { get; set; }

        public DbSet<SellerCustomers> SellerCustomers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder dbContextOptionsBuilder)
        {
            configurationRoot = new ConfigurationBuilder().SetBasePath(Path.Combine
            (@"C:\Users\chich\AppData\Roaming\Microsoft\UserSecrets\83e24cc6-ea41-467b-9f75-7186383e377a")).AddJsonFile("secrets.json").Build();
            dbContextOptionsBuilder.UseSqlServer(configurationRoot.GetSection("DefaultConnection:ConnectionString").Value);
        }
        
        private void ApplyChanges()
        {
            var entries = this.ChangeTracker.Entries().Where(x => x.Entity is IAuditInfo).ToList();

            foreach (var entry in entries)
            {
                var entity = (IAuditInfo)entry.Entity;
                if (entry.State == EntityState.Added)
                {
                    entity.CreatedAt = DateTime.UtcNow;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entity.ModifiedAt = DateTime.UtcNow;
                }
                else
                {
                    entity.DeletedAt = DateTime.UtcNow;
                }
            }
        }
        public override int SaveChanges()
        {
            return SaveChanges(true);
        }
        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            this.ApplyChanges();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }
        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken= default)
        {
            this.ApplyChanges();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess);

        }
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return SaveChangesAsync(cancellationToken);
        }

    }
}
