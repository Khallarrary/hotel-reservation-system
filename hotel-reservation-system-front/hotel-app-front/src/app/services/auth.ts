import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';


export interface LoginRequest {
  email: string;
  senha: string;
}

export interface LoginResponse {
  token: string;
  email: string;
  nome: string;
  perfil: string;  
}


@Injectable({
  providedIn: 'root',
})
export class Auth 
{
  private apiUrl = `${environment.apiUrl}/api/Usuario/login`;

  constructor(private http: HttpClient) {}

  login(dados: LoginRequest){
    return this.http.post<LoginResponse>(this.apiUrl, dados);
  }

  salvarSessao(resposta: LoginResponse){
    localStorage.setItem('hotel_token', resposta.token)
    localStorage.setItem('hotel_nome', resposta.nome)
    localStorage.setItem('hotel_email', resposta.email)
    localStorage.setItem('hotel_perfil', resposta.perfil)
  }

  obterToken(): string | null {
    return localStorage.getItem('hotel_token');
  }

   obterNome(): string | null {
    return localStorage.getItem('hotel_nome');
  }

  obterPerfil(): string | null {
    return localStorage.getItem('hotel_perfil');
  }

  ehGestor(): boolean{
    return this.obterPerfil()?.toLowerCase() === 'gestor';
  }

  estaLogado(): boolean {
    return this.obterToken() !== null;
  }

  logout(): void {
    localStorage.removeItem('hotel_token')
    localStorage.removeItem('hotel_nome')
    localStorage.removeItem('hotel_email')
    localStorage.removeItem('hotel_perfil')
  }

}
