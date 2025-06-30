import { ChatMessageModel } from '../../Models/ChatMessage.model';
import { PaginationModel } from '../../Models/PaginationModel';

export interface ChatState {
  messages: ChatMessageModel[];
  loading: boolean;
  error: string | null;
  pagination: PaginationModel | null;
}

export const initialChatState: ChatState = {
  messages: [],
  loading: false,
  error: null,
  pagination: null,
};
