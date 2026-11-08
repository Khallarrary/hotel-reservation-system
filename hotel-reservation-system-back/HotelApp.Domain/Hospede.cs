using System;
using System.Collections.Generic;
using System.Text;

namespace HotelApp.Domain
{
    public class Hospede
    {
        public int Id { get; private set; }
        public int HotelId { get; private set; }
        public string Nome { get; private set; }
        public TipoDocumento TipoDocumento { get; private set; }
        public string Documento { get; private set; }
        public string? Email { get; private set; }
        public string? Telefone { get; private set; }

        private Hospede() 
        {
            Nome = string.Empty;
            Documento = string.Empty;
        }

        public Hospede(int hotelId, string nome, TipoDocumento tipoDocumento, string documento, string? email, string? telefone)
        {
            if (hotelId <= 0) 
            {
                throw new ArgumentException("Hotel inválido.");
            }

            if (string.IsNullOrWhiteSpace(nome)) 
            { 
                throw new ArgumentException("Nome do hóspede é obrigatório."); 
            }
                      
            var documentoNormalizado = NormalizarDocumento(tipoDocumento, documento);

            HotelId = hotelId;
            Nome = nome.Trim();
            TipoDocumento = tipoDocumento;
            Documento = documentoNormalizado;
            Email = string.IsNullOrWhiteSpace(email)
            ? null
            : email.Trim().ToLowerInvariant();
            Telefone = string.IsNullOrWhiteSpace(telefone)
            ? null
            : new string(telefone.Where(char.IsDigit).ToArray());

            
        }

        public void AtualizarDados(string nome, TipoDocumento tipoDocumento, string documento, string? email, string? telefone)
        {
            if (string.IsNullOrWhiteSpace(nome))
            {
                throw new ArgumentException("Nome do hóspede é obrigatório.");
            }
           
            var documentoNormalizado = NormalizarDocumento(tipoDocumento, documento);
                       
            Nome = nome.Trim();
            TipoDocumento = tipoDocumento;
            Documento = documentoNormalizado;

            Email = string.IsNullOrWhiteSpace(email)
                ? null
                : email.Trim().ToLowerInvariant();

            Telefone = string.IsNullOrWhiteSpace(telefone)
                ? null
                : new string(
                    telefone.Where(char.IsDigit).ToArray());
        }

        public static string NormalizarDocumento(TipoDocumento tipoDocumento, string documento)
        {
            if (!Enum.IsDefined(tipoDocumento))
            {
                throw new ArgumentException("Tipo de documento inválido.");
            }

            if (string.IsNullOrWhiteSpace(documento))
            {
                throw new ArgumentException("Documento é obrigatório.");
            }

            var documentoNormalizado = new string(documento.Where(char.IsLetterOrDigit).ToArray()).ToUpperInvariant();


            if (documentoNormalizado.Length == 0)
            {
                throw new ArgumentException("Documento inválido.");
            }

            if (tipoDocumento == TipoDocumento.CPF &&(documentoNormalizado.Length != 11 || documentoNormalizado.Any(char.IsLetter)))
            {
                throw new ArgumentException("CPF inválido.");
            }

            return documentoNormalizado;
        }


    }
}
