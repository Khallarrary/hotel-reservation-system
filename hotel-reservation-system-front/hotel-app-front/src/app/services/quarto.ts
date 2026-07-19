import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class QuartoService {

  private apiUrl = `${environment.apiUrl}/api/Quarto`;
  

  constructor(private http: HttpClient) {}

  listar(): Observable<any[]> {
    return this.http.get<any[]>(this.apiUrl);
  }

  criar(quarto: any) {
    
  return this.http.post(this.apiUrl, quarto);
}
}