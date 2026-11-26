import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface DadosCategoriaQuarto {
  nome: string;
  descricao: string | null;
  capacidadeFisica: number;
  limiteComercial: number;
}

export interface CategoriaQuarto extends DadosCategoriaQuarto {
  id: number;
  ativa: boolean;
}

@Injectable({
  providedIn: 'root',
})
export class CategoriaQuartoService {
  private readonly apiUrl = `${environment.apiUrl}/api/CategoriaQuarto`;

  constructor(private http: HttpClient) {}

  listar(): Observable<CategoriaQuarto[]> {
    return this.http.get<CategoriaQuarto[]>(this.apiUrl);
  }

  criar(dados: DadosCategoriaQuarto): Observable<void> {
    return this.http.post<void>(this.apiUrl, dados);
  }

  atualizar(id: number, dados: DadosCategoriaQuarto): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, dados);
  }
}
