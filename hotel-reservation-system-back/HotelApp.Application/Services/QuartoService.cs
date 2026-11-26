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
        private readonly ICategoriaQuartoRepository _categoriaRepo;

        public QuartoService(IQuartoRepository repo, IReservaRepository reservaRepo, IHotelContexto hotelContexto, ICategoriaQuartoRepository categoriaRepo)
        {
            _repo = repo;
            _reservaRepo = reservaRepo;
            _hotelContexto = hotelContexto;
            _categoriaRepo = categoriaRepo;
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

            var categorias = await _categoriaRepo.ObterTodasAsync(hotelId.Value);

            var categoriasPorId = categorias.ToDictionary(c => c.Id);

            foreach (var quarto in quartos)
            {
                var reservas = await _reservaRepo.ObterReservasPorQuartoAsync(quarto.Id, hotelId.Value);

                var reservasFiltradas = reservas.Where(r => r.Status == ReservaStatus.CheckIn || r.Status == ReservaStatus.Confirmada);

                var categoria = categoriasPorId[quarto.CategoriaQuartoId];

                resultado.Add(new QuartoDto
                {
                    numero = quarto.Numero,
                    tipo = categoria.Nome,
                    ReservaList = reservasFiltradas.Select(r => new ReservaDto
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

        public async Task<List<QuartoGestaoDto>> ObterParaGestao()
        {
            var hotelId = _hotelContexto.ObterHotelId()
                ?? throw new ForbiddenException("Hotel não encontrado.");
            var quartos = await _repo.ObterTodosAsync(hotelId);

            return quartos.OrderBy(quarto => quarto.Numero)
                .Select(quarto => new QuartoGestaoDto(
                    quarto.Id, quarto.Numero, quarto.CategoriaQuartoId, quarto.Ativo))
                .ToList();
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

        public async Task Criar(string numero, int categoriaId)
        {            
            var hotelId = _hotelContexto.ObterHotelId();

            if (!hotelId.HasValue)
            {
                throw new ForbiddenException("Hotel não encontrado");
            }

            var categoria = await _categoriaRepo.ObterPorIdAsync(categoriaId, hotelId.Value);

            if(categoria == null)
            {
                throw new ArgumentException("Quarto deve contem uma categoria.");
            }

            if (!categoria.Ativa)
                throw new ArgumentException("A categoria está inativa.");

            var quarto = new Quarto(numero, categoria.Id, hotelId.Value);
            
            var numeroJaExiste = await _repo.ExisteNumeroAsync(quarto.Numero, hotelId.Value);

            if (numeroJaExiste)
            {
                throw new ConflictException("Já existe um quarto com esse numero");
            }

            await _repo.AdicionarAsync(quarto);
        }

        public async Task Atualizar(int id, string numero, int categoriaId)
        {
            var hotelId = _hotelContexto.ObterHotelId()
                ?? throw new ForbiddenException("Hotel não encontrado.");
            var quarto = await _repo.ObterPorIdAsync(id, hotelId)
                ?? throw new NotFoundException("Quarto não encontrado.");

            if (string.IsNullOrWhiteSpace(numero))
                throw new ArgumentException("Informe o número do quarto.");

            var numeroNormalizado = numero.Trim();
            if (numeroNormalizado != quarto.Numero &&
                await _repo.ExisteNumeroAsync(numeroNormalizado, hotelId))
            {
                throw new ConflictException("Já existe um quarto com esse número.");
            }

            if (categoriaId != quarto.CategoriaQuartoId)
            {
                var categoria = await _categoriaRepo.ObterPorIdAsync(categoriaId, hotelId);
                if (categoria is null || !categoria.Ativa)
                    throw new ArgumentException("Selecione uma categoria ativa do hotel.");
            }

            quarto.Atualizar(numeroNormalizado, categoriaId);
            await _repo.SalvarAlteracoesAsync();
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
