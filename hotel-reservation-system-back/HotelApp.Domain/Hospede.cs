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
                      
            var nomeNormalizado = nome.Trim();

            var documentoNormalizado =
                NormalizarDocumento(tipoDocumento, documento);

            var emailNormalizado = string.IsNullOrWhiteSpace(email)
                ? null
                : email.Trim().ToLowerInvariant();

            var telefoneNormalizado = string.IsNullOrWhiteSpace(telefone)
                ? null
                : new string(telefone.Where(char.IsDigit).ToArray());

            ValidarLimites(
                nomeNormalizado,
                documentoNormalizado,
                emailNormalizado,
                telefoneNormalizado);


            HotelId = hotelId;
            Nome = nomeNormalizado;
            TipoDocumento = tipoDocumento;
            Documento = documentoNormalizado;
            Email = emailNormalizado;
            Telefone = telefoneNormalizado;

        }

        public void AtualizarDados(string nome, TipoDocumento tipoDocumento, string documento, string? email, string? telefone)
        {
            if (string.IsNullOrWhiteSpace(nome))
            {
                throw new ArgumentException("Nome do hóspede é obrigatório.");
            }

            var nomeNormalizado = nome.Trim();

            var documentoNormalizado =
                NormalizarDocumento(tipoDocumento, documento);

            var emailNormalizado = string.IsNullOrWhiteSpace(email)
                ? null
                : email.Trim().ToLowerInvariant();

            var telefoneNormalizado = string.IsNullOrWhiteSpace(telefone)
                ? null
                : new string(telefone.Where(char.IsDigit).ToArray());

            ValidarLimites(
                nomeNormalizado,
                documentoNormalizado,
                emailNormalizado,
                telefoneNormalizado);


            Nome = nomeNormalizado;
            TipoDocumento = tipoDocumento;
            Documento = documentoNormalizado;
            Email = emailNormalizado;
            Telefone = telefoneNormalizado;
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

        private static void ValidarLimites(string nome, string documento, string? email, string? telefone)
        {
            if (nome.Length > 150)
            {
                throw new ArgumentException(
                    "Nome deve possuir no máximo 150 caracteres.");
            }

            if (documento.Length > 30)
            {
                throw new ArgumentException(
                    "Documento deve possuir no máximo 30 caracteres.");
            }

            if (email?.Length > 254)
            {
                throw new ArgumentException(
                    "E-mail deve possuir no máximo 254 caracteres.");
            }

            if (telefone?.Length > 20)
            {
                throw new ArgumentException(
                    "Telefone deve possuir no máximo 20 caracteres.");
            }
        }



    }
}
