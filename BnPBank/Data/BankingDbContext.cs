using System;
using BnPBank.Models;
using Microsoft.EntityFrameworkCore;

namespace BnPBank.Data
{
    public class BankingDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseMySql("server=localhost;database=mydatabase;user=myuser;password=mypassword", new MySqlServerVersion(new Version(8, 0, 21)));
        //}

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase("TestDb");
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure primary key
            modelBuilder.Entity<User>().HasKey(u => u.Id);

            // Configure unique constraint
            modelBuilder.Entity<User>().HasIndex(u => u.Username).IsUnique();

            // Configure data validation rules
            modelBuilder.Entity<User>()
                .Property(u => u.Username)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<User>()
                .Property(u => u.HashedPassword)
                .IsRequired();

            modelBuilder.Entity<User>()
                .Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false);
        }
    }
}