using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using HotelApp.Domain;
using HotelApp.Application.Interfaces;

namespace HotelApp.Application.Services
{
    public class QuartoService
    {
        private readonly IQuartoRepository _repo;

        public QuartoService(IQuartoRepository repo)
        {
            _repo = repo;
        }

        public async Task <List<Quarto>> ObterTodos()
        {
            return await _repo.ObterTodosAsync();
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
