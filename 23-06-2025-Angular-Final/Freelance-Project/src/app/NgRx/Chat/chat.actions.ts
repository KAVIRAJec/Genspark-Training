import { createAction, props } from "@ngrx/store";
import { ChatMessageModel } from "../../Models/ChatMessage.model";
import { PaginationModel } from "../../Models/PaginationModel";

export const loadMessages = createAction('[Chat] Load Messages', 
    props<{ chatRoomId: string, page?: number, pageSize?: number }>());
export const loadMessagesSuccess = createAction('[Chat] Load Messages Success', 
    props<{ messages: ChatMessageModel[], pagination: PaginationModel }>());
export const loadMessagesFailure = createAction('[Chat] Load Messages Failure', props<{ error: string }>());

export const sendMessage = createAction('[Chat] Send Message', props<{ content: string, chatRoomId: string, senderId: string }>());
export const sendMessageSuccess = createAction('[Chat] Send Message Success', props<{ message: ChatMessageModel }>());
export const sendMessageFailure = createAction('[Chat] Send Message Failure', props<{ error: string }>());

export const updateMessage = createAction('[Chat] Update Message', props<{ messageId: string, content: string }>());
export const updateMessageSuccess = createAction('[Chat] Update Message Success', props<{ message: ChatMessageModel }>());
export const updateMessageFailure = createAction('[Chat] Update Message Failure', props<{ error: string }>());

export const markMessageAsRead = createAction('[Chat] Mark Message As Read', props<{ messageId: string, chatRoomId: string, senderId: string }>());
export const markMessageAsReadSuccess = createAction('[Chat] Mark Message As Read Success', props<{ message: ChatMessageModel }>());
export const markMessageAsReadFailure = createAction('[Chat] Mark Message As Read Failure', props<{ error: string }>());

export const deleteMessage = createAction('[Chat] Delete Message', props<{ messageId: string, chatRoomId: string }>());
export const deleteMessageSuccess = createAction('[Chat] Delete Message Success', props<{ messageId: string }>());
export const deleteMessageFailure = createAction('[Chat] Delete Message Failure', props<{ error: string }>());

export const receiveMessage = createAction('[Chat] Receive Message', props<{ message: ChatMessageModel }>());