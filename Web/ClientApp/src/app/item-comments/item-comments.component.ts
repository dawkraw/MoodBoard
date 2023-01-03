import {Component, Input, OnChanges, OnInit, SimpleChanges} from '@angular/core';
import {ItemComment} from "../models/comment.model";
import {FormControl, Validators} from "@angular/forms";
import {BoardService} from "../services/board.service";

@Component({
  selector: 'app-item-comments',
  templateUrl: './item-comments.component.html',
})
export class ItemCommentsComponent implements OnInit, OnChanges {

  @Input()
  public itemId!: string;

  @Input()
  public comments!: ItemComment[];

  commentControl!: FormControl;

  errorMessages!: Array<string>;
  isSubmitting = false;

  constructor(private boardService: BoardService) {
  }

  ngOnChanges(changes: SimpleChanges): void {
    this.getComments();
  }

  ngOnInit(): void {
    this.commentControl = new FormControl('', [
      Validators.required,
      Validators.maxLength(200)
    ])
  }

  getComments() {
    this.boardService.getComments(this.itemId).subscribe({
      next: data => {
        this.comments = data;
      },
      error: error => {
        this.errorMessages = Object.values<string>(error.error.errors);
        console.log(error);
      }
    })
  }

  submitForm() {
    this.boardService.addComment(this.itemId, this.commentControl.value).subscribe({
      next: data => {
        this.commentControl.setValue("");
        this.getComments()
      },
      error: error => {
        this.errorMessages = Object.values<string>(error.error.errors);
        console.log(error);
      }
    })
  }
}
