import {BoardUser} from "./board-user.model";

export type ItemComment = {
  createdBy: BoardUser,
  commentId: string,
  content: string
}
