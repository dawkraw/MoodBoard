import {Component, OnDestroy, OnInit} from '@angular/core';
import {ActivatedRoute, Router} from "@angular/router";
import Konva from "konva";
import {BoardHubService} from "../services/board-hub.service";
import {BoardService} from "../services/board.service";
import {ModalService} from "../services/modal.service";
import {BoardItem} from "../models/board-item.model";
import {fromEvent} from "rxjs";
import Image = Konva.Image;
import Stage = Konva.Stage;
import Layer = Konva.Layer;

@Component({
  selector: 'app-board',
  templateUrl: './board.component.html',
  styleUrls: ['./board.component.css'],
  providers: [{provide: BoardService}]
})
export class BoardComponent implements OnInit, OnDestroy {
  stage!: Stage;
  selectedItem: BoardItem | undefined;
  heightScale!: number;
  widthScale!: number;

  resizeSub!: any;
  itemCreationSub!: any;
  itemDeletionSub!: any;
  itemUpdatedSub!: any;

  constructor(private route: ActivatedRoute, private router: Router,
              public boardService: BoardService, public boardHubService: BoardHubService,
              private modalService: ModalService) {
  }

  ngOnInit(): void {
    this.subscribeToEvents();
    this.route.params.subscribe(params => {
      this.boardService.initBoard(params['boardId'])
        .then(() => {
          this.generateCanvas()
        });
    })
    let resizeObservable = fromEvent(window, 'resize')
    this.resizeSub = resizeObservable.subscribe(evt => {
      this.generateCanvas();
    })
  }

  generateCanvas(): void {
    this.stage = new Stage({
      container: 'container',
      width: window.innerWidth,
      height: window.innerHeight,
    });
    let container = document.querySelector('#container') as HTMLElement;
    if (container != null) {
      this.heightScale = container.offsetHeight / 975; //intended size
      this.widthScale = container.offsetWidth / 1920; //intended size
    }
    this.boardService.board.items?.forEach(item => {
      this.createItem(item);
    })
    if (this.boardService.board.items?.length == 0) {
      this.modalService.open("create-item-prompt");
    }
  }

  createItem(item: BoardItem) {
    let layer = new Layer();
    Konva.Image.fromURL(item.imageUrl, (img: Image) => {
      img.setAttrs({
        width: item.size.x * this.widthScale,
        height: item.size.y * this.heightScale,
        x: item.position.x * this.widthScale,
        y: item.position.y * this.heightScale,
        rotation: item.rotation,
        name: item.title,
        id: item.boardItemId,
        draggable: true,
      })
      layer.add(img);

      let transformer = new Konva.Transformer({
        nodes: [img],
        keepRatio: false,
        boundBoxFunc: (oldBox, newBox) => {
          if (newBox.width < 10 || newBox.height < 10) {
            return oldBox;
          }
          return newBox;
        },
      });
      layer.add(transformer)

      img.on('dragend', () => {
        this.boardService.updateItemTransform(item,
          img.attrs.x / this.widthScale,
          img.attrs.y / this.heightScale,
          img.attrs.rotation,
          img.attrs.width / this.widthScale,
          img.attrs.height / this.heightScale);
      })

      img.on('transformend', () => {
        img.setAttrs({
          width: img.width() * img.scaleX(),
          height: img.height() * img.scaleY(),
          scaleX: 1,
          scaleY: 1
        })
        this.boardService.updateItemTransform(item,
          img.attrs.x / this.widthScale,
          img.attrs.y / this.heightScale,
          img.attrs.rotation,
          img.attrs.width / this.widthScale,
          img.attrs.height / this.heightScale);
      })

      img.on('dblclick', () => {
        this.showItem(img.attrs.id);
      })
    })
    this.stage.add(layer)
  }

  subscribeToEvents(){
    this.itemCreationSub = this.boardHubService.itemCreated.subscribe(data => {
      this.boardService.board.items?.push(data);
      this.createItem(data);
    });

    this.itemDeletionSub = this.boardHubService.itemDeleted.subscribe(data => {
      let indexToDelete = this.boardService.board.items?.findIndex((value) => value.boardItemId == data.boardItemId);
      if (indexToDelete) {
        this.boardService.board.items?.splice(indexToDelete, 1);
      }
    });

    this.itemUpdatedSub = this.boardHubService.placementUpdated.subscribe(data => {
      let indexOfItem = this.boardService.board.items?.findIndex((value) => value.boardItemId == data.boardItemId);
      if (indexOfItem != undefined && this.boardService.board.items != undefined) {
        this.boardService.board.items[indexOfItem].size = data.size;
        this.boardService.board.items[indexOfItem].position = data.position;
        this.boardService.board.items[indexOfItem].rotation = data.rotation;
      }
      let image = this.stage?.find("#" + data.boardItemId)[0];
      image?.setAttrs({
        width: data.size?.x * this.widthScale,
        height: data.size?.y * this.heightScale,
        x: data.position?.x * this.widthScale,
        y: data.position?.y * this.heightScale,
        rotation: data.rotation,
      })
      image?.draw();
    });
  }

  openModal(id: string) {
    this.modalService.open(id);
  }

  showItem(id: string) {
    let indexOfItem = this.boardService.board.items?.findIndex((value) => value.boardItemId == id);
    if (indexOfItem != undefined && this.boardService.board.items != undefined) {
      this.selectedItem = this.boardService.board.items[indexOfItem];
      this.openModal('item-info');
    }
  }

  ngOnDestroy(): void {
    this.boardService.leaveBoard();
    this.resizeSub.unsubscribe();
    this.itemUpdatedSub.unsubscribe();
    this.itemDeletionSub.unsubscribe();
    this.itemCreationSub.unsubscribe();
  }
}
