
import { Component, EventEmitter, Input, Output} from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Reserva } from '../services/reserva';

@Component({
  selector: 'reserva-detalhes-modal',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './reserva-detalhes-modal.html',
  styleUrl: './reserva-detalhes-modal.css'
})

export class ReservaDetalhes {

    @Input() reserva: Reserva | null = null
    @Output() fechar = new EventEmitter<void>();
    @Output() cancelar = new EventEmitter<void>();
    

    aoFechar(): void{
        this.fechar.emit();

    }

    aoCancelar(): void{
        this.cancelar.emit();
    }
}