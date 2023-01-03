import {ItemComment} from "./comment.model";

export type BoardItem = {
  boardItemId: string,
  boardId: string,
  title: string,
  note: string,
  comments: ItemComment[],
  imageUrl: string,
  position: CanvasVector,
  size: CanvasVector,
  rotation: number,
};

export type CanvasVector = {
  x: number,
  y: number,
}
