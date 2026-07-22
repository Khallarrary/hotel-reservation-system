namespace HotelApp.Infrastructure;
using Microsoft.EntityFrameworkCore;
using HotelApp.Domain;
public class AppDbContext : DbContext
{
    public DbSet <Reserva> Reservas { get; set; }
    public DbSet <Quarto> Quartos { get; set; }
    public DbSet <ContaReserva> ContaReserva { get; set; }
    public DbSet <LancamentoConta> LancamentoConta { get; set; }
    public DbSet <Usuario> Usuario { get; set; }
    public DbSet <Hotel> Hoteis { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Quarto>()
            .HasIndex(q => q.Numero)
            .IsUnique();

        modelBuilder.Entity<Usuario>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<Hotel>()
            .HasIndex(h => h.Documento)
            .IsUnique();
    }

}
