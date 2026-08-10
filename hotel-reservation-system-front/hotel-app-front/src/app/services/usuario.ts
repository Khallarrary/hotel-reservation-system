import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { Component, OnInit } from '@angular/core';


export interface Usuario {
  id: number;
  nome: string;
  email: string;
  perfil: string;
  ativo: boolean;
}

export interface CriarUsuario {
  nome: string;
  email: string;
  senha: string;
  perfil: string;
  
}

@Injectable({
  providedIn: 'root',
})
export class UsuarioService {

private apiUrl = `${environment.apiUrl}/api/Usuario`;

constructor(private http: HttpClient) {}

listar(): Observable<Usuario[]> {
    return this.http.get<Usuario[]>(this.apiUrl);
}

criar(usuario: CriarUsuario): Observable<void> {
  return this.http.post<void>(this.apiUrl, usuario)
}


alterarAtivacao(usuarioId: number, ativo: boolean): Observable<void>{
  const corpo = { ativo };

  return this.http.patch<void>(`${this.apiUrl}/${usuarioId}/status`, corpo)
}
}