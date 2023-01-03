import {Component, EventEmitter, OnInit, Output} from '@angular/core';
import {BoardService} from "../services/board.service";
import {BoardItem} from "../models/board-item.model";
import {BoardHubService} from "../services/board-hub.service";

@Component({
  selector: 'app-board-item-list',
  templateUrl: './board-item-list.component.html',
  styleUrls: ['./board-item-list.component.css']
})
export class BoardItemListComponent implements OnInit {

  @Output()
  itemSelected: EventEmitter<BoardItem> = new EventEmitter<BoardItem>();

  errorMessages!: Array<string>;

  constructor(private boardService: BoardService, private boardHubService: BoardHubService) {}

  get items() {
    return this.boardService.board?.items;
  }

  ngOnInit(): void {
  }

  deleteItem(id: string) {
    this.boardService.deleteItem(id).subscribe({
      error: error => {
        this.errorMessages = Object.values<string>(error.error.errors);
        console.log(error);
      }
    })
  }

  showInfo(boardItem: BoardItem) {
    this.itemSelected.emit(boardItem);
  }
}
