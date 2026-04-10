import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReservaService, Reserva } from '../../services/reserva';
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

  constructor(private reservaService: ReservaService, private quartoService: QuartoService,
  private cdr: ChangeDetectorRef){}

  ngOnInit(): void {
    this.gerarDias();
    this.carregarQuartos();
    }


  gerarDias() {
  const inicio = new Date(2026, 3, 10);
  const totalDias = 10;

  this.dias = []

  for (let i = 0; i < totalDias; i++){
    const d = new Date(inicio);
    d.setDate(inicio.getDate() + i);
    this.dias.push(d);
  }
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

getDuracao(reserva: any): number {
  const checkIn = new Date(reserva.checkIn);
  const checkOut = new Date(reserva.checkOut);

  const diff = checkOut.getTime() - checkIn.getTime();
  return diff / (1000 * 60 * 60 * 24);
}

getOffset(reserva: any): number {
  const inicioTimeline = this.dias[0];

  const checkIn = new Date(reserva.checkIn);

  const diff = checkIn.getTime() - inicioTimeline.getTime();
  return diff / (1000 * 60 * 60 * 24);
}

mostrarForm = false;

novoQuarto = {
  numero: '',
  tipo: ''
};

criarQuarto() {

  console.log('CLICOU', this.novoQuarto);

  this.quartoService.criar(this.novoQuarto).subscribe({
    next: () => {
      alert('Quarto criado!');
      this.carregarQuartos();
      this.mostrarForm = false;

      this.novoQuarto = { numero: '', tipo: '' };
      alert(this.novoQuarto);
    },
    error: (err) => {
      console.log(err);
      console.log(err.error);
      alert('Erro ao criar quarto');
    }
  });
}

};


