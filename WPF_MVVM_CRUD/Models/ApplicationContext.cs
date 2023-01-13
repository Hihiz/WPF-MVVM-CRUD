using Microsoft.EntityFrameworkCore;
using System.Configuration;

namespace WPF_MVVM_CRUD.Models
{
    public class ApplicationContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }

        public ApplicationContext()
        {
            Database.EnsureDeleted();
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(ConfigurationManager.ConnectionStrings["ConnectionSqlite"].ToString());
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>().HasData(
                new Role[]
                {
                    new Role { Id=1, Name="Администратор"},
                    new Role { Id=2, Name="Менеджер"},
                    new Role { Id=3, Name="Клиент"}
                });

            modelBuilder.Entity<User>().HasData(
                new User[]
                {
                    new User { Id=1, Name="Tom",RoleId = 1},
                    new User { Id=2, Name="Alice",RoleId = 3},
                    new User { Id=3, Name="Rob",RoleId = 2 }
                });
        }
    }
}
