import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { ReservaService, Reserva, ReservaPorNumero } from '../../services/reserva';
import { QuartoService } from '../../services/quarto';
import { ReservaDetalhes } from '../../components/reserva-detalhes-modal'
import { FormsModule } from '@angular/forms';
import { ChangeDetectorRef } from '@angular/core';
import { Auth } from '../../services/auth';
import { ReservaDetalhesBase } from '../../shared/reserva-detalhes-base';
import {
  calcularDuracaoNaTimeline,
  calcularOffsetNaTimeline,
  gerarDiasTimeline,
  reservaEstaNaTimeline
} from '../../shared/reserva-timeline';

@Component({
  selector: 'app-reservas',
  standalone: true,
  imports: [CommonModule, FormsModule, ReservaDetalhes],
  templateUrl: './reservas.html',
  styleUrl: './reservas.css'
})

export class ReservasComponent extends ReservaDetalhesBase implements OnInit
{
  quartos: any[] = [];
  dias: Date[] = []; 
  inicioTimeLine = new Date();
  

  constructor(reservaService: ReservaService, private quartoService: QuartoService,
  cdr: ChangeDetectorRef, router: Router, private route: ActivatedRoute,
  private authService: Auth){
    super(reservaService, router, cdr);
  }

  protected recarregarDados(): void {
    this.carregarQuartos();
  }

  ngOnInit(): void {
    this.inicioTimeLine.setHours(0, 0, 0, 0);
    this.gerarDias();
    this.carregarQuartos();

    this.route.queryParamMap.subscribe(params => {
      if (params.get('acao') === 'novo-quarto') {
        if (this.authService.ehGestor()) {
          this.mostrarForm = true;
        }

        this.router.navigate([], {
          relativeTo: this.route,
          queryParams: { acao: null },
          queryParamsHandling: 'merge',
          replaceUrl: true
        });
      }
    });
    }


  gerarDias() {
  this.dias = gerarDiasTimeline(this.inicioTimeLine, 10);
}

reservaEstaVisivel(reserva: any): boolean{
  return reservaEstaNaTimeline(reserva, this.dias);
}

avancarData(){
  const novaData = new Date(this.inicioTimeLine);
  novaData.setDate(this.inicioTimeLine.getDate() + 5);

  this.inicioTimeLine = novaData;
  
  this.gerarDias()
}

voltarData(){
  const novaData = new Date(this.inicioTimeLine);
  novaData.setDate(this.inicioTimeLine.getDate() - 5);

  const hoje = new Date()
  hoje.setHours(0,0,0,0);

  if(novaData < hoje){
    this.inicioTimeLine = hoje
  }
  else{
    this.inicioTimeLine = novaData;
  }
  this.gerarDias()
}

carregarQuartos(){
  console.log('CARREGANDO...');
  this.quartoService.listar().subscribe({
    next: (data) => {
      console.log('CHEGOU:', data);
      this.quartos = data;

      this.cdr.detectChanges(); // Fix nao carregar no refresh
    }
  });
}

getReservaNoDia(quarto: any, dia: Date) {
    return quarto.reservaList?.find((r: any) => {
      const checkIn = new Date(r.checkIn);
      const checkOut = new Date(r.checkOut);

      return dia >= checkIn && dia < checkOut;
 });
}

novaReserva: Reserva = {
  id: 0,
  checkIn: '',
  checkOut: '',
  nomeDoHospede: '',
  quartoId: 0,
  numeroQuarto: '',
  status: ''
};

salvando: boolean = false;

private chaveIdempotenciaReserva: string | null = null;
private chaveIdempotenciaReservaPorNumero: string | null = null;

criarReserva() {

  this.chaveIdempotenciaReserva ??= crypto.randomUUID();

  this.reservaService.criar(this.novaReserva, this.chaveIdempotenciaReserva).subscribe({
    next: () => {
      
      alert('Reserva criada com sucesso!');
      this.carregarQuartos();

      // reset do form
      this.novaReserva = {
        id: 0, 
        checkIn: '',
        checkOut: '',
        nomeDoHospede: '',
        quartoId: 0,
        numeroQuarto: '',
        status: ''
      };

      this.cdr.detectChanges();
      this.chaveIdempotenciaReserva = null;
    },
    error: (err) => {
      console.log(err);
      alert(err.error?.message || 'Erro ao criar reserva');
      this.salvando = false;
      this.cdr.detectChanges();
    }
  });
}

novaReservaPorNumero: ReservaPorNumero = {
  checkIn: '',
  checkOut: '',
  nomeDoHospede: '',
  numeroDoQuarto: ''
};




criarReservaPorNumero() {

  this.chaveIdempotenciaReservaPorNumero ??= crypto.randomUUID();

  this.limparMensagens()

  if(!this.novaReservaPorNumero.nomeDoHospede.trim()){
    this.mostrarErro("Nome é obrigatório")
    return
  }

  if(!this.novaReservaPorNumero.checkIn.trim()){
    this.mostrarErro("Dia do check-in é obrigatório")
    return
  }

  if(!this.novaReservaPorNumero.checkOut.trim()){
    this.mostrarErro("Dia do check-out é obrigatório")
    return
  }

  if(!this.novaReservaPorNumero.numeroDoQuarto.trim()){
    this.mostrarErro("Numero é obrigatório")
    return
  }

  if(this.salvando){
        return
      }

  this.salvando = true;

  this.reservaService.criarPorNumero(this.novaReservaPorNumero, this.chaveIdempotenciaReservaPorNumero).subscribe({
    next: () => {

      this.mostrarSucesso("Reserva criada com sucesso")

      this.carregarQuartos();

      // reset do form
      this.novaReservaPorNumero = {
        checkIn: '',
        checkOut: '',
        nomeDoHospede: '',
        numeroDoQuarto: ''
      };
      this.salvando = false;
      this.cdr.detectChanges();
      this.chaveIdempotenciaReservaPorNumero = null;
    },
    error: (err) => {
      console.log(err);
      this.mostrarErro(err.error?.message || "Erro ao criar reserva");
      this.salvando = false;
      this.cdr.detectChanges();
    }
  });
}


getDuracao(reserva: any): number {
  return calcularDuracaoNaTimeline(reserva, this.dias);
}

getOffset(reserva: any): number {
  return calcularOffsetNaTimeline(reserva, this.dias);
}

mostrarForm: boolean = false;
mostrarFormReserva: boolean = false;

alterarFormularioQuarto() {
  this.mostrarForm = !this.mostrarForm;

  if(!this.mostrarForm){
    this.novoQuarto = { numero: '', tipo: ''};
  }
}

alterarFormularioReserva() {
  this.mostrarFormReserva = !this.mostrarFormReserva;

  if (!this.mostrarFormReserva) {
    this.chaveIdempotenciaReservaPorNumero = null;

    this.novaReservaPorNumero = {
      checkIn: '',
      checkOut: '',
      nomeDoHospede: '',
      numeroDoQuarto: ''
    };
  }
}

novoQuarto = {
  numero: '',
  tipo: ''
};

criarQuarto() {

  if (!this.novoQuarto.numero.trim() || !this.novoQuarto.tipo.trim()) {
  alert('Preencha número e tipo do quarto.');
  return;
}

  this.quartoService.criar(this.novoQuarto).subscribe({
    next: () => {
      alert('Quarto criado!');
      this.carregarQuartos();
      this.mostrarForm = false;
      this.novoQuarto = { numero: '', tipo: '' };
      
    },
    error: (err) => {
      console.log(err);
      console.log(err.error);
      this.mostrarErro(err.erros?.message || 'Erro ao criar quarto');
    }
  });
}

};
