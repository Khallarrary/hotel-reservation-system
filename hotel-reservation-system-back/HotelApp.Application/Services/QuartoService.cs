using HotelApp.Application.DTOs;
using HotelApp.Application.Exceptions;
using HotelApp.Application.Interfaces;
using HotelApp.Domain;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace HotelApp.Application.Services
{
    public class QuartoService
    {
        private readonly IQuartoRepository _repo;
        private readonly IReservaRepository _reservaRepo;

        public QuartoService(IQuartoRepository repo, IReservaRepository reservaRepo)
        {
            _repo = repo;
            _reservaRepo = reservaRepo;
        }

        public async Task<List<QuartoDto>> ObterTodos()
        {
            var quartos = await _repo.ObterTodosAsync();

            var resultado = new List<QuartoDto>();

            foreach (var quarto in quartos)
            {
                var reservas = await _reservaRepo.ObterPorQuartoAsync(quarto.Id);

                resultado.Add(new QuartoDto
                {
                    numero = quarto.Numero,
                    tipo = quarto.Tipo,
                    ReservaList = reservas.Select(r => new ReservaDto
                    {
                        CheckIn = r.CheckIn,
                        CheckOut = r.CheckOut,
                        NomeDoHospede = r.NomeDoHospede,
                        QuartoId = r.QuartoId
                    }).ToList()
                });
            }

            return resultado;
        }

        public async Task<Quarto?> ObterPorId(int id)
        {
            return await _repo.ObterPorIdAsync(id);
        }

        public async Task Criar(string numero, string tipo)
        {
            var numeroJaExiste = await _repo.ExisteNumeroAsync(numero);

            if (numeroJaExiste)
            {
                throw new ConflictException("Já existe um quarto com esse numero");
            }

            var quarto = new Quarto(numero, tipo);
            await _repo.AdicionarAsync(quarto);
        }

        public async Task RemoverPorId(int id) {

            var quarto = await _repo.ObterPorIdAsync(id);

            if (quarto == null)
                throw new NotFoundException("Quarto não encontrado");

            var reservasQuarto = await _reservaRepo.ObterPorQuartoAsync(id);

            if (reservasQuarto.Any())
            {
                throw new ConflictException("Quarto possui reserva. Não pode ser removido.");
            }

            await _repo.RemoverAsync(id);
        }

        public async Task<Quarto?> ObterPorNumero(string numero)
        {
            return await _repo.ObterPorNumeroAsync(numero);
        }

        public async Task RemoverPorNumero(string numero)
        {
            var quarto = await _repo.ObterPorNumeroAsync(numero);

            if (quarto == null)
            {
                throw new NotFoundException("Quarto não encontrado");
            }

         
            await RemoverPorId(quarto.Id);


        }
    }
}
