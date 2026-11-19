using System;
using System.Collections.Generic;
using System.Text;

namespace HotelApp.Domain
{
    public class CategoriaQuarto
    {
        public int Id { get; private set; }
        public int HotelId { get; private set; }
        public string Nome { get; private set; } = null!;
        public string? Descricao { get; private set; }
        public int CapacidadeFisica { get; private set; }
        public int LimiteComercial { get; private set; }
        public bool Ativa { get; private set; }

        public CategoriaQuarto(int hotelId, string nome, string? descricao, int capacidadeFisica, int limiteComercial)
        {
            if (hotelId <= 0)
            {
                throw new ArgumentException("Hotel inválido.");
            }

            ValidarDados(nome, capacidadeFisica, limiteComercial);

            HotelId = hotelId;
            Nome = nome.Trim();
            Descricao = string.IsNullOrWhiteSpace(descricao) ? null : descricao.Trim();
            CapacidadeFisica = capacidadeFisica;
            LimiteComercial = limiteComercial;
            Ativa = true;
        }

        private CategoriaQuarto() { }

        public void AtualizarDados(string nome, string? descricao, int capacidadeFisica, int limiteComercial)
        {

            ValidarDados(nome, capacidadeFisica, limiteComercial);

            Nome = nome.Trim();
            Descricao = string.IsNullOrWhiteSpace(descricao) ? null : descricao.Trim();
            CapacidadeFisica = capacidadeFisica;
            LimiteComercial = limiteComercial;
        }

        public void Ativar()
        {
            Ativa = true;
        }
        public void Desativar()
        {
            Ativa = false;
        }

        private static void ValidarDados(string nome, int capacidadeFisica, int limiteComercial)
        {

            if (string.IsNullOrWhiteSpace(nome))
            {
                throw new ArgumentException("Nome da categoria é obrigatório.");
            }

            if (capacidadeFisica <= 0)
            {
                throw new ArgumentException("Capacidade deve ser maior que 0.");
            }

            if (limiteComercial <= 0)
            {
                throw new ArgumentException("Limite comercial deve ser maior que 0.");
            }

            if (limiteComercial > capacidadeFisica)
            {
                throw new ArgumentException("Limite comercial não pode superar a capacidade física.");
            }
        }
    }
}
