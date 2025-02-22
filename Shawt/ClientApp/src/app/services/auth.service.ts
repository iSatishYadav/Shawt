import { Injectable, Inject } from '@angular/core';
import { UserManager, UserManagerSettings, User } from 'oidc-client';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  userManagerSettings: UserManagerSettings;
  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string) {
    http.get<UserManagerSettings>(baseUrl + 'metadata').subscribe(x => {
      this.userManagerSettings = x;
    });
  }

  log(...message) {
    console.log(`[Auth Service]`, message);
  }
  getClientSettings(): Promise<UserManagerSettings> {
    return this.http.get<UserManagerSettings>(this.baseUrl + 'metadata').toPromise();
  }

  private manager = this.getClientSettings().then(x => {
    let manager = new UserManager(x);
    this.log("Getting User Manager", manager);
    return new Promise<UserManager>((res, rej) => res(manager));
  });
  public user: User = null;

  isLoggedIn(): boolean {
    let isLoggedIn = this.user != null && !this.user.expired;
    this.log("Is logged in?", isLoggedIn)
    return isLoggedIn;
  }

  getClaims(): any {
    return this.user.profile;
  }

  getAuthorizationHeaderValue(): string {
    return `${this.user.token_type} ${this.user.id_token}`;
  }

  startAuthentication(): Promise<void> {
    this.log("Starting Authentication");
    return this.manager.then(x => {
      this.log("redirecting...")
      return x.signinRedirect()
    });
  }

  completeAuthentication(): Promise<void> {
    return this.manager
      .then(x => x.signinRedirectCallback())
      .then(user => {
        this.log("User", user);
        this.user = user;
      });
  }
}
