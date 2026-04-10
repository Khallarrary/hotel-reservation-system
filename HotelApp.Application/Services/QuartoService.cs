using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using HotelApp.Domain;
using HotelApp.Application.DTOs;
using HotelApp.Application.Interfaces;

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

        public async Task<Quarto>? ObterPorId(int id)
        {
            return await _repo.ObterPorIdAsync(id);
        }

        public async Task Criar(string numero, string tipo)
        {
            var quarto = new Quarto(numero, tipo);
            await _repo.AdicionarAsync(quarto);
        }

        public async Task Remover(int id) { 
            await _repo.RemoverAsync(id);
        }
    }
}
