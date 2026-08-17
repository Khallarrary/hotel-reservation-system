import { ChangeDetectorRef } from '@angular/core';
import { Router } from '@angular/router';
import { Reserva, ReservaService } from '../services/reserva';

export abstract class ReservaDetalhesBase {
  reservaSelecionada: Reserva | null = null;
  mensagemSucesso = '';
  mensagemErro = '';

  protected constructor(
    protected readonly reservaService: ReservaService,
    protected readonly router: Router,
    protected readonly cdr: ChangeDetectorRef
  ) {}

  protected abstract recarregarDados(): void;

  exibirDetalhesReserva(reserva: Reserva): void {
    this.reservaSelecionada = reserva;
  }

  fecharDetalhesReserva(): void {
    this.reservaSelecionada = null;
    this.cdr.detectChanges();
  }

  limparMensagens(): void {
    this.mensagemErro = '';
    this.mensagemSucesso = '';
  }

  mostrarSucesso(texto: string): void {
    this.mensagemSucesso = texto;
    this.mensagemErro = '';

    setTimeout(() => {
      this.mensagemSucesso = '';
    }, 3000);
  }

  mostrarErro(texto: string): void {
    this.mensagemErro = texto;
    this.mensagemSucesso = '';

    setTimeout(() => {
      this.mensagemErro = '';
    }, 3000);
  }

  deletarReservaSelecionada(): void {
    if (this.reservaSelecionada == null) {
      return;
    }

    this.reservaService.deletarReserva(this.reservaSelecionada.id).subscribe({
      next: () => {
        this.reservaSelecionada = null;
        this.cdr.detectChanges();
        this.recarregarDados();
        this.mostrarSucesso('Reserva cancelada com sucesso');
      },
      error: (err) => {
        console.log(err);
        console.log(err.error);
        this.mostrarErro(err.error?.message || err.error || 'Erro ao deletar reserva');
      }
    });
  }

  realizarCheckIn(): void {
    if (this.reservaSelecionada == null) {
      return;
    }

    this.reservaService.realizarCheckIn(this.reservaSelecionada.id).subscribe({
      next: () => {
        if (this.reservaSelecionada == null) {
          return;
        }

        this.reservaSelecionada.status = 'CheckIn';
        this.recarregarDados();
        this.mostrarSucesso('Reserva em check-in!');
      },
      error: (err) => {
        console.log(err);
        console.log(err.error);
        this.mostrarErro(err.error || 'Erro ao realizar check-in da reserva');
      }
    });
  }

  realizarCheckOut(): void {
    if (this.reservaSelecionada == null) {
      return;
    }

    this.reservaService.realizarCheckOut(this.reservaSelecionada.id).subscribe({
      next: () => {
        if (this.reservaSelecionada == null) {
          return;
        }

        this.reservaSelecionada.status = 'CheckOut';
        this.recarregarDados();
        this.mostrarSucesso('Reserva em check-out!');
      },
      error: (err) => {
        console.log(err);
        console.log(err.error);
        this.mostrarErro(err.error || 'Erro ao realizar check-out da reserva');
      }
    });
  }

  abrirCaixa(): void {
    if (this.reservaSelecionada == null) {
      return;
    }

    this.router.navigate(['/reservas', this.reservaSelecionada.id, 'caixa']);
  }
}
