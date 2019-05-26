import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class TokenService {
  private key: string;
  constructor() {
    this.key = 'making-sense-blogging-token';
  }

  getToken() {
    const data = localStorage.getItem(this.key);
    if (data) {
      return JSON.parse(data).token;
    }
    return null;
  }

  getUsername() {
    const data = localStorage.getItem(this.key);
    if (data) {
      return JSON.parse(data).username;
    }
    return null;
  }

  saveToken(token: string) {
    localStorage.setItem(this.key, token);
  }
  clearToken() {
    localStorage.removeItem(this.key);
  }
}
