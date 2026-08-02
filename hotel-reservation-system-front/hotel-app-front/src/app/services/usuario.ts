import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';


export interface Usuario {
  id: number;
  nome: string;
  email: string;
  perfil: string;
  ativo: boolean;
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

}