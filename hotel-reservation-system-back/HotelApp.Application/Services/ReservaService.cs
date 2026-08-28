namespace HotelApp.Application.Services;
using HotelApp.Application.DTOs;
using HotelApp.Application.Interfaces;
using HotelApp.Application.Exceptions;
using HotelApp.Domain;
using System.Runtime.InteropServices.Marshalling;
using System.Globalization;


/// <summary>
/// Serviço responsável por orquestrar as operações relacionadas a reservas.
/// Aplica regras de aplicação e coordena acesso aos repositórios.
/// </summary>
public class ReservaService
{
    private readonly IReservaRepository _repo;
    private readonly IQuartoRepository _quartoRepo;
    private readonly IContaReservaRepository _contaRepo;
    private readonly IHotelContexto _hotelContexto;
    private readonly ITransacao _transacao;
    private readonly IConsultaSaldoConta _consultaSaldo;
    private readonly IHotelRepository _hotelRepositoy;
    private readonly IRelogioHotel _relogioHotel;
    


    public ReservaService(IReservaRepository repo, IQuartoRepository quartoRepo, IContaReservaRepository contaRepo, IHotelContexto hotelContexto, ITransacao transacao, IConsultaSaldoConta consultaSaldo, IRelogioHotel relogioHotel, IHotelRepository hotelRepo)
    {
        _repo = repo ?? throw new ArgumentNullException(nameof(repo)); 
        _quartoRepo = quartoRepo ?? throw new ArgumentNullException(nameof(quartoRepo));
        _contaRepo = contaRepo ?? throw new ArgumentNullException(nameof(contaRepo));
        _hotelContexto = hotelContexto;
        _transacao = transacao;
        _consultaSaldo = consultaSaldo;
        _relogioHotel = relogioHotel;
        _hotelRepositoy = hotelRepo;
    }


        /// <summary>
    /// Retorna todas as reservas cadastradas.
    /// </summary>
    public async Task<List<ReservaDto>> ListarReservas() 
    {
        var hotelId = _hotelContexto.ObterHotelId();

        if (!hotelId.HasValue)
        {
            throw new ForbiddenException("Hotel não encontrado");
        }

        var reserva = await _repo.ListarReservasAsync(hotelId.Value) ?? new List<Reserva>();

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

    private static void ValidarMesmaSolicitacao(Reserva reservaExistente, DateTime checkIn, DateTime checkOut, string nome, int quartoId)
    {
        var mesmaSolicitacao =
            reservaExistente.CheckIn == checkIn &&
            reservaExistente.CheckOut == checkOut &&
            reservaExistente.QuartoId == quartoId &&
            reservaExistente.NomeDoHospede == nome.Trim();

        if (!mesmaSolicitacao)
        {
            throw new ConflictException(
                "A chave de idempotência já foi utilizada em outra solicitação.");
        }
    }

    /// <summary>
    /// Cria uma nova reserva validando:
    /// - Existência do quarto
    /// - Conflito de datas com reservas existentes
    /// </summary>
    public async Task CriarReserva(DateTime checkIn, DateTime checkOut, string nome, int quartoId, Guid chaveIdempotencia)
    {
        checkIn = DateTime.SpecifyKind(checkIn, DateTimeKind.Utc);
        checkOut = DateTime.SpecifyKind(checkOut, DateTimeKind.Utc);

        var hotelId = _hotelContexto.ObterHotelId();

        if (!hotelId.HasValue)
        {
            throw new ForbiddenException("Hotel não encontrado");
        }

        // Validação básica de entrada
        if (string.IsNullOrWhiteSpace(nome))
        {
            throw new ArgumentException("Nome do Hospede é obrigatorio.");
        }

        if (quartoId <= 0)
        {
            throw new ArgumentException("Quarto inválido.");
        }

        if (chaveIdempotencia == Guid.Empty)
        {
            throw new ArgumentException("Chave de idempotência inválida.");
        }

        // Verifica se o quarto existe
        var quarto = await _quartoRepo.ObterPorIdAsync(quartoId, hotelId.Value);

        if (quarto == null)
        {
            throw new NotFoundException("Quarto nao existe");

        }

        var reservaExistente =
        await _repo.ObterPorChaveIdempotenciaAsync(
            chaveIdempotencia,
            hotelId.Value);

        if (reservaExistente != null)
        {
            ValidarMesmaSolicitacao(
                reservaExistente,
                checkIn,
                checkOut,
                nome,
                quartoId);

            return;
        }

        var dataAtual = await ObterDataAtualHotel(hotelId.Value);

        var nova = new Reserva(checkIn, checkOut, nome, quartoId, hotelId.Value, dataAtual, chaveIdempotencia);

        var reservas = await _repo.ObterReservasPorQuartoAsync(quartoId, hotelId.Value) ?? new List<Reserva>();

        foreach (var reserva in reservas)
        {
            if(reserva == null)
            {
                continue;
            }

            if(reserva.ChaveIdempotencia == chaveIdempotencia)
            {
                ValidarMesmaSolicitacao(
                reserva,
                checkIn,
                checkOut,
                nome,
                quartoId);

                return;
            }

            if (nova.ConflitaCom(reserva))
            {
                throw new ConflictException("Quarto ja ocupado nesse período");
            }

            
        }

        try
        {
            await _transacao.ExecutarAsync(async () =>
            {
                await _repo.AdicionarReservaAsync(nova);

                var conta = new ContaReserva(nova.Id);
                await _contaRepo.AdicionarAsync(conta);
            });
        }
        catch (ChaveIdempotenciaDuplicadaException)
        {
            var reservaProcessada =
                await _repo.ObterPorChaveIdempotenciaAsync(
                    chaveIdempotencia,
                    hotelId.Value);

            if (reservaProcessada == null)
            {
                throw;
            }

            ValidarMesmaSolicitacao(
                reservaProcessada,
                checkIn,
                checkOut,
                nome,
                quartoId);
        }
    }

    public async Task CriarReservaPorNumero(DateTime checkIn, DateTime checkOut, string nome, string numeroDoQuarto, Guid chaveIdempotencia)
    {

        var hotelId = _hotelContexto.ObterHotelId();

        if (!hotelId.HasValue)
        {
            throw new ForbiddenException("Hotel não encontrado");
        }


        var quarto = await _quartoRepo.ObterPorNumeroAsync(numeroDoQuarto, hotelId.Value);

        if(quarto == null)
        {
            throw new NotFoundException("Quarto nao existe");
        }
                       
        await CriarReserva(checkIn, checkOut, nome, quarto.Id, chaveIdempotencia);
    }

    public async Task DeletarReserva (int id)
    {
        var hotelId = _hotelContexto.ObterHotelId();

        if (!hotelId.HasValue)
        {
            throw new ForbiddenException("Hotel não encontrado");
        }

        var reserva = await _repo.ObterReservaPorIdAsync(id, hotelId.Value);

        if (reserva == null)
        {
            throw new NotFoundException("Reserva nao encontrada");
        }

        await _repo.DeletarReservaAsync(reserva);
    }

    public async Task RealizarCheckIn(int id) 
    {
        var hotelId = _hotelContexto.ObterHotelId();

        if (!hotelId.HasValue)
        {
            throw new ForbiddenException("Hotel não encontrado");
        }

        var dataAtual = await ObterDataAtualHotel(hotelId.Value);

        var reserva = await _repo.ObterReservaPorIdAsync(id, hotelId.Value);

        if(reserva == null)
        {
            throw new NotFoundException("Reserva nao encontrada");
        }

        reserva.RealizarCheckIn(dataAtual);

        await _repo.AtualizarReservaAsync(reserva);
    }

    public async Task RealizarCheckOut(int id)
    {
        var hotelId = _hotelContexto.ObterHotelId();

        if (!hotelId.HasValue)
        {
            throw new ForbiddenException("Hotel não encontrado");
        }

        var dataAtual = await ObterDataAtualHotel(hotelId.Value);

        var reserva = await _repo.ObterReservaPorIdAsync(id, hotelId.Value);

        if (reserva == null)
        {
            throw new NotFoundException("Reserva nao encontrada");
        }

        reserva.RealizarCheckOut(dataAtual);

        await _repo.AtualizarReservaAsync(reserva);
    }

    public async Task<ReservasPaginadasDto> ListarReservasPaginadas(ReservaConsultaDto consulta)
    {

        var hotelId = _hotelContexto.ObterHotelId();

        if (!hotelId.HasValue)
        {
            throw new ForbiddenException("Hotel não encontrado");
        }


        if (consulta.Pagina <= 0)
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

        
        var totalItens = await _repo.ContarReservasAsync(consulta, hotelId.Value);
        
        var totalDePaginas = (int)Math.Ceiling((decimal)totalItens / consulta.TamanhoPagina);

        var reservasPagina = await _repo.ListarReservasPaginadasAsync(consulta, hotelId.Value);

        var quartoIds = reservasPagina.Select(r => r.QuartoId).Distinct().ToList();

        var quartos = await _quartoRepo.ObterPorIdsAsync(quartoIds, hotelId.Value);

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

    public async Task CancelarReserva(int id)
    {
        var hotelId = _hotelContexto.ObterHotelId();
   
        if (!hotelId.HasValue)
        {
            throw new ForbiddenException("Hotel não encontrado");
        }

        var reserva = await _repo.ObterReservaPorIdAsync(id, hotelId.Value);

        if (reserva == null)
        {
            throw new NotFoundException("Reserva nao encontrada");
        }

        var saldo = await _consultaSaldo.ObterSaldoAsync(id);

        if (saldo != 0m)
        {
            throw new ConflictException(
                "A reserva não pode ser cancelada enquanto a conta possuir saldo.");
        }

        reserva.Cancelar();

        await _repo.AtualizarReservaAsync(reserva);
    }

    private async Task<DateOnly> ObterDataAtualHotel(int hotelId)
    {
        var hotel = await _hotelRepositoy.ObterPorIdAsync(hotelId);

        if (hotel == null)
        {
            throw new ForbiddenException("Hotel não encontrado");
        }

        var dataAtual = _relogioHotel.ObterDataAtual(hotel.FusoHorario);

        return dataAtual;
    }
}
 
