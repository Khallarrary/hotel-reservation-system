using HotelApp.Application.DTOs;
using HotelApp.Application.Exceptions;
using HotelApp.Application.Interfaces;
using HotelApp.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelApp.Application.Services
{
    public class CategoriaQuartoService
    {
        private readonly ICategoriaQuartoRepository _repo;
        private readonly IHotelContexto _hotelContexto;

        public CategoriaQuartoService(ICategoriaQuartoRepository repo, IHotelContexto hotelContexto)
        {
            _repo = repo;
            _hotelContexto = hotelContexto;
        }

        public async Task Criar(string nome, string? descricao, int capacidadeFisica, int limiteComercial)
        {
            var hotelId = _hotelContexto.ObterHotelId();

            if (!hotelId.HasValue)
            {
                throw new ForbiddenException("Hotel não encontrado");
            }

            var categoria = new CategoriaQuarto(hotelId.Value, nome, descricao, capacidadeFisica, limiteComercial);

            var jaExiste = await _repo.ExisteNomeAsync(categoria.Nome, hotelId.Value);

            if (jaExiste)
            {
                throw new ConflictException("Já existe uma categoria com esse nome.");
            }

            await _repo.AdicionarAsync(categoria);
        }

        public async Task<List<CategoriaQuartoDto>> ObterTodas()
        {
            var hotelId = _hotelContexto.ObterHotelId();

            if (!hotelId.HasValue)
            {
                throw new ForbiddenException("Hotel não encontrado");
            }

            var categorias = await _repo.ObterTodasAsync(hotelId.Value);

            return categorias
                .Select(MapearDto)
                .ToList();
        }

        public async Task<List<CategoriaQuartoDto>> ObterAtivas()
        {
            var hotelId = _hotelContexto.ObterHotelId();

            if (!hotelId.HasValue)
            {
                throw new ForbiddenException("Hotel não encontrado.");
            }

            var categorias = await _repo.ObterAtivasAsync(hotelId.Value);

            return categorias
                .Select(MapearDto)
                .ToList();
        }

        private static CategoriaQuartoDto MapearDto(CategoriaQuarto categoria)
        {
            return new CategoriaQuartoDto
            {
                Id = categoria.Id,
                Nome = categoria.Nome,
                Descricao = categoria.Descricao,
                CapacidadeFisica = categoria.CapacidadeFisica,
                LimiteComercial = categoria.LimiteComercial,
                Ativa = categoria.Ativa
            };
        }

        public async Task Atualizar(int categoriaId, string nome, string? descricao, int capacidadeFisica, int limiteComercial)
        {
            var hotelId = _hotelContexto.ObterHotelId();

            if (!hotelId.HasValue)
            {
                throw new ForbiddenException("Hotel não encontrado.");
            }

            var categoria = await _repo.ObterPorIdAsync(categoriaId, hotelId.Value);

            if (categoria == null)
            {
                throw new NotFoundException("Categoria não encontrada.");
            }

            categoria.AtualizarDados(nome, descricao, capacidadeFisica, limiteComercial);

            var jaExiste = await _repo.ExisteNomeAsync(categoria.Nome, hotelId.Value, categoria.HotelId);

            if (jaExiste)
            {
                throw new ConflictException(
                    "Já existe uma categoria com esse nome.");
            }

            await _repo.AtualizarAsync(categoria);
        }

        public async Task Ativar(int categoriaId)
        {
            var hotelId = _hotelContexto.ObterHotelId();

            if (!hotelId.HasValue)
            {
                throw new ForbiddenException("Hotel não encontrado.");
            }

            var categoria = await _repo.ObterPorIdAsync(categoriaId, hotelId.Value);

            if (categoria == null)
            {
                throw new NotFoundException("Categoria não encontrada.");
            }

            categoria.Ativar();

            await _repo.AtualizarAsync(categoria);

         }

        public async Task Desativar(int categoriaId)
        {
            var hotelId = _hotelContexto.ObterHotelId();

            if (!hotelId.HasValue)
            {
                throw new ForbiddenException("Hotel não encontrado.");
            }

            var categoria = await _repo.ObterPorIdAsync(categoriaId, hotelId.Value);

            if (categoria == null)
            {
                throw new NotFoundException("Categoria não encontrada.");
            }

            categoria.Desativar();

            await _repo.AtualizarAsync(categoria);

        }

        public async Task<CategoriaQuartoDto> ObterPorId(int categoriaId)
        {
            var hotelId = _hotelContexto.ObterHotelId();

            if (!hotelId.HasValue)
            {
                throw new ForbiddenException("Hotel não encontrado.");
            }

            var categoria = await _repo.ObterPorIdAsync(categoriaId, hotelId.Value);

            if (categoria == null)
            {
                throw new NotFoundException("Categoria não encontrada.");
            }

            return MapearDto(categoria);
        }
    }
}
   
