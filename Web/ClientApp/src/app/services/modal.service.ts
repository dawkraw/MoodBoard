import {Injectable} from '@angular/core';
import {ModalComponent} from "../modal/modal.component";

@Injectable({
  providedIn: 'root'
})
export class ModalService {
  private modals: ModalComponent[] = [];
  private activeModal: ModalComponent | undefined;

  add(modal: ModalComponent) {
    this.modals.push(modal);
  }

  remove(id: string) {
    this.modals = this.modals.filter(m => m.modalId !== id);
  }

  open(id: string) {
    if (this.activeModal !== undefined) this.activeModal.close();
    let modal = this.modals.find(m => m.modalId === id);
    if (modal === undefined) return;
    this.activeModal = modal;
    modal.open();
  }

  close(id: string) {
    let modal = this.modals.find(m => m.modalId === id);
    if (modal === undefined) return;
    modal.close();
  }
}
