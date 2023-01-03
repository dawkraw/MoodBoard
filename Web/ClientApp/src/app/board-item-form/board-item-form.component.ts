import {Component, OnInit} from '@angular/core';
import {AbstractControl, FormControl, FormGroup, ValidationErrors, ValidatorFn, Validators} from "@angular/forms";
import {BoardService} from "../services/board.service";
import {ModalService} from "../services/modal.service";

@Component({
  selector: 'app-board-item-form',
  templateUrl: './board-item-form.component.html',
})
export class BoardItemFormComponent implements OnInit {
  boardItemForm!: FormGroup;

  errorMessages!: Array<string>;
  isSubmitting = false;

  constructor(private boardService: BoardService, private modalService: ModalService) {
  }

  get title() {
    return this.boardItemForm.get('title')!
  }

  get note() {
    return this.boardItemForm.get('note')!
  }

  get imageUrl() {
    return this.boardItemForm.get('imageUrl')!
  }

  ngOnInit(): void {
    this.boardItemForm = new FormGroup({
      title: new FormControl('', [
        Validators.required,
        Validators.maxLength(20)
      ]),
      note: new FormControl('', [
        Validators.required,
        Validators.maxLength(500)
      ]),
      imageUrl: new FormControl('', [
        Validators.required,
        this.isUrl
      ])
    })
  }

  isUrl(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      let convertedUrl;
      try {
        convertedUrl = new URL(control.value);
      } catch (ex) {
        console.log(ex);
        return {isUrl: true};
      }
      return convertedUrl.protocol === "http:" || convertedUrl.protocol === "https:"
        ? null : {isUrl: true};
    }
  }

  submitForm() {
    this.isSubmitting = true;
    this.boardService.createItem(this.title.value, this.note.value, this.imageUrl.value).subscribe({
      next: data => {
        this.isSubmitting = false;
        this.modalService.close('create-item');
      },
      error: error => {
        this.isSubmitting = false;
        this.errorMessages = Object.values<string>(error.error.errors);
        console.log(error);
      }
    })
  }
}
