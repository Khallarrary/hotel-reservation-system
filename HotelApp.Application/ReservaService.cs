namespace HotelApp.Application;
using HotelApp.Domain;

public class ReservaService
{
    private readonly IReservaRepository _repo;

    public ReservaService(IReservaRepository repo)
    {
        _repo = repo;
    }

    public async Task CriarReserva(DateTime checkIn, DateTime checkOut, string nome, int quartoId)
    {

        var reservas = await _repo.ObterPorQuartoAsync(quartoId);


        var nova = new Reserva(checkIn, checkOut, nome, quartoId);

        foreach (var reserva in reservas) {

            if (nova.ConflitaCom(reserva)) 
            {
                throw new Exception("Quarto já ocupado nesse período");
            }
        
        }

        await _repo.AdicionarAsync(nova);

    }
}
