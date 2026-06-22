import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Reserva, ReservaService } from '../../services/reserva';
import { ChangeDetectorRef } from '@angular/core';
import { ReservaDetalhes } from '../../components/reserva-detalhes-modal'
import { Router } from '@angular/router';

@Component({
  selector: 'app-reservas-lista',
  imports: [CommonModule, ReservaDetalhes],
  templateUrl: './reservas-lista.html',
  styleUrl: './reservas-lista.css',
  standalone: true
})



export class ReservasLista implements OnInit {
  reservas: Reserva[] = []
  constructor(private reservaService: ReservaService, private cdr: ChangeDetectorRef, private router: Router) {}

  reservaSelecionada: Reserva | null = null;

  ngOnInit(): void {
     console.log('ENTROU NA LISTAGEM');
    this.carregarReservas()

  }


  carregarReservas(): void{
  console.log('CARREGANDO...');
  this.reservaService.listar().subscribe({
    next: (data) => {
      console.log('RESERVAS:', data);
      this.reservas = data;
      this.cdr.detectChanges();
      }, 
      error: (err) => {
        console.log(err)
      }
  });
  }

  private obterDataApi(data: string): string {
    return data.substring(0, 10);
  }

  formatarDataApi(data: string): string {
    const [ano, mes, dia] = this.obterDataApi(data).split('-');

    return `${dia}/${mes}/${ano}`;
  }

  exibirDetalhesReserva(reserva: Reserva){
    this.reservaSelecionada = reserva;
    console.log(reserva);
  }

  fecharDetalhesReserva() {
    this.reservaSelecionada = null;
    this.cdr.detectChanges();
  }

  deletarReservaSelecionada(){
  if(this.reservaSelecionada != null){
    this.reservaService.deletarReserva(this.reservaSelecionada.id).subscribe({
      next: () => {
        this.reservaSelecionada = null;
        this.cdr.detectChanges();
        this.carregarReservas();
        this.mostrarSucesso("Reserva cancelada com sucesso");
      }, 
      error: (err) => {
        console.log(err);
        console.log(err.error);
        this.mostrarErro(err.error || 'Erro ao deletar reserva');
      }
    })
  }
}

mensagemSucesso: string = "";
mensagemErro: string = "";

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

realizarCheckIn(){
  if(this.reservaSelecionada != null){
    this.reservaService.realizarCheckIn(this.reservaSelecionada.id).subscribe({
      next: () => {
        const reserva = this.reservaSelecionada;
        if(reserva == null){
          return
        }
        reserva.status = "CheckIn";
        this.carregarReservas();
        this.mostrarSucesso("Reserva em check-in!")
    },
      error: (err) => {
        console.log(err);
        console.log(err.error);
        this.mostrarErro(err.error || 'Erro ao realizar check-in da reserva');
      }
    })
  }
}

realizarCheckOut(){
  if(this.reservaSelecionada != null){
    this.reservaService.realizarCheckOut(this.reservaSelecionada.id).subscribe({
      next: () => {
        const reserva = this.reservaSelecionada;
        if(reserva == null){
          return
        }
        reserva.status = "CheckOut";
        this.carregarReservas();
        this.mostrarSucesso("Reserva em check-out!")
    },
      error: (err) => {
        console.log(err);
        console.log(err.error);
        this.mostrarErro(err.error || 'Erro ao realizar check-out da reserva');
      }
    })
  }
}

abrirCaixa(): void {
  if(this.reservaSelecionada != null){
    const reserva = this.reservaSelecionada;
    this.router.navigate(['/reservas', reserva.id, 'caixa'])
  }
}

}
