using System;
using System.Collections.Generic;
using System.Text;

namespace HotelApp.Domain
{
    public class LancamentoConta
    {
        public int Id { get; private set; }
        public int ContaReservaId { get; private set; }
        public LancamentoTipo Tipo { get; private set; }
        public string Descricao { get; private set; }
        public decimal Valor { get; private set; }
        public FormaPagamento? FormaPagamento { get; private set; }
        public DateTime DataLancamento { get; set; }

        private LancamentoConta() 
        {
            Descricao = string.Empty;
        }
        public LancamentoConta(int contaReservaId, LancamentoTipo tipo, string descricao, decimal valor, FormaPagamento? formaPagamento = null)
        {
            if (contaReservaId <= 0)
            {
                throw new ArgumentException("Lançamento deve estar vinculado a uma conta valida");
            }

            if (string.IsNullOrWhiteSpace(descricao))
            {
                throw new ArgumentException("Lançamento deve conter uma descrição.");
            }

            if (valor <= 0)
            {
                throw new ArgumentException("Valor do lançamento deve ser maior que zero.");
            }

            if (tipo == LancamentoTipo.Credito && formaPagamento == null)
            {
                throw new ArgumentException("Crédito deve conter forma de pagamento.");
            }

            if (tipo == LancamentoTipo.Debito && formaPagamento != null)
            {
                throw new ArgumentException("Débito não deve conter forma de pagamento.");
            }

            ContaReservaId = contaReservaId;
            Tipo = tipo;
            Descricao = descricao.Trim();
            Valor = valor;
            FormaPagamento = formaPagamento;
            DataLancamento = DateTime.UtcNow;
        }
    }
}
