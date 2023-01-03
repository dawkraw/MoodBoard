import {Component, OnInit} from '@angular/core';
import {FormControl, Validators} from "@angular/forms";
import {BoardService} from "../services/board.service";

@Component({
  selector: 'app-board-members',
  templateUrl: './board-members.component.html',
})
export class BoardMembersComponent implements OnInit {
  memberControl!: FormControl;
  errorMessages!: Array<string>;
  isSubmitting = false;

  constructor(private boardService: BoardService) {
  }

  get members() {
    return this.boardService.board?.boardMembers;
  }

  ngOnInit(): void {
    this.memberControl = new FormControl('', [
      Validators.required,
      Validators.maxLength(15)
    ])
  }

  submitForm() {
    this.boardService.addMember(this.memberControl.value).subscribe({
      next: data => {
        this.boardService.board.boardMembers?.push(data);
      },
      error: error => {
        this.errorMessages = Object.values<string>(error.error.errors);
        console.log(error);
      }
    })
  }

  removeMember(username: string) {
    this.boardService.removeMember(username).subscribe({
      next: data => {
        let indexToDelete = this.boardService.board.boardMembers?.findIndex((value) => value.userName == username);
        if (indexToDelete) {
          this.boardService.board.boardMembers?.splice(indexToDelete, 1);
        }
      },
      error: error => {
        this.errorMessages = Object.values<string>(error.error.errors);
        console.log(error);
      }
    })
  }
}
