import {BoardItem} from "./board-item.model";
import {BoardUser} from "./board-user.model";

export type Board = {
  boardId: string,
  name: string,
  description: string,
  createdBy: BoardUser,
  lastModifiedBy: BoardUser,
  isOwner: boolean,
  items: BoardItem[],
  boardMembers: BoardUser[]
}
