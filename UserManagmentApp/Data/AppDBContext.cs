using Microsoft.EntityFrameworkCore;
using UserManagmentApp.Models;

namespace UserManagmentApp.Data
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options) 
        {
            
        }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(tb =>
            {
                tb.HasKey(col => col.Id);
                tb.Property(col => col.Id)
                .UseIdentityColumn()
                .ValueGeneratedOnAdd();

                tb.Property(col => col.Name).HasMaxLength(50);
                tb.Property(col => col.Email).HasMaxLength(150);
                tb.HasIndex(col => col.Email).IsUnique();
                tb.Property(col => col.Password);
                tb.Property(col => col.LastLogin);
                tb.Property(col => col.RegistrationTime).HasDefaultValueSql("getdate()");
                tb.Property(col => col.Status).HasDefaultValue("Active");
            });

            modelBuilder.Entity<User>().ToTable("Users");
        }
    }
}
