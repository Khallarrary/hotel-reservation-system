import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Reserva, ReservaService } from '../../services/reserva';
import { ChangeDetectorRef } from '@angular/core';
import { ReservaDetalhes } from '../../components/reserva-detalhes-modal'
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { ReservaDetalhesBase } from '../../shared/reserva-detalhes-base';

@Component({
  selector: 'app-reservas-lista',
  imports: [CommonModule, FormsModule, ReservaDetalhes],
  templateUrl: './reservas-lista.html',
  styleUrl: './reservas-lista.css',
  standalone: true
})



export class ReservasLista extends ReservaDetalhesBase implements OnInit {
  reservas: Reserva[] = []
  paginaAtual: number = 1;
  tamanhoPagina: number = 10;
  totalItens: number = 0;
  totalPaginas: number = 0;
  constructor(reservaService: ReservaService, cdr: ChangeDetectorRef, router: Router) {
    super(reservaService, router, cdr);
  }

  protected recarregarDados(): void {
    this.carregarReservas();
  }

  filtros = {
  nomeHospede: '',
  status: '',
  numeroQuarto: '',
  reservaId: null
  }

  ngOnInit(): void {
     console.log('ENTROU NA LISTAGEM');
    this.carregarReservas()

  }

  carregarReservas(): void{
  console.log('CARREGANDO...');
  this.reservaService.listarPaginada(this.paginaAtual, this.tamanhoPagina, this.filtros).subscribe({
    next: (resposta) => {
      console.log(resposta)
      this.reservas = resposta.itens;
      this.paginaAtual = resposta.pagina;
      this.tamanhoPagina = resposta.tamanhoPagina;
      this.totalItens = resposta.totalItens;
      this.totalPaginas = resposta.totalPaginas;
      this.cdr.detectChanges();
      }, 
      error: (err) => {
        console.log(err)
      }
  });
  }

  buscar(): void{
    this.paginaAtual = 1;
    this.carregarReservas();
  }

  limpar(): void{
    this.filtros = {
      nomeHospede: '',
      status: '',
      numeroQuarto: '',
      reservaId: null
      }
    this.paginaAtual = 1;
    this.carregarReservas();
  }

  proximaPagina(): void{
    if(this.paginaAtual >= this.totalPaginas){
      return;
    }
      this.paginaAtual += 1;
      this.carregarReservas();
  }

  anteriorPagina(): void{
    if(this.paginaAtual <= 1){
      return;
    }
      this.paginaAtual -= 1;
      this.carregarReservas();
  }

  private obterDataApi(data: string): string {
    return data.substring(0, 10);
  }

  formatarDataApi(data: string): string {
    const [ano, mes, dia] = this.obterDataApi(data).split('-');

    return `${dia}/${mes}/${ano}`;
  }

}
