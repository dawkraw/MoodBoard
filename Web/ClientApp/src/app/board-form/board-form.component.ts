import {Component, Input, OnChanges, OnInit, SimpleChanges} from '@angular/core';
import {Board} from "../models/board.model";
import {FormControl, FormGroup, Validators} from "@angular/forms";
import {UserService} from "../services/user.service";
import {Router} from "@angular/router";
import {BoardService} from "../services/board.service";
import {ModalService} from "../services/modal.service";

@Component({
  selector: 'app-board-form',
  templateUrl: './board-form.component.html',
})
export class BoardFormComponent implements OnInit, OnChanges {
  boardForm!: FormGroup;
  @Input() board?: Board;

  errorMessages!: Array<string>;
  isSubmitting = false;

  constructor(private userService: UserService, private router: Router, private boardService: BoardService, private modalService: ModalService) {
  }

  get name() {
    return this.boardForm.get('name')!
  }

  get description() {
    return this.boardForm.get('description')!
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (this.boardForm != undefined) {
      this.boardForm.get('name')?.setValue(this.board?.name);
      this.boardForm.get('description')?.setValue(this.board?.description);
    }
  }

  ngOnInit(): void {
    this.boardForm = new FormGroup({
      name: new FormControl(this.board?.name, [
        Validators.required,
        Validators.maxLength(20)
      ]),
      description: new FormControl(this.board?.description, [
        Validators.required,
        Validators.maxLength(150)
      ])
    })
  }

  submitForm(): void {
    this.isSubmitting = true;
    if (this.board == undefined) {
      this.createNewBoard();
    } else {
      this.editExistingBoard();
    }
    this.isSubmitting = false;
  }

  createNewBoard(): void {
    this.userService.createBoard(this.name.value, this.description.value).subscribe({
      next: data => {
        this.router.navigate(['board', data.boardId]);
      },
      error: error => {
        this.errorMessages = Object.values<string>(error.error.errors);
        console.log(error);
      }
    })
  }

  editExistingBoard(): void {
    this.boardService.editBoard(this.name.value, this.description.value).subscribe({
      next: data => {
        this.boardService.board.name = this.name.value;
        this.boardService.board.description = this.description.value;
        this.modalService.close('edit-board');
      },
      error: error => {
        this.errorMessages = Object.values<string>(error.error.errors);
        console.log(error);
      }
    })
  }
}
