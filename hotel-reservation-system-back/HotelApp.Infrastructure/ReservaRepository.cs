using System;
using System.Collections.Generic;
using System.Text;
using HotelApp.Domain;
using Microsoft.EntityFrameworkCore;
using HotelApp.Application.Interfaces;
using HotelApp.Application.DTOs;
using HotelApp.Application.Exceptions;
using Npgsql;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;


namespace HotelApp.Infrastructure
{
    public class ReservaRepository : IReservaRepository
    {
        private readonly AppDbContext _context;

        public ReservaRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Reserva>> ObterReservasPorQuartoAsync(int quartoId , int hotelId)
        {

            return await _context.Reservas
                .Where(r => r.QuartoId == quartoId && r.HotelId == hotelId)
                .ToListAsync();
        }

        public async Task AdicionarReservaAsync(Reserva reserva)
        {
            try
                {
                    await _context.Reservas.AddAsync(reserva);
                    await _context.SaveChangesAsync();
                }
            catch (DbUpdateException ex) when (ex.InnerException is PostgresException postgresException && postgresException.SqlState == PostgresErrorCodes.UniqueViolation && postgresException.ConstraintName == "IX_Reservas_HotelId_ChaveIdempotencia")
                {
                    throw new ChaveIdempotenciaDuplicadaException("A tentativa de criação de reserva ja foi processada.", ex);
                } 
            
               
        }

        public async Task<List<Reserva>> ListarReservasAsync(int hotelId)
        {
            return await _context.Reservas.Where(r => r.HotelId == hotelId).ToListAsync();
        }

        public async Task DeletarReservaAsync(Reserva reserva)
        {
            _context.Reservas.Remove(reserva);
            await _context.SaveChangesAsync();

        }

        public async Task<Reserva?> ObterReservaPorIdAsync(int id, int hotelId)
        {
            return await _context.Reservas.FirstOrDefaultAsync(r => r.Id == id && r.HotelId == hotelId);
        }

        public async Task AtualizarReservaAsync(Reserva reserva)
        {
            _context.Reservas.Update(reserva);
            await _context.SaveChangesAsync();
        }

        public async Task<int> ContarReservasAsync(ReservaConsultaDto consulta, int hotelId)
        {
            var query = MontarQuery(consulta, hotelId);

            return await query.CountAsync();
        }

        public async Task<List<Reserva>> ListarReservasPaginadasAsync(ReservaConsultaDto consulta, int hotelId)
        {
            var query = MontarQuery(consulta, hotelId);

            return await query
                .OrderBy(r => r.Id)
                .Skip((consulta.Pagina - 1) * consulta.TamanhoPagina)
                .Take(consulta.TamanhoPagina)
                .ToListAsync();
        }

        private IQueryable<Reserva> MontarQuery(ReservaConsultaDto consulta, int hotelId)
        {
            var query = _context.Reservas
                .Where(r => r.HotelId == hotelId)
                .AsQueryable();

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
                    && quarto.HotelId == hotelId
                    select reserva;
            }

            return query;
        }

        public async Task<Reserva?> ObterPorChaveIdempotenciaAsync(Guid chaveIdempotencia, int hotelId)
        {
            return await _context.Reservas.AsNoTracking().FirstOrDefaultAsync(r => r.ChaveIdempotencia == chaveIdempotencia && r.HotelId == hotelId);
        }

    }
   
}
