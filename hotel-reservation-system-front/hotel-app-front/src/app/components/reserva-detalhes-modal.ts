
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
    @Output() checkout = new EventEmitter<void>();
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

    aoCheckOut(): void{
        this.checkout.emit()
    }

    tentarAcaoPrincipal(): void {
        if (this.podeExecutarAcaoPrincipal()) {
            this.mensagemAcaoPrincipal = '';
            this.executarAcaoPrincipal();
            return;
        }

        this.mensagemAcaoPrincipal = this.obterMensagemAcaoPrincipalBloqueada();

        setTimeout(() => {
            this.mensagemAcaoPrincipal = '';
        }, 3000);
    }

    obterTextoAcaoPrincipal(): string {
        if (this.reserva?.status === 'CheckIn') {
            return 'Check-out';
        }

        if (this.reserva?.status === 'CheckOut') {
            return 'Finalizada';
        }

        if (this.reserva?.status === 'Cancelada') {
            return 'Cancelada';
        }

        return 'Check-in';
    }

    podeExecutarAcaoPrincipal(): boolean {
        if (this.reserva?.status === 'Pendente') {
            return this.podeRealizarCheckIn();
        }

        if (this.reserva?.status === 'CheckIn') {
            return this.podeRealizarCheckOut();
        }

        return false;
    }

    executarAcaoPrincipal(): void {
        if (this.reserva?.status === 'Pendente') {
            this.aoCheckIn();
            return;
        }

        if (this.reserva?.status === 'CheckIn') {
            this.aoCheckOut();
        }
    }

    obterMensagemAcaoPrincipalBloqueada(): string {
        if (this.reserva?.status === 'Pendente') {
            return this.obterMensagemCheckInBloqueado();
        }

        if (this.reserva?.status === 'CheckIn') {
            return this.obterMensagemCheckOutBloqueado();
        }

        return 'Esta reserva nao possui uma acao principal disponivel.';
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

    obterMensagemCheckOutBloqueado(): string {
   if (this.reserva == null) {
            return 'Selecione uma reserva para realizar check-out.';
        }

        if (this.reserva.status !== 'CheckIn') {
            return 'Check-out disponivel apenas para reservas em check-in.';
        }

        if (this.obterDataLocalHoje() < this.obterDataApi(this.reserva.checkOut)) {
            return 'Check-out disponivel apenas a partir da data final da reserva.';
        }

        return 'Nao foi possivel realizar o check-out desta reserva.';
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

    podeRealizarCheckOut(): boolean{

        if(this.reserva == null){
            return false
        }

        if(this.reserva.status !== "CheckIn"){
            return false
        }
        
        if(this.obterDataLocalHoje() < this.obterDataApi(this.reserva.checkOut)){
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
