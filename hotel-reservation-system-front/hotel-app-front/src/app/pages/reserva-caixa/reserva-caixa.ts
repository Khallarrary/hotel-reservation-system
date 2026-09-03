import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { CommonModule } from '@angular/common';
import { ChangeDetectorRef } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Location } from '@angular/common';
import { CaixaResumo, CaixaService, LancarCredito, LancarDebito } from '../../services/caixa'
import { ReservaService } from '../../services/reserva';

@Component({
  selector: 'app-reserva-caixa',
  imports: [CommonModule, FormsModule],
  templateUrl: './reserva-caixa.html',
  styleUrl: './reserva-caixa.css',
})
export class ReservaCaixa {

  reservaId: number = 0;
  caixa: CaixaResumo | null = null

  novoMovimento = { tipo: 'Credito', descricao: '', valor: 0, formaPagamento: 0 }

  mensagemSucesso: string = "";
  mensagemErro: string = "";

  constructor(
    private route: ActivatedRoute,
    private caixaService: CaixaService,
    private reservaService: ReservaService,
    private cdr: ChangeDetectorRef,
    private location: Location
  ) {}

  ngOnInit(): void {
    this.reservaId = Number(this.route.snapshot.paramMap.get('id'));
    this.carregarResumo();
  }

  carregarResumo(): void {
    this.caixaService.obterResumo(this.reservaId).subscribe({
      next: (resumo) => {
        console.log('RESUMO CAIXA:', resumo);
        this.caixa = resumo;
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.log(err);
      }
    })
  }

  limparFormulario(): void {
    this.novoMovimento = { tipo: 'Credito', descricao: '', valor: 0, formaPagamento: 0 }
  }

  voltar(): void {
    this.location.back();
  }

  limparMensagens() {
    this.mensagemErro = '';
    this.mensagemSucesso = '';
  }

  mostrarSucesso(texto: string) {
    this.mensagemSucesso = texto;
    this.mensagemErro = '';

    setTimeout(() => {
      this.mensagemSucesso = '';
    }, 3000);
  }

  mostrarErro(texto: string) {
    this.mensagemErro = texto;
    this.mensagemSucesso = '';

    setTimeout(() => {
      this.mensagemErro = '';
    }, 3000);
  }

  salvarMovimento(): void {
    console.log(this.novoMovimento)

    const credito: LancarCredito = {
      valor: this.novoMovimento.valor,
      descricao: this.novoMovimento.descricao,
      formaPagamento: this.novoMovimento.formaPagamento
    };

    const debito: LancarDebito = {
      valor: this.novoMovimento.valor,
      descricao: this.novoMovimento.descricao,
    };

    if (this.novoMovimento.tipo === 'Credito') {
      this.caixaService.lancarCredito(this.reservaId, credito).subscribe({
        next: () => {
          this.carregarResumo();
          this.limparFormulario();
          this.mostrarSucesso('Credito lancado')
        },
        error: (err) => {
          this.mostrarErro(err.error?.message || 'Nao foi possivel lancar credito.')
        }
      })
    } else if (this.novoMovimento.tipo === 'Debito') {
      this.caixaService.lancarDebito(this.reservaId, debito).subscribe({
        next: () => {
          this.carregarResumo();
          this.limparFormulario();
          this.mostrarSucesso('Debito lancado')
        },
        error: (err) => {
          this.mostrarErro(err.error?.message || 'Nao foi possivel lancar debito.')
        }
      })
    }
  }

  encerrarConta(): void {
    if (this.caixa === null) {
      this.mostrarErro('Caixa nao encontrado.')
      return
    }

    if (this.caixa.saldo !== 0) {
      this.mostrarErro('Conta so pode ser encerrada com saldo zerado')
      return
    }

    this.caixaService.encerrarConta(this.reservaId).subscribe({
      next: () => {
        this.carregarResumo();
        this.mostrarSucesso('Caixa encerrado!')
      },
      error: (err) => {
        this.mostrarErro(err.error?.message || 'Nao foi possivel encerrar a conta.')
      }
    })
  }

  realizarCheckOut(): void {
    this.reservaService.realizarCheckOut(this.reservaId).subscribe({
      next: () => {
        this.mostrarSucesso('Check-out realizado com sucesso!')
      },
      error: (err) => {
        this.mostrarErro(err.error?.message || err.error || 'Nao foi possivel realizar o check-out.')
      }
    })
  }

}
