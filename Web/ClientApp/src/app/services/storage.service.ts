import {Injectable} from '@angular/core';

const KEY = "mood-user";

@Injectable({
  providedIn: 'root'
})
export class StorageService {
  constructor() {
  }

  public saveUser(user: any): void {
    window.localStorage.removeItem(KEY);
    window.localStorage.setItem(KEY, JSON.stringify(user));
  }

  public getUser(): any {
    const user = window.localStorage.getItem(KEY);
    if (user) {
      return JSON.parse(user);
    }
    return null;
  }

  public isLoggedIn(): boolean {
    const user = window.localStorage.getItem(KEY);
    return !!user;
  }

  public clearUser(): void {
    window.localStorage.clear();
  }
}
