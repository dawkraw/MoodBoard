import {Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges} from '@angular/core';
import {BoardListItem} from "../models/board-list-item.model";
import {UserService} from "../services/user.service";
import {ModalService} from "../services/modal.service";

@Component({
  selector: 'app-board-list',
  templateUrl: './board-list.component.html'
})
export class BoardListComponent implements OnInit, OnChanges {

  @Input() query?: string;
  @Output() queryChange = new EventEmitter<string>();
  boards?: BoardListItem[];
  title!: string;

  constructor(private boardService: UserService, private modalService: ModalService) {}

  ngOnInit(): void {
    this.loadBoards();
  }

  openModal(id: string) {
    this.modalService.open(id);
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['query'].currentValue) {
      this.title = "Search results for: " + this.query;
      this.boardService.searchBoards(changes['query'].currentValue).subscribe({
        next: data => {
          this.boards = data;
        }
      })
    }
  }

  loadBoards(): void {
    if (this.query == undefined) {
      this.boardService.getBoards().subscribe({
        next: data => {
          this.boards = data;
        }
      })
      this.title = "Your boards:"
    }
  }
}
