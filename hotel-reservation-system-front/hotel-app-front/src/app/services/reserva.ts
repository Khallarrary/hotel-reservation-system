import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

export interface Reserva {
  checkIn: string;
  checkOut: string;
  nomeDoHospede: string;
  quartoId: number;
}

@Injectable({
  providedIn: 'root'
})
export class ReservaService {
  private apiUrl = 'https://localhost:7265/api/Reserva';

  constructor(private http: HttpClient) {}

  listar(): Observable<Reserva[]> {
    return this.http.get<Reserva[]>(this.apiUrl);
  }

  criar(reserva: Reserva) {
  return this.http.post(this.apiUrl, reserva);
}
}