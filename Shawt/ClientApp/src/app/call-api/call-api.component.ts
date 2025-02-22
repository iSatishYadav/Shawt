import { Component, OnInit } from '@angular/core';
import { AuthService } from '../services/auth.service';
import { HttpClient, HttpHeaders } from '@angular/common/http';

@Component({
  selector: 'app-call-api',
  templateUrl: './call-api.component.html',
  styleUrls: ['./call-api.component.css']
})
export class CallApiComponent implements OnInit {

  response: Object;
  constructor(private http: HttpClient, private authService: AuthService) { }

  ngOnInit() {

    let headers = new HttpHeaders({ 'Authorization': this.authService.getAuthorizationHeaderValue() });

    this.http.get("https://localhost:44320/api/identity", { headers: headers })
      .subscribe(response => this.response = response);
  }
}
