using System;
using System.Collections.Generic;
using System.Text;

namespace HotelApp.Domain
{
    public class ContaReserva
    {
        public int Id { get; private set; }
        public int ReservaId { get; private set; }
        public ContaStatus Status { get; private set; }
        public DateTime DataAbertura { get; private set; }
        public DateTime? DataEncerramento { get; private set; }

        private ContaReserva() { }

        public ContaReserva(int reservaId)
        {
            if (reservaId <= 0)
            {
                throw new ArgumentException("Conta deve ser vinculada a uma reserva válida.");
            }

            ReservaId = reservaId;
            Status = ContaStatus.Aberta;
            DataAbertura = DateTime.UtcNow;
        }

        public void Encerrar()
        {
            if (Status == ContaStatus.Encerrada)
            {
                throw new ArgumentException("Conta já está encerrada.");
            }

            Status = ContaStatus.Encerrada;
            DataEncerramento = DateTime.UtcNow;
        }
    }
}
