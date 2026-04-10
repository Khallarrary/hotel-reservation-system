namespace HotelApp.Application.Services;
using HotelApp.Application.DTOs;
using HotelApp.Application.Interfaces;
using HotelApp.Application.Exceptions;
using HotelApp.Domain;
using System.Runtime.InteropServices.Marshalling;


/// <summary>
/// Serviço responsável por orquestrar as operações relacionadas a reservas.
/// Aplica regras de aplicação e coordena acesso aos repositórios.
/// </summary>
public class ReservaService
{
    private readonly IReservaRepository _repo;
    private readonly IQuartoRepository _quartoRepo;

    public ReservaService(IReservaRepository repo, IQuartoRepository quartoRepo)
    {
        _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        _quartoRepo = quartoRepo ?? throw new ArgumentNullException(nameof(quartoRepo));
    }


        /// <summary>
    /// Retorna todas as reservas cadastradas.
    /// </summary>
    public async Task<List<ReservaDto>> ListarReservas() 
    {
        var reserva = await _repo.ListarReservasAsync() ?? new List<Reserva>();

        return reserva.Select(reserva => new ReservaDto
        {
            
            CheckIn = reserva.CheckIn,
            CheckOut = reserva.CheckOut,
            NomeDoHospede = reserva.NomeDoHospede,
            QuartoId = reserva.QuartoId
            
             
        }).ToList();
    }


    /// <summary>
    /// Cria uma nova reserva validando:
    /// - Existência do quarto
    /// - Conflito de datas com reservas existentes
    /// </summary>
    public async Task CriarReserva(DateTime checkIn, DateTime checkOut, string nome, int quartoId)
    {
        checkIn = DateTime.SpecifyKind(checkIn, DateTimeKind.Utc);
        checkOut = DateTime.SpecifyKind(checkOut, DateTimeKind.Utc);

        // Validação básica de entrada
        if (string.IsNullOrWhiteSpace(nome))
        {
            throw new ArgumentException("Nome do Hospede é obrigatorio.");
        }

        if (quartoId <= 0) 
        {
            throw new ArgumentException("Quarto inválido.");
        }

        // Verifica se o quarto existe
        var quarto = await _quartoRepo.ObterPorIdAsync(quartoId);

        if (quarto == null)
        {
            throw new NotFoundException("Quarto nao existe");

        }

        // Busca reservas existentes do quarto
        var reservas = await _repo.ObterPorQuartoAsync(quartoId) ?? new List<Reserva>();

        // Cria nova reserva (validação adicional ocorre no domínio)
        var nova = new Reserva(checkIn, checkOut, nome, quartoId);


        // Verifica conflito com reservas existentes
        foreach (var reserva in reservas) {

            if (reserva is null)
                continue;

            if (nova.ConflitaCom(reserva)) 
            {
                throw new ConflictException("Quarto já ocupado nesse período");
            }
        
        }

        await _repo.AdicionarAsync(nova);

    }
}
