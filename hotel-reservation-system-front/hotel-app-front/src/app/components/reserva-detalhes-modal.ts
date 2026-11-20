
import { Component, EventEmitter, Input, Output} from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Reserva, ReservaHospedeDetalhe } from '../services/reserva';
import { AtualizarHospede, HospedeService} from '../services/hospede';
import { finalize, switchMap } from 'rxjs';


@Component({
  selector: 'reserva-detalhes-modal',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './reserva-detalhes-modal.html',
  styleUrl: './reserva-detalhes-modal.css'
})

export class ReservaDetalhes {

    constructor(private hospedeService: HospedeService) {}

    hospedeEmEdicaoId: number | null = null;
    dadosHospedeEmEdicao: AtualizarHospede | null = null;
    salvandoHospede = false;
    erroEdicaoHospede = '';

    @Input() reserva: Reserva | null = null
    @Output() fechar = new EventEmitter<void>();
    @Output() cancelar = new EventEmitter<void>();
    @Output() checkin = new EventEmitter<void>();
    @Output() caixa = new EventEmitter<void>();
    mensagemAcaoPrincipal = '';
    hospedeSelecionadoId: number | null = null;

    alternarHospede(hospedeId: number): void {
        this.hospedeSelecionadoId = this.hospedeSelecionadoId === hospedeId
            ? null
            : hospedeId;
    }

    hospedeEstaAberto(hospedeId: number): boolean {
        return this.hospedeSelecionadoId === hospedeId;
    }

    formatarDocumento(hospede: ReservaHospedeDetalhe): string {
        if (hospede.tipoDocumento === 'CPF' && hospede.documento.length === 11) {
            return hospede.documento.replace(
                /^(\d{3})(\d{3})(\d{3})(\d{2})$/,
                '$1.$2.$3-$4');
        }

        return hospede.documento;
    }

    formatarTelefone(telefone: string | null): string {
        if (!telefone) {
            return 'Não informado';
        }

        if (telefone.length === 11) {
            return telefone.replace(/^(\d{2})(\d{5})(\d{4})$/, '($1) $2-$3');
        }

        return telefone;
    }

    aoFechar(): void{
        this.hospedeSelecionadoId = null;
        this.cancelarEdicao();
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

        if (this.reserva.status !== 'Confirmada') {
            return 'Check-in disponivel apenas para reservas confirmadas.';
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

        if(this.reserva.status !== "Confirmada"){
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

    iniciarEdicao(hospede: ReservaHospedeDetalhe): void {
        this.hospedeEmEdicaoId = hospede.id;

        this.dadosHospedeEmEdicao = {
            nome: hospede.nome,
            tipoDocumento: hospede.tipoDocumento,
            documento: hospede.documento,
            email: hospede.email,
            telefone: hospede.telefone
        };

        this.erroEdicaoHospede = '';
    }

    cancelarEdicao(): void {
    this.hospedeEmEdicaoId = null;
    this.dadosHospedeEmEdicao = null;
    this.erroEdicaoHospede = '';
    }

    salvarEdicao(hospede: ReservaHospedeDetalhe): void {
        const dados = this.dadosHospedeEmEdicao;

        if(!dados || this.hospedeEmEdicaoId !== hospede.id){
            return;
        }

        if(!dados.nome.trim()) {
            this.erroEdicaoHospede = "Informe o nome do hóspede.";
            return;
        }

        if (!dados.documento.trim()) {
            this.erroEdicaoHospede = 'Informe o documento do hóspede.';
            return;
        }

        if (this.salvandoHospede) {
            return;
        }

        this.salvandoHospede = true;
        this.erroEdicaoHospede = '';

        this.hospedeService.atualizar(hospede.id, dados)
        .pipe(
            switchMap(() =>
                this.hospedeService.obterPorDocumento({
                    tipoDocumento: dados.tipoDocumento,
                    documento: dados.documento
                })),
            finalize(() => {
                this.salvandoHospede = false;
            })
        ).subscribe({
            next: hospedeAtualizado => {
                Object.assign(hospede, hospedeAtualizado);
                this.cancelarEdicao();
            },
            error: err => {
                this.erroEdicaoHospede = err.error?.message || 'Não foi possivel atualizar o hospede';
            }
        })
    }
}
