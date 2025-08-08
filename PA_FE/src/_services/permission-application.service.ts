import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../environments/environment';
import { Observable } from 'rxjs';
import { PermissionApplication } from '../_models/PermissionApplication';

@Injectable({
  providedIn: 'root',
})
export class PermissionApplicationService {
  private apiUrl = `${environment.apiUrl}/permissionapplications`;

  constructor(private http: HttpClient) {}

  getApplications(): Observable<PermissionApplication[]> {
    return this.http.get<PermissionApplication[]>(this.apiUrl);
  }

  createApplication(app: PermissionApplication): Observable<PermissionApplication> {
    return this.http.post<PermissionApplication>(this.apiUrl, app);
  }

  closeApplication(id: number, note: string): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/${id}/close`, { note });
  }
}
