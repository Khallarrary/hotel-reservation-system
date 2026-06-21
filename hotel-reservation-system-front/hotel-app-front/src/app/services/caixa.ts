import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';




export interface LancamentoConta {
  id: number;
  tipo: string;
  descricao: string;
  valor: number;
  formaPagamento?: string | null;
  dataLancamento: string;
}

export interface CaixaResumo {
  reservaId: number;
  contaReservaId: number;
  statusConta: string;
  totalDebitos: number;
  totalCreditos: number;
  saldo: number;
  lancamentos: LancamentoConta[];
}

export interface LancarCredito {
  valor: number;
  descricao: string;
  formaPagamento: number;
}


@Injectable({
  providedIn: 'root',
})

export class CaixaService
{
  private apiUrl = 'https://localhost:7265/api/reserva';

  constructor(private http: HttpClient) {}


  obterResumo(reservaId: number): Observable<CaixaResumo> {
    return this.http.get<CaixaResumo>(`${this.apiUrl}/${reservaId}/caixa`);
  }

  lancarCredito(reservaId: number, credito: LancarCredito) {
    return this.http.post(`${this.apiUrl}/${reservaId}/credito`, credito);
  }
}
