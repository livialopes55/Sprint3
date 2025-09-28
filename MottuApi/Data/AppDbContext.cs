using Microsoft.EntityFrameworkCore;
using MottuApi.Models;

namespace MottuApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Filial> Filiais => Set<Filial>();
        public DbSet<Patio> Patios => Set<Patio>();
        public DbSet<Moto> Motos => Set<Moto>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Filial>()
                .HasMany(f => f.Patios)
                .WithOne(p => p.Filial!)
                .HasForeignKey(p => p.FilialId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Patio>()
                .HasMany(p => p.Motos)
                .WithOne(m => m.Patio!)
                .HasForeignKey(m => m.PatioId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Moto>()
                .HasIndex(m => m.Placa)
                .IsUnique();
        }
    }
}