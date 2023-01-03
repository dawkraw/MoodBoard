import {EventEmitter, Inject, Injectable} from '@angular/core';
import {HubConnection, HubConnectionBuilder, HubConnectionState} from "@microsoft/signalr";
import {BoardItem} from "../models/board-item.model";

@Injectable({
  providedIn: 'root'
})
export class BoardHubService {

  public placementUpdated = new EventEmitter<BoardItem>();
  public itemDeleted = new EventEmitter<BoardItem>();
  public itemCreated = new EventEmitter<BoardItem>();
  private _connection!: HubConnection;
  private _joinedBoards!: string[];

  constructor(@Inject('BASE_URL') private baseUrl: string) {
    this.startConnection();
    this.registerListeners();
    this._joinedBoards = [];
  }

  public async joinBoard(boardId: string): Promise<BoardItem[]> {
    await this.waitForConnection();
    if (this._joinedBoards.includes(boardId)) {
      return [];
    }

    return await this._connection.invoke<BoardItem[]>("JoinBoard", boardId).then(
      data => {
        this._joinedBoards.push(boardId);

        return data;
      }
    ).catch(err => {
      console.log(err)
      return [];
    });
  }

  public async leaveBoard(boardId: string) {
    if (!this._joinedBoards.includes(boardId)) {
      return;
    }

    return await this._connection.invoke("LeaveBoard", boardId).then(
      data => {
        let itemIndex = this._joinedBoards.indexOf(boardId, 0);
        this._joinedBoards.splice(itemIndex, 1);
      }
    ).catch(err => console.log(err));
  }

  public updatePlacement(boardItem: BoardItem) {
    return this._connection.invoke("UpdatePlacement", {
      boardItemId: boardItem.boardItemId,
      positionX: boardItem.position!.x,
      positionY: boardItem.position!.y,
      width: boardItem.size!.x,
      height: boardItem.size!.y,
      rotation: boardItem.rotation
    });
  }

  private waitForConnection() {
    const poll = (resolve: Function) => {
      if (this._connection.state == HubConnectionState.Connected) resolve();
      else setTimeout((_: any) => poll(resolve), 100);
    }
    return new Promise(poll);
  }

  private startConnection() {
    this._connection = new HubConnectionBuilder()
      .withUrl(this.baseUrl + "ws/boardHub")
      .build();

    this._connection
      .start()
      .then(() => console.log("connected."))
      .catch(err => console.log(err));
  }

  private registerListeners() {
    this._connection.on("PlacementUpdated", data => {
      this.placementUpdated.emit(data);
    });

    this._connection.on("ItemCreated", data => {
      this.itemCreated.emit(data);
    })

    this._connection.on("ItemDeleted", data => {
      this.itemDeleted.emit(data);
    })
  }

}
