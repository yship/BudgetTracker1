using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data
{
    public class BudgetTrackerDbContext : DbContext
    {
        public BudgetTrackerDbContext(DbContextOptions<BudgetTrackerDbContext> options) : base(options)
        {           }

            public DbSet<Users> Users { get; set; }
            public DbSet<Income> Incomes { get; set; }
            public DbSet<Expenditure> Expenditures { get; set; }
            

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
            modelBuilder.Entity<Users>(ConfigureUser);
            modelBuilder.Entity<Income>(ConfigureIncome);
            modelBuilder.Entity<Expenditure>(ConfigureExpenditure);
            
            }

            private void ConfigureUser(EntityTypeBuilder<Users> builder)
            {
                builder.ToTable("Users");
                builder.HasKey(u => u.Id);
                builder.HasIndex(u => u.Email).IsUnique();
                builder.Property(u => u.Email).HasMaxLength(100);
                builder.Property(u => u.FullName).HasMaxLength(100);
                builder.Property(u => u.Password).HasMaxLength(20);
                builder.Property(u => u.Joinedon).HasDefaultValueSql("getdate()");
                builder.Property(u => u.HashedPassword).HasMaxLength(256);
                builder.Property(u => u.Salt).HasMaxLength(256);


        }

                private void ConfigureIncome(EntityTypeBuilder<Income> builder)
                {
                    builder.ToTable("Income");
                    builder.HasKey(u => u.Id);
        
                    builder.Property(u => u.Amount).HasMaxLength(256);
                    builder.Property(u => u.Description).HasMaxLength(200);
                    builder.Property(u => u.Remarks).HasMaxLength(1000);
                    builder.Property(u => u.IncomeDate).HasDefaultValueSql("getdate()");
                }

                private void ConfigureExpenditure(EntityTypeBuilder<Expenditure> builder) {
                    builder.ToTable("Expenditure");
                    builder.HasKey(u => u.Id);
                    builder.Property(u => u.Amount).HasMaxLength(256);
                    builder.Property(u => u.Description).HasMaxLength(200);
                    builder.Property(u => u.Remarks).HasMaxLength(1000);
                    builder.Property(u => u.ExpDate).HasDefaultValueSql("getdate()");

                }
        
    }
}
