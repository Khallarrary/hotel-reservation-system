import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Reserva, ReservaService } from '../../services/reserva';
import { ChangeDetectorRef } from '@angular/core';

@Component({
  selector: 'app-reservas-lista',
  imports: [CommonModule],
  templateUrl: './reservas-lista.html',
  styleUrl: './reservas-lista.css',
  standalone: true
})



export class ReservasLista implements OnInit {
  reservas: Reserva[] = []
  constructor(private reservaService: ReservaService, private cdr: ChangeDetectorRef) {}


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
}
