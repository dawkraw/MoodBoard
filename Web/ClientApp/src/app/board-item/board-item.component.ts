import {Component, Input, OnChanges, OnInit, SimpleChanges} from '@angular/core';
import {BoardItem} from "../models/board-item.model";
import {BoardService} from "../services/board.service";
import {BoardHubService} from "../services/board-hub.service";
import {ModalService} from "../services/modal.service";

@Component({
  selector: 'app-board-item',
  templateUrl: './board-item.component.html',
})
export class BoardItemComponent implements OnInit, OnChanges {

  @Input()
  item!: BoardItem;


  constructor(private boardService: BoardService, private boardHubService: BoardHubService, private modalService: ModalService) {
  }

  ngOnInit(): void {
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (this.item != undefined && this.item.boardItemId != undefined) {
      this.boardService.getComments(this.item.boardItemId).subscribe(
        data => {
          this.item.comments = data;
        },
        error => {
          console.log(error)
        }
      )
    }
  }

  deleteItem(id: string) {
    this.boardService.deleteItem(id).subscribe({
      next: data => {
        this.modalService.close('item-info');
      },
      error: error => {
        console.log(error);
      }
    })
  }
}
