import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface DadosQuarto {
  numero: string;
  categoriaQuartoId: number;
}

export interface QuartoGestao extends DadosQuarto {
  id: number;
  ativo: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class QuartoService {

  private apiUrl = `${environment.apiUrl}/api/Quarto`;
  

  constructor(private http: HttpClient) {}

  listar(): Observable<any[]> {
    return this.http.get<any[]>(this.apiUrl);
  }

  listarParaGestao(): Observable<QuartoGestao[]> {
    return this.http.get<QuartoGestao[]>(`${this.apiUrl}/gestao`);
  }

  criar(quarto: DadosQuarto): Observable<void> {
    return this.http.post<void>(this.apiUrl, quarto);
  }

  atualizar(id: number, quarto: DadosQuarto): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, quarto);
  }
}
