import {Component, ElementRef, Input, OnDestroy, OnInit, ViewEncapsulation} from '@angular/core';
import {ModalService} from "../services/modal.service";

@Component({
  selector: 'app-modal',
  templateUrl: './modal.component.html',
  encapsulation: ViewEncapsulation.None
})
export class ModalComponent implements OnInit, OnDestroy {
  @Input() modalId!: string;
  @Input() title!: string;
  private domElement!: any;
  private previousOverFlow!: any;

  constructor(private modalService: ModalService, private elementRef: ElementRef) {
    this.domElement = elementRef.nativeElement;
  }


  ngOnInit() {
    if (this.modalId == undefined) {
      console.error("Modal doesn't have an id!");
      return;
    }

    this.domElement.classList.add("hidden");
    this.domElement.addEventListener("click", (element: any) => {
      if (element.target.classList.contains("z-50")) {
        this.close();
      }
    });

    this.modalService.add(this);
  }

  ngOnDestroy() {
    this.modalService.remove(this.modalId);
    this.domElement.remove();
  }

  open() {
    this.domElement.classList.remove("hidden");
    this.previousOverFlow = document.body.style.overflow.toString();
    document.body.style.overflow = "hidden";
  }

  close() {
    this.domElement.classList.add("hidden");
    document.body.style.overflow = this.previousOverFlow;
  }

}
