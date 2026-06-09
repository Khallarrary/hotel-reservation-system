import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

export interface Reserva {
  id: number;
  checkIn: string;
  checkOut: string;
  nomeDoHospede: string;
  quartoId: number;
  status: string;
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
  
}