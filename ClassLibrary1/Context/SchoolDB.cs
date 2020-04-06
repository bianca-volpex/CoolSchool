using DataContext.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace DataContext
{
    public class SchoolDB : DbContext
    {
        public SchoolDB(DbContextOptions<SchoolDB> options)
           : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<Student> Students { get; set; }
        public DbSet<Gender> Gender { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<UserInGroup> UserInGroups { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Gender>().HasData(
                new Gender { Id = 1,  Title = "Женский" },
                 new Gender {Id = 2, Title = "Мужской" }
                );

            builder.Entity<Student>()
                .HasMany(e => e.UserInGroups);

            builder.Entity<Student>()
               .HasIndex(u => u.CallSign)
               .IsUnique();

            builder.Entity<Group>()
               .HasMany(e => e.UserInGroups);
        }
    }
}