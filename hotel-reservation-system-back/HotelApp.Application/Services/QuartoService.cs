using HotelApp.Application.DTOs;
using HotelApp.Application.Exceptions;
using HotelApp.Application.Interfaces;
using HotelApp.Domain;


namespace HotelApp.Application.Services
{
    public class QuartoService
    {
        private readonly IQuartoRepository _repo;
        private readonly IReservaRepository _reservaRepo;
        private readonly IHotelContexto _hotelContexto;

        public QuartoService(IQuartoRepository repo, IReservaRepository reservaRepo, IHotelContexto hotelContexto)
        {
            _repo = repo;
            _reservaRepo = reservaRepo;
            _hotelContexto = hotelContexto;
        }

        public async Task<List<QuartoDto>> ObterTodos()
        {
            var hotelId = _hotelContexto.ObterHotelId();

            if (!hotelId.HasValue)
            {
                throw new ForbiddenException("Hotel não encontrado");
            }

            var quartos = await _repo.ObterTodosAsync(hotelId.Value);

            var resultado = new List<QuartoDto>();

            foreach (var quarto in quartos)
            {
                var reservas = await _reservaRepo.ObterReservasPorQuartoAsync(quarto.Id, hotelId.Value);

                resultado.Add(new QuartoDto
                {
                    numero = quarto.Numero,
                    tipo = quarto.Tipo,
                    ReservaList = reservas.Select(r => new ReservaDto
                    {
                        Id = r.Id,
                        CheckIn = r.CheckIn,
                        CheckOut = r.CheckOut,
                        NomeDoHospede = r.NomeDoHospede,
                        QuartoId = r.QuartoId,
                        Status = r.Status.ToString(),
                    }).ToList()
                });
            }

            return resultado;
        }

        public async Task<Quarto?> ObterPorId(int id)
        {
            var hotelId = _hotelContexto.ObterHotelId();

            if (!hotelId.HasValue)
            {
                throw new ForbiddenException("Hotel não encontrado");
            }

            return await _repo.ObterPorIdAsync(id, hotelId.Value);
        }

        public async Task Criar(string numero, string tipo)
        {            
            var hotelId = _hotelContexto.ObterHotelId();

            if (!hotelId.HasValue)
            {
                throw new ForbiddenException("Hotel não encontrado");
            }

            var numeroJaExiste = await _repo.ExisteNumeroAsync(numero, hotelId.Value);

            if (numeroJaExiste)
            {
                throw new ConflictException("Já existe um quarto com esse numero");
            }                      
                       
            var quarto = new Quarto(numero, tipo, hotelId.Value);
            await _repo.AdicionarAsync(quarto);
        }

        public async Task RemoverPorId(int id) {

            var hotelId = _hotelContexto.ObterHotelId();

            if (!hotelId.HasValue)
            {
                throw new ForbiddenException("Hotel não encontrado");
            }


            var quarto = await _repo.ObterPorIdAsync(id, hotelId.Value);

            if (quarto == null)
                throw new NotFoundException("Quarto não encontrado");

            var reservasQuarto = await _reservaRepo.ObterReservasPorQuartoAsync(id, hotelId.Value);

            if (reservasQuarto.Any())
            {
                throw new ConflictException("Quarto possui reserva. Não pode ser removido.");
            }

            await _repo.RemoverAsync(id, hotelId.Value);
        }

        public async Task<Quarto?> ObterPorNumero(string numero)
        {
            var hotelId = _hotelContexto.ObterHotelId();

            if (!hotelId.HasValue)
            {
                throw new ForbiddenException("Hotel não encontrado");
            }

            return await _repo.ObterPorNumeroAsync(numero, hotelId.Value);
        }

        public async Task RemoverPorNumero(string numero)
        {
            var hotelId = _hotelContexto.ObterHotelId();

            if (!hotelId.HasValue)
            {
                throw new ForbiddenException("Hotel não encontrado");
            }

            var quarto = await _repo.ObterPorNumeroAsync(numero, hotelId.Value);

            if (quarto == null)
            {
                throw new NotFoundException("Quarto não encontrado");
            }

         
            await RemoverPorId(quarto.Id);


        }
    }
}
