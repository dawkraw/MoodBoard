import {Inject, Injectable} from '@angular/core';
import {HttpClient, HttpHeaders} from "@angular/common/http";
import {Observable} from "rxjs";
import {UserInfo} from "../models/user-info.model";

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  AUTHENTICATION_URL!: string;
  httpOptions = {
    headers: new HttpHeaders({'Content-Type': 'application/json'})
  };

  constructor(private httpClient: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.AUTHENTICATION_URL = baseUrl + "api/account/";
  }

  login(username: string, password: string): Observable<any> {
    return this.httpClient.post(this.AUTHENTICATION_URL + 'authenticate', {
      username,
      password
    }, this.httpOptions);
  }

  register(username: string, password: string): Observable<any> {
    return this.httpClient.post(this.AUTHENTICATION_URL, {
      username,
      password
    }, this.httpOptions);
  }

  logout() {
    return this.httpClient.post(this.AUTHENTICATION_URL + "logout", {}, this.httpOptions);
  }
}
