import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class QuartoService {

  private apiUrl = 'https://localhost:7265/api/Quarto';

  constructor(private http: HttpClient) {}

  listar(): Observable<any[]> {
    return this.http.get<any[]>(this.apiUrl);
  }

  criar(quarto: any) {
    
  return this.http.post(this.apiUrl, quarto);
}
}