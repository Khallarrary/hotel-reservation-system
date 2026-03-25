namespace HotelApp.Application.Services;
using HotelApp.Application.DTOs;
using HotelApp.Application.Interfaces;
using HotelApp.Application.Exceptions;
using HotelApp.Domain;
using System.Runtime.InteropServices.Marshalling;

public class ReservaService
{
    private readonly IReservaRepository _repo;
    private readonly IQuartoRepository _quartoRepo;

    public ReservaService(IReservaRepository repo, IQuartoRepository quartoRepo)
    {
        _repo = repo;
        _quartoRepo = quartoRepo;
    }

    public async Task<List<ReservaDto>> ListarReservas()
    {
        var reserva = await _repo.ListarReservasAsync();

        return reserva.Select(reserva => new ReservaDto
        {
            
            CheckIn = reserva.CheckIn,
            CheckOut = reserva.CheckOut,
            NomeDoHospede = reserva.NomeDoHospede,
            QuartoId = reserva.QuartoId
            
             
        }).ToList();
    }

    public async Task CriarReserva(DateTime checkIn, DateTime checkOut, string nome, int quartoId)
    {
        var quarto = await _quartoRepo.ObterPorIdAsync(quartoId);

        if (quarto == null)
        {
            throw new NotFoundException("Quarto nao existe");

        }


        var reservas = await _repo.ObterPorQuartoAsync(quartoId);
        

        var nova = new Reserva(checkIn, checkOut, nome, quartoId);

        foreach (var reserva in reservas) {

            if (nova.ConflitaCom(reserva)) 
            {
                throw new ConflictException("Quarto já ocupado nesse período");
            }
        
        }

        await _repo.AdicionarAsync(nova);

    }
}
