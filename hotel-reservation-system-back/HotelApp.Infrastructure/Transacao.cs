using HotelApp.Application.Interfaces;

namespace HotelApp.Infrastructure
{
    public class Transacao : ITransacao
    {
        private readonly AppDbContext _context;

        public Transacao(AppDbContext context)
        {
            _context = context;
        }

        public async Task ExecutarAsync(Func<Task> operacao)
        {
            await using var transacao = await _context.Database.BeginTransactionAsync();

            try
            {
                await operacao();
                await transacao.CommitAsync();
            }
            catch
            {
                await transacao.RollbackAsync();
                throw;
            }
        }
    }
}
