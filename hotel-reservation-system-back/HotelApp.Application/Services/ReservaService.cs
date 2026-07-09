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
    private readonly IContaReservaRepository _contaRepo;
    

    public ReservaService(IReservaRepository repo, IQuartoRepository quartoRepo, IContaReservaRepository contaRepo)
    {
        _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        _quartoRepo = quartoRepo ?? throw new ArgumentNullException(nameof(quartoRepo));
        _contaRepo = contaRepo ?? throw new ArgumentNullException(nameof(contaRepo));
    }


        /// <summary>
    /// Retorna todas as reservas cadastradas.
    /// </summary>
    public async Task<List<ReservaDto>> ListarReservas() 
    {
        var reserva = await _repo.ListarReservasAsync() ?? new List<Reserva>();

        return reserva.Select(reserva => new ReservaDto
        {
            Id = reserva.Id,
            CheckIn = reserva.CheckIn,
            CheckOut = reserva.CheckOut,
            NomeDoHospede = reserva.NomeDoHospede,
            QuartoId = reserva.QuartoId,
            Status = reserva.Status.ToString(),
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
        var reservas = await _repo.ObterReservasPorQuartoAsync(quartoId) ?? new List<Reserva>();

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

        await _repo.AdicionarReservaAsync(nova);

        var conta = new ContaReserva(nova.Id);
        await _contaRepo.AdicionarAsync(conta);

    }

    public async Task CriarReservaPorNumero(DateTime checkIn, DateTime checkOut, string nome, string numeroDoQuarto)
    {

        var quarto = await _quartoRepo.ObterPorNumeroAsync(numeroDoQuarto);

        if(quarto == null)
        {
            throw new NotFoundException("Quarto nao existe");
        }
                       
        await CriarReserva(checkIn, checkOut, nome, quarto.Id);
    }

    public async Task DeletarReserva (int id)
    {
        var reserva = await _repo.ObterReservaPorIdAsync(id);

        if (reserva == null)
        {
            throw new NotFoundException("Reserva nao encontrada");
        }

        await _repo.DeletarReservaAsync(reserva);
    }

    public async Task RealizarCheckIn(int id) 
    {
        var reserva = await _repo.ObterReservaPorIdAsync(id);

        if(reserva == null)
        {
            throw new NotFoundException("Reserva nao encontrada");
        }

        reserva.RealizarCheckIn();

        await _repo.AtualizarReservaAsync(reserva);
    }

    public async Task RealizarCheckOut(int id)
    {
        var reserva = await _repo.ObterReservaPorIdAsync(id);

        if (reserva == null)
        {
            throw new NotFoundException("Reserva nao encontrada");
        }

        reserva.RealizarCheckOut();

        await _repo.AtualizarReservaAsync(reserva);
    }

    public async Task<ReservasPaginadasDto> ListarReservasPaginadas(ReservaConsultaDto consulta)
    {
        if(consulta.Pagina <= 0)
        {
            consulta.Pagina = 1;
        }

        if(consulta.TamanhoPagina <= 0)
        {
            consulta.TamanhoPagina = 10;
        }

        if(consulta.TamanhoPagina > 50)
        {
            consulta.TamanhoPagina = 50;
        }

        
        var totalItens = await _repo.ContarReservasAsync(consulta);
        
        var totalDePaginas = (int)Math.Ceiling((decimal)totalItens / consulta.TamanhoPagina);

        var reservasPagina = await _repo.ListarReservasPaginadasAsync(consulta);

        var quartoIds = reservasPagina.Select(r => r.QuartoId).Distinct().ToList();

        var quartos = await _quartoRepo.ObterPorIdsAsync(quartoIds);

        var quartosPorId = quartos.ToDictionary(q => q.Id, q => q.Numero);

        var itens = reservasPagina.Select(reserva => new ReservaDto
        {
            Id = reserva.Id,
            CheckIn = reserva.CheckIn,
            CheckOut = reserva.CheckOut,
            NomeDoHospede = reserva.NomeDoHospede,
            QuartoId = reserva.QuartoId,
            NumeroQuarto = quartosPorId.ContainsKey(reserva.QuartoId) ? quartosPorId[reserva.QuartoId] : "",
            Status = reserva.Status.ToString(),
        }).ToList();

        return new ReservasPaginadasDto
        {
            Itens = itens,
            Pagina = consulta.Pagina,
            TamanhoPagina = consulta.TamanhoPagina,
            TotalItens = totalItens,
            TotalPaginas = totalDePaginas

        };
    }
}
 
