import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';


export interface Hospede {
  id: number;
  nome: string;
  tipoDocumento: string;
  documento: string;
  email: string | null;
  telefone: string | null;
}

export interface BuscarHospede {
  tipoDocumento: string;
  documento: string;
}

export interface CriarHospede {
  nome: string;
  tipoDocumento: string;
  documento: string;
  email?: string | null;
  telefone?: string | null;
}


@Injectable({
  providedIn: 'root',
})
export class HospedeService {

  private apiUrl = `${environment.apiUrl}/api/hospede`;

  constructor(private http: HttpClient) {}

  obterPorDocumento(filtro: BuscarHospede): Observable<Hospede>{
    return this.http.post<Hospede>(`${this.apiUrl}/buscar`, filtro);
  }

  criar(dados: CriarHospede): Observable<Hospede> {
    return this.http.post<Hospede>(
      this.apiUrl,
      dados
    );
  }

}
