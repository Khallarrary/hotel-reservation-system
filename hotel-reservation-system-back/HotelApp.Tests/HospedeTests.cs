using FluentAssertions;
using HotelApp.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelApp.Tests
{
    public class HospedeTests
    {
        [Fact]
        public void Deve_Normalizar_Dados_Ao_Criar_Hospede()
        {
            var hospede = new Hospede(1, "Jose da Silva", TipoDocumento.CPF, "123.123.123-09", "jose.jose@email.com", "(11) 99999-9999");

            hospede.HotelId.Should().Be(1);
            hospede.Nome.Should().Be("Jose da Silva");
            hospede.TipoDocumento.Should().Be(TipoDocumento.CPF);
            hospede.Documento.Should().Be("12312312309");
            hospede.Email.Should().Be("jose.jose@email.com");
            hospede.Telefone.Should().Be("11999999999");

            
        }

        [Fact]
        public void Deve_Atualizar_E_Normalizar_Dados_Do_Hospede()
        {
            var hospede = new Hospede(1, "Jose da Silva", TipoDocumento.CPF, "123.123.123-09", "jose.jose@email.com", "(11) 99999-9999");

            hospede.AtualizarDados(
            "Maria Souza",
            TipoDocumento.CPF,
            "987.654.321-00",
            "MARIA@EMAIL.COM",
            "(11) 98888-7777");

            hospede.HotelId.Should().Be(1);
            hospede.Nome.Should().Be("Maria Souza");
            hospede.Documento.Should().Be("98765432100");
            hospede.Email.Should().Be("maria@email.com");
            hospede.Telefone.Should().Be("11988887777");
        }

        [Fact]
        public void Nao_Deve_Criar_Hospede_Com_Nome_Acima_Do_Limite()
        {
            var nome = new string('A', 151);

            Action action = () => new Hospede(
                1,
                nome,
                TipoDocumento.CPF,
                "123.123.123-09",
                null,
                null);

            action.Should()
                .Throw<ArgumentException>()
                .WithMessage("*150*");
        }

        [Fact]
        public void Nao_Deve_Criar_Hospede_Com_Documento_Acima_Do_Limite()
        {
            var documento = new string('A', 31);

            Action action = () => new Hospede(
                1,
                "Jose da Silva",
                TipoDocumento.Passaporte,
                documento,
                null,
                null);

            action.Should()
                .Throw<ArgumentException>()
                .WithMessage("*30*");
        }

        [Fact]
        public void Nao_Deve_Criar_Hospede_Com_Email_Acima_Do_Limite()
        {
            var email = new string('a', 255);

            Action action = () => new Hospede(
                1,
                "Jose da Silva",
                TipoDocumento.CPF,
                "123.123.123-09",
                email,
                null);

            action.Should()
                .Throw<ArgumentException>()
                .WithMessage("*254*");
        }

        [Fact]
        public void Nao_Deve_Criar_Hospede_Com_Telefone_Acima_Do_Limite()
        {
            var telefone = new string('1', 21);

            Action action = () => new Hospede(
                1,
                "Jose da Silva",
                TipoDocumento.CPF,
                "123.123.123-09",
                null,
                telefone);

            action.Should()
                .Throw<ArgumentException>()
                .WithMessage("*20*");
        }

        [Fact]
        public void Nao_Deve_Alterar_Dados_Quando_Atualizacao_Exceder_Limite()
        {
            var hospede = new Hospede(
                1,
                "Jose da Silva",
                TipoDocumento.CPF,
                "123.123.123-09",
                "jose@email.com",
                "11999999999");

            Action action = () => hospede.AtualizarDados(
                "Maria Souza",
                TipoDocumento.CPF,
                "987.654.321-00",
                new string('a', 255),
                "11988887777");

            action.Should().Throw<ArgumentException>();
            hospede.Nome.Should().Be("Jose da Silva");
            hospede.Documento.Should().Be("12312312309");
            hospede.Email.Should().Be("jose@email.com");
            hospede.Telefone.Should().Be("11999999999");
        }
    }
}

