using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Neocare.Domain.Entities;

namespace Neocare.Infrastructure.Data;

public class NeocareDbContext : IdentityDbContext
{
    public NeocareDbContext(DbContextOptions<NeocareDbContext> options) : base(options) { }

    public DbSet<Patient> Patients { get; set; } = null!;
    public DbSet<HealthProfessional> HealthProfessionals { get; set; } = null!;
    public DbSet<Appointment> Appointments { get; set; } = null!;
    public DbSet<Treatment> Treatments { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Patient>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
            entity.Property(e => e.CPF).IsRequired().HasMaxLength(11);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.Status).HasDefaultValue("Active");
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.CPF).IsUnique();
            entity.HasMany(e => e.Appointments).WithOne(a => a.Patient).HasForeignKey(a => a.PatientId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<HealthProfessional>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
            entity.Property(e => e.CPF).IsRequired().HasMaxLength(11);
            entity.Property(e => e.CRM).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Specialty).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Status).HasDefaultValue("Active");
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.CPF).IsUnique();
            entity.HasIndex(e => e.CRM).IsUnique();
            entity.HasMany(e => e.Appointments).WithOne(a => a.HealthProfessional).HasForeignKey(a => a.HealthProfessionalId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Reason).HasMaxLength(500);
            entity.Property(e => e.Status).HasDefaultValue("Scheduled");
            entity.HasOne(e => e.Patient).WithMany(p => p.Appointments).HasForeignKey(e => e.PatientId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.HealthProfessional).WithMany(h => h.Appointments).HasForeignKey(e => e.HealthProfessionalId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.Treatment).WithOne(t => t.Appointment).HasForeignKey<Treatment>(t => t.AppointmentId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Treatment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Description).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Status).HasDefaultValue("Active");
            entity.HasOne(e => e.Appointment).WithOne(a => a.Treatment).HasForeignKey<Treatment>(e => e.AppointmentId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.Patient).WithMany().HasForeignKey(e => e.PatientId).OnDelete(DeleteBehavior.Cascade);
        });
    }
}
