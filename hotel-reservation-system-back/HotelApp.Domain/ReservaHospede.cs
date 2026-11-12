using System;
using System.Collections.Generic;
using System.Text;

namespace HotelApp.Domain
{
    public class ReservaHospede
    {
        public int ReservaId { get; private set; }
        public int HospedeId { get; private set; }
        public int HotelId { get; private set; }
        public bool Titular { get; private set; }

        private ReservaHospede() { }
        internal ReservaHospede(int hospedeId, int hotelId, bool titular)
        {
           
            if (hospedeId <= 0)
            {
                throw new ArgumentException("Id de hospede inválido.");
            }

            if (hotelId <= 0)
            {
                throw new ArgumentException("Id de hotel inválido.");
            }

            HospedeId = hospedeId;
            HotelId = hotelId;
            Titular = titular;
        }

        internal void MarcarComoTitular()
        {
            if (!Titular)
            {
                Titular = true;
            }

        }

        internal void RemoverTitular()
        {
            if (Titular)
            {
                Titular = false;
            }
        }
    }
}