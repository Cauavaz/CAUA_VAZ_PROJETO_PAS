import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CriarTituloRequest, TituloListagem } from '../models/titulo.model';
import { environment } from '../../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class TituloService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/api/titulos`;

  listar(): Observable<TituloListagem[]> {
    return this.http.get<TituloListagem[]>(this.apiUrl);
  }

  criar(request: CriarTituloRequest): Observable<TituloListagem> {
    return this.http.post<TituloListagem>(this.apiUrl, request);
  }
}
