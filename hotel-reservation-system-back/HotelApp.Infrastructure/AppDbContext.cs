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
            .HasIndex(q => new { q.HotelId, q.Numero})
            .IsUnique();

        modelBuilder.Entity<Usuario>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<Hotel>()
            .HasIndex(h => h.Documento)
            .IsUnique();

        modelBuilder.Entity<Usuario>()
            .HasOne<Hotel>()
            .WithMany() 
            .HasForeignKey(u => u.HotelId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Quarto>()
            .HasOne<Hotel>()
            .WithMany()
            .HasForeignKey(q => q.HotelId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Reserva>()
            .HasOne<Hotel>()
            .WithMany()
            .HasForeignKey(r => r.HotelId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Reserva>()
            .HasIndex(r => new
            {
                r.HotelId,
                r.ChaveIdempotencia
            })
            .IsUnique()
            .HasFilter("\"ChaveIdempotencia\" IS NOT NULL");

    }

}
