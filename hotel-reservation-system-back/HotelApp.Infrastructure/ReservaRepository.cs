using System;
using System.Collections.Generic;
using System.Text;
using HotelApp.Domain;
using Microsoft.EntityFrameworkCore;
using HotelApp.Application.Interfaces;
using HotelApp.Application.DTOs;

namespace HotelApp.Infrastructure
{
    public class ReservaRepository : IReservaRepository
    {
        private readonly AppDbContext _context;

        public ReservaRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Reserva>> ObterReservasPorQuartoAsync(int quartoId)
        {

            return await _context.Reservas
                .Where(r => r.QuartoId == quartoId)
                .ToListAsync();
        }

        public async Task AdicionarReservaAsync(Reserva reserva)
        {
            await _context.Reservas.AddAsync(reserva);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Reserva>> ListarReservasAsync()
        {
            return await _context.Reservas.ToListAsync();
        }

        public async Task DeletarReservaAsync(Reserva reserva)
        {
            _context.Reservas.Remove(reserva);
            await _context.SaveChangesAsync();

        }

        public async Task<Reserva?> ObterReservaPorIdAsync(int id)
        {
            return await _context.Reservas.FindAsync(id);
        }

        public async Task AtualizarReservaAsync(Reserva reserva)
        {
            _context.Reservas.Update(reserva);
            await _context.SaveChangesAsync();
        }

        public async Task<int> ContarReservasAsync(ReservaConsultaDto consulta)
        {
            var query = MontarQuery(consulta);

            return await query.CountAsync();
        }

        public async Task<List<Reserva>> ListarReservasPaginadasAsync(ReservaConsultaDto consulta)
        {
            var query = MontarQuery(consulta);

            return await query
                .OrderBy(r => r.Id)
                .Skip((consulta.Pagina - 1) * consulta.TamanhoPagina)
                .Take(consulta.TamanhoPagina)
                .ToListAsync();
        }

        private IQueryable<Reserva> MontarQuery(ReservaConsultaDto consulta)
        {
            var query = _context.Reservas.AsQueryable();

            if (!string.IsNullOrWhiteSpace(consulta.NomeHospede))
            {
                query = query.Where(r => r.NomeDoHospede.Contains(consulta.NomeHospede));
            }

            if (consulta.ReservaId.HasValue)
            {
                query = query.Where(r => r.Id == consulta.ReservaId.Value);
            }

            if (!string.IsNullOrWhiteSpace(consulta.Status))
            {
                if (Enum.TryParse<ReservaStatus>(consulta.Status, true, out var status))
                {
                    query = query.Where(r => r.Status == status);
                }
            }

            if (!string.IsNullOrWhiteSpace(consulta.NumeroQuarto))
            {
                query =
                    from reserva in query
                    join quarto in _context.Quartos
                        on reserva.QuartoId equals quarto.Id
                    where quarto.Numero == consulta.NumeroQuarto
                    select reserva;
            }

            return query;
        }

    }
   
}
