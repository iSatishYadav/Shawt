import { Component, OnInit } from '@angular/core';
import { AuthService } from '../services/auth.service';
import { Router } from '@angular/router';
import { LoaderService } from '../services/loader.service';

@Component({
  selector: 'app-auth-callback',
  templateUrl: './auth-callback.component.html',
  styleUrls: ['./auth-callback.component.css']
})
export class AuthCallbackComponent implements OnInit {

  constructor(private authService: AuthService, private router: Router, private loader: LoaderService) { }

  ngOnInit() {
    this.loader.show();
    this.authService
      .completeAuthentication()
      .then(x => {
        this.loader.hide();
        this.router.navigate(['/shorten'])
      });
  }
}
