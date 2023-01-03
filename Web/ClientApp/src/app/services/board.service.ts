import {Inject, Injectable} from '@angular/core';
import {Board} from "../models/board.model";
import {Observable} from "rxjs";
import {HttpClient, HttpHeaders} from "@angular/common/http";
import {StorageService} from "./storage.service";
import {BoardHubService} from "./board-hub.service";
import {BoardItem} from "../models/board-item.model";
import {BoardUser} from "../models/board-user.model";
import {ItemComment} from "../models/comment.model";
import {Router} from "@angular/router";

@Injectable({
  providedIn: 'root'
})
export class BoardService {

  boardId!: string;
  board!: Board;

  API_URL!: string;
  httpOptions = {
    headers: new HttpHeaders({'Content-Type': 'application/json'})
  };

  constructor(private httpClient: HttpClient, private storageService: StorageService, private boardHubService: BoardHubService, private router: Router, @Inject('BASE_URL') baseUrl: string) {
    this.API_URL = baseUrl + "api/";
  }

  public async initBoard(boardId: string) {
    this.boardId = boardId;
    return new Promise<void>((resolve) => {
      this.getBoard().subscribe(data => {
        this.board = data;
        this.board.isOwner = data.createdBy.identityId == this.storageService.getUser()?.identityId;
        this.boardHubService.joinBoard(this.boardId).then(data => {
          this.board.items = data
          resolve();
        });
      }, error => {
        this.router.navigate(['home']);
      })
    })
  }

  public updateItemTransform(item: BoardItem, posX: number, posY: number, rotation: number, width: number, height: number) {
    item.rotation = rotation;
    item.position.x = posX;
    item.position.y = posY;
    item.size.x = width;
    item.size.y = height;
    this.boardHubService.updatePlacement(item)
      .catch(err => console.log(err));
  }

  public deleteBoard(): Observable<any> {
    return this.httpClient.delete(this.API_URL + "boards/" + this.boardId, this.httpOptions);
  }

  public leaveBoard() {
    this.boardHubService.leaveBoard(this.boardId);
  }

  public editBoard(name: string, description: string): Observable<any> {
    return this.httpClient.put(this.API_URL + "boards/", {
      boardId: this.board.boardId,
      name: name,
      description: description
    }, this.httpOptions);
  }

  public createItem(title: string, note: string, imageUrl: string): Observable<any> {
    return this.httpClient.post(this.API_URL + "boardItems", {
      boardId: this.board.boardId,
      title: title,
      note: note,
      imageUrl: imageUrl
    }, this.httpOptions);
  }

  public deleteItem(id: string): Observable<any> {
    return this.httpClient.delete(this.API_URL + "boardItems", {
      body: {
        boardItemId: id
      }
    });
  }

  public getComments(id: string): Observable<ItemComment[]> {
    return this.httpClient.get<ItemComment[]>(this.API_URL + "comments/" + id, this.httpOptions);
  }

  public addComment(itemId: string, content: string): Observable<any> {
    return this.httpClient.post(this.API_URL + "comments", {
      commentContent: content,
      itemId: itemId
    }, this.httpOptions)
  }

  public addMember(userName: string) {
    return this.httpClient.post<BoardUser>(this.API_URL + "boards/member", {
      boardId: this.board.boardId,
      userName: userName,
    }, this.httpOptions);
  }

  public removeMember(userName: string) {
    return this.httpClient.delete(this.API_URL + "boards/member", {
      body: {
        boardId: this.board.boardId,
        userName: userName,
      }
    });
  }

  private getBoard(): Observable<Board> {
    return this.httpClient.get<Board>(this.API_URL + "boards/" + this.boardId, this.httpOptions);
  }
}
