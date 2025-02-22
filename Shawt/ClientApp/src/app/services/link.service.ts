import { Injectable, Inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { AuthService } from './auth.service';
import { Link } from '../models/link';
import { Observable } from 'rxjs';
import { LinkWithLog } from '../models/link-with-log';

@Injectable({
  providedIn: 'root'
})
export class LinkService {

  constructor(private http: HttpClient, private authService: AuthService, @Inject('BASE_URL') private baseUrl: string) {
  }

  getLinkWithLogs(id: string): Observable<LinkWithLog> {
    let headers = new HttpHeaders({ 'Authorization': this.authService.getAuthorizationHeaderValue() });

    return this.http.get<LinkWithLog>(`${this.baseUrl}api/links/${id}`, { headers: headers });
    }
  getLinks(): Observable<Link[]> {

    let headers = new HttpHeaders({ 'Authorization': this.authService.getAuthorizationHeaderValue() });

    return this.http.get<Link[]>(`${this.baseUrl}api/links`, { headers: headers });    
  }

  shortenLink(originalLink: string) {
    let headers = new HttpHeaders({ 'Authorization': this.authService.getAuthorizationHeaderValue(), 'Content-Type': 'application/JSON' });
    return this.http.post<Link>(`${this.baseUrl}api/links`, originalLink, { headers: headers });
  }

}
