namespace HotelApp.Infrastructure;
using Microsoft.EntityFrameworkCore;
using HotelApp.Domain;
public class AppDbContext : DbContext
{
    public DbSet<Reserva> Reservas { get; set; }
    public DbSet<Quarto> Quartos { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

}
