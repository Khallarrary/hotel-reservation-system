import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

export interface Reserva {
  id: number;
  checkIn: string;
  checkOut: string;
  nomeDoHospede: string;
  quartoId: number;
  numeroQuarto: string;
  status: string;
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

export interface ReservaPorNumero{
  checkIn: string;
  checkOut: string;
  nomeDoHospede: string;
  numeroDoQuarto: string;
}

@Injectable({
  providedIn: 'root'
})

export class ReservaService {
  private apiUrl = 'https://localhost:7265/api/Reserva';
  private apiUrlCriarPorNumero = 'https://localhost:7265/api/Reserva/numero';

  constructor(private http: HttpClient) {}

listar(): Observable<Reserva[]> {
    return this.http.get<Reserva[]>(this.apiUrl);
}

criar(reserva: Reserva) {
  return this.http.post(this.apiUrl, reserva);
}

criarPorNumero(reservaPorNumero: ReservaPorNumero) {
  return this.http.post(this.apiUrlCriarPorNumero, reservaPorNumero);
}

deletarReserva(id: number) {
  return this.http.delete(`${this.apiUrl}/${id}`);
}

realizarCheckIn(id: number){
  return this.http.patch(`${this.apiUrl}/${id}/check-in`, null)
}

realizarCheckOut(id: number){
  return this.http.patch(`${this.apiUrl}/${id}/check-out`, null)
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