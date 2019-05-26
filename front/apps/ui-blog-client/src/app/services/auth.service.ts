import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';

import { TokenService } from './token.service';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  constructor(private http: HttpClient, @Inject('apiurl') private apiurl: string, private tokenService: TokenService) { }

  login(username: string, password: string) {
    return this.http.post<any>(`${this.apiurl}login`, { username: username, password: password })
      .pipe(map(result => {
        // login successful if there's a jwt token in the response
        if (result && result.token) {
          // store user details and jwt token in local storage to keep user logged in between page refreshes
          this.tokenService.saveToken(JSON.stringify(result));
        }
        return result;
      }));
  }

  logout() {
    // remove user from local storage to log user out
    this.tokenService.clearToken();
  }
}
