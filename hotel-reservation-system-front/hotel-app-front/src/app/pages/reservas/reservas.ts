import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReservaService, Reserva, ReservaPorNumero } from '../../services/reserva';
import { QuartoService } from '../../services/quarto';
import { FormsModule } from '@angular/forms';
import { ChangeDetectorRef } from '@angular/core';

@Component({
  selector: 'app-reservas',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './reservas.html',
  styleUrl: './reservas.css'
})

export class ReservasComponent implements OnInit
{
  quartos: any[] = [];
  dias: Date[] = []; 
  inicioTimeLine = new Date();
  

  constructor(private reservaService: ReservaService, private quartoService: QuartoService,
  private cdr: ChangeDetectorRef){}

  ngOnInit(): void {
    this.inicioTimeLine.setHours(0, 0, 0, 0);
    this.gerarDias();
    this.carregarQuartos();
    }


  gerarDias() {
  const totalDias = 10;

  this.dias = [];

  for (let i = 0; i < totalDias; i++) {
    const d = new Date(this.inicioTimeLine);
    d.setDate(this.inicioTimeLine.getDate() + i);
    this.dias.push(d);
  }
}

reservaEstaVisivel(reserva: any): boolean{
  const inicioTimeLine = this.dias[0];
  const fimTimeLine = new Date(inicioTimeLine);
  fimTimeLine.setDate(inicioTimeLine.getDate() + this.dias.length)
  const checkIn = new Date(reserva.checkIn)
  const checkOut = new Date(reserva.checkOut)


  if(inicioTimeLine >= checkOut){
    return false
  } else if(fimTimeLine <= checkIn){
    return false
  } else {
    return true
  }
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
  quartoId: 0
};

criarReserva() {
  this.reservaService.criar(this.novaReserva).subscribe({
    next: () => {
      alert('Reserva criada com sucesso!');
      this.carregarQuartos();

      // reset do form
      this.novaReserva = {
        id: 0, 
        checkIn: '',
        checkOut: '',
        nomeDoHospede: '',
        quartoId: 0
      };
    },
    error: (err) => {
      console.log(err);
      alert(err.error?.message || 'Erro ao criar reserva');
    }
  });
}

novaReservaPorNumero: ReservaPorNumero = {
  checkIn: '',
  checkOut: '',
  nomeDoHospede: '',
  numeroDoQuarto: ''
};

mensagemSucesso: string = "";
mensagemErro: string = "";

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

criarReservaPorNumero() {


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

  this.reservaService.criarPorNumero(this.novaReservaPorNumero).subscribe({
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
    },
    error: (err) => {
      console.log(err);
      this.mostrarErro(err.error?.message || "Erro ao criar reserva");
    }
  });
}


getDuracao(reserva: any): number {
  const checkIn = new Date(reserva.checkIn);
  const checkOut = new Date(reserva.checkOut);
  const inicioTimeLine = this.dias[0];
  const fimTimeLine = new Date(inicioTimeLine);
  fimTimeLine.setDate(inicioTimeLine.getDate() + this.dias.length)

  let inicioVisual = inicioTimeLine;

  if(checkIn > inicioTimeLine){
     inicioVisual = checkIn;
  } 

let fimVisual =fimTimeLine;

  if(checkOut < fimTimeLine){
     fimVisual = checkOut;
  } 

  

   
  const diff = fimVisual.getTime() - inicioVisual.getTime();
  let duracao = diff / (1000 * 60 * 60 * 24);

  if(checkIn < inicioTimeLine){
    duracao = duracao + 0.5;
  }

  return duracao;
}

getOffset(reserva: any): number {
  const inicioTimeLine = this.dias[0];
  
  const checkIn = new Date(reserva.checkIn);
  let inicioVisual = inicioTimeLine;

  if(checkIn > inicioTimeLine){
     inicioVisual = checkIn;
  } 

  if(checkIn < inicioTimeLine){
    return -0.5;
  }

  const diff = inicioVisual.getTime() - inicioTimeLine.getTime();
  return diff / (1000 * 60 * 60 * 24);
}

mostrarForm = false;
mostrarFormReserva = false;

alterarFormularioQuarto() {
  this.mostrarForm = !this.mostrarForm;

  if(!this.mostrarForm){
    this.novoQuarto = { numero: '', tipo: ''};
  }
}

alterarFormularioReserva() {
  this.mostrarFormReserva = !this.mostrarFormReserva;

  if (!this.mostrarFormReserva) {
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
      alert('Erro ao criar quarto');
    }
  });
}

reservaSelecionada: Reserva | null = null;

exibirDetalhesReserva(reserva: Reserva){
  this.reservaSelecionada = reserva;
}

};

