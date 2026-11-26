using System;
using System.Collections.Generic;
using System.Configuration.Assemblies;
using System.Text;

namespace HotelApp.Domain
{
    public class Quarto
    {
        public int Id { get; private set; }
        public string Numero { get; private set; }
        public int CategoriaQuartoId { get; private set; }
        public int HotelId { get; private set; }
        public bool Ativo { get; private set; }


        private Quarto() { }
        public Quarto(string numero, int categoriaQuartoId, int hotelId) 
        { 
           

            if (string.IsNullOrWhiteSpace(numero))
            {
                throw new ArgumentException("O quarto deve conter um numero");
            }

            Numero = numero.Trim();

            if (categoriaQuartoId < 1)
            {
                throw new ArgumentException("O quarto deve conter uma categoria.");
            }

            if (hotelId < 1) 
            {
                throw new ArgumentException("O quarto deve conter um hotel");
            }

            Ativo = true;
            CategoriaQuartoId = categoriaQuartoId;
            HotelId = hotelId;
        }

        public void Ativar()
        {
            Ativo = true;
        }

        public void Desativar()
        {
            Ativo = false;
        }

        public void Atualizar(string numero, int categoriaQuartoId)
        {
            if (string.IsNullOrWhiteSpace(numero))
            {
                throw new ArgumentException("O quarto deve conter um numero");
            }
                       
            if (categoriaQuartoId < 1)
            {
                throw new ArgumentException("O quarto deve conter uma categoria.");
            }

            Numero = numero.Trim();
            CategoriaQuartoId = categoriaQuartoId;
        }
    }
}
