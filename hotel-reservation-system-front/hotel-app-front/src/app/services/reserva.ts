import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface Reserva {
  id: number;
  checkIn: string;
  checkOut: string;
  nomeDoHospede: string;
  quartoId: number;
  numeroQuarto: string;
  status: string;
  hospedes?: ReservaHospedeDetalhe[];
}

export interface ReservaHospedeDetalhe {
  id: number;
  nome: string;
  tipoDocumento: string;
  documento: string;
  email: string | null;
  telefone: string | null;
  titular: boolean;
}

export interface ReservaPaginada {
  itens: Reserva[];
  pagina: number;
  tamanhoPagina: number;
  totalItens: number;
  totalPaginas: number;
}

export interface ReservaConsulta {
  nomeHospede?: string;
  status?: string;
  numeroQuarto?: string;
  reservaId?: number | null;
}

export interface ConfirmacoesCheckOut{
  confirmarCheckOutAntecipado: boolean;
  confirmarSaldoAberto: boolean;
}

export interface ReservaHospedePayload {
  hospedeId: number;
  titular: boolean;
}

export interface ReservaPorNumero {
  checkIn: string;
  checkOut: string;
  numeroDoQuarto: string;
  hospedes: ReservaHospedePayload[];
}

@Injectable({
  providedIn: 'root'
})

export class ReservaService {
  private apiUrl = `${environment.apiUrl}/api/Reserva`;
  private apiUrlCriarPorNumero = `${environment.apiUrl}/api/Reserva/numero`;

  constructor(private http: HttpClient) {}

listar(): Observable<Reserva[]> {
    return this.http.get<Reserva[]>(this.apiUrl);
}

obterPorId(id: number): Observable<Reserva> {
  return this.http.get<Reserva>(`${this.apiUrl}/${id}`);
}

criar(reserva: Reserva, chaveIdempotencia: string) {
  return this.http.post(this.apiUrl, reserva, {
    headers: {
      'Idempotency-key': chaveIdempotencia
    }
  });
}

criarPorNumero(reservaPorNumero: ReservaPorNumero, chaveIdempotencia: string) {
  return this.http.post(this.apiUrlCriarPorNumero, reservaPorNumero, {
    headers: {
      'Idempotency-Key': chaveIdempotencia
    }
  });
}

deletarReserva(id: number) {
  return this.http.delete(`${this.apiUrl}/${id}`);
}

realizarCheckIn(id: number){
  return this.http.patch(`${this.apiUrl}/${id}/check-in`, null)
}

realizarCheckOut(id: number, confirmacoes: ConfirmacoesCheckOut ){
  return this.http.patch(`${this.apiUrl}/${id}/check-out`, confirmacoes)
}

listarPaginada(pagina: number, tamanhoPagina: number, filtros?: ReservaConsulta): Observable<ReservaPaginada>{
  
    const params: any = {
      pagina: pagina,
      tamanhoPagina: tamanhoPagina
    };

    if(filtros?.nomeHospede){
      params.nomeHospede = filtros.nomeHospede;
    }

    if(filtros?.status){
      params.status = filtros.status;
    }

    if(filtros?.numeroQuarto){
      params.numeroQuarto = filtros.numeroQuarto;
    }

    if(filtros?.reservaId){
      params.reservaId = filtros.reservaId;
    }

  return this.http.get<ReservaPaginada>(`${this.apiUrl}/paginadas`, { params });
}
  
}
