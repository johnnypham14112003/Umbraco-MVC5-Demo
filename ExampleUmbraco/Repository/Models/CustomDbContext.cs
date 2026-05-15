using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace Repository.Models
{
    public partial class CustomDbContext : DbContext
    {
        public CustomDbContext() : base("name=CustomLocalDb") { }//name of connectString in Web.config

        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<Motor> Motors { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>()
                .ToTable("Account");

            modelBuilder.Entity<Motor>()
                .ToTable("Motor")
                .Property(e => e.Price)
                .HasPrecision(19, 4);

            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            base.OnModelCreating(modelBuilder);
        }
    }
}
