import {Inject, Injectable} from '@angular/core';
import {HttpClient, HttpHeaders} from "@angular/common/http";
import {Observable} from "rxjs";
import {BoardListItem} from "../models/board-list-item.model";
import {map} from "rxjs/operators";

@Injectable({
  providedIn: 'root'
})
export class UserService {

  API_URL!: string;
  httpOptions = {
    headers: new HttpHeaders({'Content-Type': 'application/json'})
  };

  constructor(private httpClient: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.API_URL = baseUrl + "api/";
  }

  createBoard(name: string, description: string): Observable<any> {
    return this.httpClient.post(this.API_URL + "boards", {
      name,
      description
    }, this.httpOptions);
  }

  getBoards(): Observable<BoardListItem[]> {
    return this.httpClient.get<BoardListItem[]>(this.API_URL + "boards", this.httpOptions)
      .pipe(
        map(response => response.map(i => i = {
          name: i.name,
          boardId: i.boardId,
          lastModified: new Date(i.lastModified)
        })));
  }

  searchBoards(query: string): Observable<BoardListItem[]> {
    return this.httpClient.get<BoardListItem[]>(this.API_URL + "boards/search?query=" + query, this.httpOptions)
      .pipe(
        map(response => response.map(i => i = {
          name: i.name,
          boardId: i.boardId,
          lastModified: new Date(i.lastModified)
        })));
  }
}
