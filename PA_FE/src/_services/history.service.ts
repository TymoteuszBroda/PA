import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../environments/environment';
import { History } from '../_models/History';

@Injectable({
  providedIn: 'root',
})
export class HistoryService {
  private apiUrl = `${environment.apiUrl}/history`;

  constructor(private http: HttpClient) {}

  getHistory(employeeName?: string): Observable<History[]> {
    const options = employeeName ? { params: { employeeName } } : {};
    return this.http.get<History[]>(this.apiUrl, options);
  }
}
