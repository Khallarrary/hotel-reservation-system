
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
    @Output() checkin = new EventEmitter<void>();
    @Output() caixa = new EventEmitter<void>();
    mensagemAcaoPrincipal = '';
    
    

    aoFechar(): void{
        this.fechar.emit();

    }

    aoCaixa(): void{
        this.caixa.emit();
    }
    
    aoCancelar(): void{
        this.cancelar.emit();
    }

    aoCheckIn(): void{
        this.checkin.emit()
    }

    tentarCheckIn(): void {
        if (this.podeRealizarCheckIn()) {
            this.mensagemAcaoPrincipal = '';
            this.aoCheckIn();
            return;
        }

        this.mensagemAcaoPrincipal = this.obterMensagemCheckInBloqueado();

        setTimeout(() => {
            this.mensagemAcaoPrincipal = '';
        }, 3000);
    }

    private obterDataApi(data: string): string {
        return data.substring(0, 10);
    }

    formatarDataApi(data: string): string {
        const [ano, mes, dia] = this.obterDataApi(data).split('-');

        return `${dia}/${mes}/${ano}`;
    }

    obterMensagemCheckInBloqueado(): string {
   if (this.reserva == null) {
            return 'Selecione uma reserva para realizar check-in.';
        }

        if (this.reserva.status !== 'Pendente') {
            return 'Check-in disponivel apenas para reservas pendentes.';
        }

        if (this.obterDataLocalHoje() < this.obterDataApi(this.reserva.checkIn)) {
            return 'Check-in disponivel apenas a partir da data da reserva.';
        }

        return 'Nao foi possivel realizar o check-in desta reserva.';
    }

    podeRealizarCheckIn(): boolean{

        if(this.reserva == null){
            return false
        }

        if(this.reserva.status !== "Pendente"){
            return false
        }
        
        if(this.obterDataLocalHoje() < this.obterDataApi(this.reserva.checkIn)){
            return false
        }

        return true
    }

    private obterDataLocalHoje(): string {
        const hoje = new Date();
        const ano = hoje.getFullYear();
        const mes = String(hoje.getMonth() + 1).padStart(2, '0');
        const dia = String(hoje.getDate()).padStart(2, '0');

        return `${ano}-${mes}-${dia}`;
    }
}
