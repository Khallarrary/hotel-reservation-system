using System;
using System.Collections.Generic;
using System.Text;

namespace HotelApp.Domain
{
    public class Hotel
    {
        public int Id { get; private set; }
        public string Nome { get; private set; }
        public string Documento { get; private set; }
        public bool Ativo { get; private set; }
        public string FusoHorario{ get; private set; }

        private Hotel() { }
        public Hotel(string nome, string documento, string fusoHorario) 
        {
            var normalizado = new string(documento.Where(char.IsDigit).ToArray());

            if (string.IsNullOrWhiteSpace(nome)) 
            {
                throw new ArgumentException("Nome é obrigatório");
            }

            if (string.IsNullOrWhiteSpace(normalizado))
            {
                throw new ArgumentException("Documento é obrigatório");
            }


            if (string.IsNullOrWhiteSpace(fusoHorario))
            {
                throw new ArgumentException("Fuso horario é obrigatório");
            }

            

            Nome = nome;
            Documento = normalizado;
            FusoHorario = fusoHorario;
            Ativo = true;
        }

        public void Ativar()
        {
            Ativo = true;
        }

        public void Desativar()
        {
            Ativo = false;
        }
    }
}
