using AppointmentAuthApi.Models;
using Microsoft.EntityFrameworkCore;

namespace AppointmentAuthApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Slot> Slots { get; set; }
        public DbSet<Appointment> Appointments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Appointment>()
               .HasIndex(a => a.SlotID)
               .IsUnique();

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.User)
                .WithMany(u => u.Appointments)
                .HasForeignKey(a => a.UserID);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Slot)
                .WithOne(s => s.Appointment)
                .HasForeignKey<Appointment>(a => a.SlotID);
        }
    }
}
