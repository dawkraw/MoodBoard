import {Component, OnInit} from '@angular/core';
import {BoardService} from "../services/board.service";
import {Router} from "@angular/router";

@Component({
  selector: 'app-board-delete-prompt',
  templateUrl: './board-delete-prompt.component.html',
})
export class BoardDeletePromptComponent implements OnInit {
  errorMessages!: Array<string>;
  isSubmitting = false;

  constructor(private boardService: BoardService, private router: Router) {
  }

  ngOnInit(): void {
  }

  deleteBoard() {
    this.isSubmitting = true;
    this.boardService.deleteBoard().subscribe(
      data => {
        this.router.navigate(['']);
        this.isSubmitting = false;
      },
      error => {
        this.errorMessages = Object.values<string>(error.error.errors);
        this.isSubmitting = false;
      }
    )
  }
}
