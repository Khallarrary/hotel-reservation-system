using HotelApp.Application.DTOs;
using HotelApp.Application.Exceptions;
using HotelApp.Application.Interfaces;
using HotelApp.Domain;

namespace HotelApp.Application.Services
{
    public class HospedeService
    {
        private readonly IHospedeRepository _repository;
        private readonly IHotelContexto _hotelContexto;
        private readonly IHotelRepository _hotelRepository;

        public HospedeService(IHospedeRepository repository, IHotelContexto hotelContexto, IHotelRepository hotelRepository)
        {
            _repository = repository;
            _hotelContexto = hotelContexto;
            _hotelRepository = hotelRepository;
        }



        public async Task<HospedeDto> Criar (string nome, string tipoDocumento, string documento, string? email, string? telefone)
        {
            var hotelId = _hotelContexto.ObterHotelId();

            if (!hotelId.HasValue)
            {
                throw new ForbiddenException("Hotel não encontrado");
            }

            var tipoValido = Enum.TryParse<TipoDocumento>(tipoDocumento, true, out var tipoDocumentoConvertido);
            
            if (!tipoValido)
            {
                throw new ArgumentException("Tipo de documento inválido.");
            }

            await ValidarHotelAtivoAsync(hotelId.Value);

            var hospede = new Hospede(hotelId.Value, nome, tipoDocumentoConvertido, documento, email, telefone);

            var hospedeJaExiste = await _repository.ObterPorDocumentoAsync(tipoDocumentoConvertido, hospede.Documento, hotelId.Value);

            if(hospedeJaExiste != null)
            {
                throw new ConflictException("Hóspede já cadastrado com este documento.");
            }                       

            await _repository.AdicionarAsync(hospede);

            return new HospedeDto
            {
                Id = hospede.Id,
                Nome = hospede.Nome,
                TipoDocumento = hospede.TipoDocumento.ToString(),
                Documento = hospede.Documento,
                Email = hospede.Email,
                Telefone = hospede.Telefone
            };
        }
        

        public async Task<HospedeDto> ObterPorDocumentoAsync(string tipoDocumento, string documento)
        {
            var hotelId = _hotelContexto.ObterHotelId();

            if (!hotelId.HasValue)
            {
                throw new ForbiddenException("Hotel não encontrado");
            }

            var tipoValido = Enum.TryParse<TipoDocumento>(tipoDocumento, true, out var tipoDocumentoConvertido);

            if (!tipoValido)
            {
                throw new ArgumentException("Tipo de documento inválido.");
            }

            await ValidarHotelAtivoAsync(hotelId.Value);

            var documentoNormalizado = Hospede.NormalizarDocumento(tipoDocumentoConvertido, documento);

            var hospede = await _repository.ObterPorDocumentoAsync(tipoDocumentoConvertido, documentoNormalizado, hotelId.Value);

            if (hospede is null)
            {
                throw new NotFoundException("Hóspede não encontrado.");
            }


            return new HospedeDto
            {
                Id = hospede.Id,
                Nome = hospede.Nome,
                TipoDocumento = hospede.TipoDocumento.ToString(),
                Documento = hospede.Documento,
                Email = hospede.Email,
                Telefone = hospede.Telefone
            };


        }

        public async Task Atualizar(int id, string nome, string tipoDocumento, string documento, string? email, string? telefone)
        {
            var hotelId = _hotelContexto.ObterHotelId();

            if (!hotelId.HasValue)
            {
                throw new ForbiddenException("Hotel não encontrado.");
            }

            await ValidarHotelAtivoAsync(hotelId.Value);

            var hospede = await _repository.ObterPorIdAsync(id, hotelId.Value);

            if (hospede is null)
            {
                throw new NotFoundException("Hóspede não encontrado.");
            }

            var tipoValido = Enum.TryParse<TipoDocumento>(
                tipoDocumento,
                true,
                out var tipoDocumentoConvertido);

            if (!tipoValido)
            {
                throw new ArgumentException("Tipo de documento inválido.");
            }

            var documentoNormalizado = Hospede.NormalizarDocumento(tipoDocumentoConvertido, documento);

            var hospedeComDocumento = await _repository.ObterPorDocumentoAsync(tipoDocumentoConvertido, documentoNormalizado, hotelId.Value);


            if (hospedeComDocumento is not null && hospedeComDocumento.Id != hospede.Id)
            {
                throw new ConflictException("Documento utilizado por outro hóspede.");
            }

            hospede.AtualizarDados(nome, tipoDocumentoConvertido, documento, email, telefone);

            await _repository.AtualizarAsync(hospede);
        }


        private async Task ValidarHotelAtivoAsync(int hotelId)
        {
            var hotel = await _hotelRepository.ObterPorIdAsync(hotelId);

            if (hotel is null)
            {
                throw new NotFoundException("Hotel não encontrado.");
            }

            if (!hotel.Ativo)
            {
                throw new ForbiddenException("Hotel inativo.");
            }
        }
    }
}
