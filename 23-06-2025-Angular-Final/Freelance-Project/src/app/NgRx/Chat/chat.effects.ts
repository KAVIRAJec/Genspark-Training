import { inject, Injectable } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { ChatService } from '../../Services/chat.service';
import * as ChatActions from './chat.actions';
import { catchError, map, mergeMap, of } from 'rxjs';

@Injectable()
export class ChatEffects {
  private actions$ = inject(Actions);
    private chatService = inject(ChatService);

  loadMessages$ = createEffect(() =>
    this.actions$.pipe(
      ofType(ChatActions.loadMessages),
      mergeMap(({ chatRoomId, page, pageSize }) =>
        this.chatService.getMessagesByChatRoomId(chatRoomId, page, pageSize).pipe(
          map(res => {
            if (res.success) {
              return ChatActions.loadMessagesSuccess({ messages: res.data.data, pagination: res.data.pagination });
            } else {
              return ChatActions.loadMessagesFailure({ error: res.errors || res.message });
            }
        }),
          catchError(error => of(ChatActions.loadMessagesFailure({ error: error.message || 'Failed to load messages' })))
        )
      )
    )
  );

  sendMessage$ = createEffect(() =>
    this.actions$.pipe(
      ofType(ChatActions.sendMessage),
      mergeMap(({ content, chatRoomId, senderId }) =>
        this.chatService.sendMessage(content, chatRoomId, senderId).pipe(
          map(res => {
            if (res.success) {
              return ChatActions.sendMessageSuccess({ message: res.data });
            } else {
              return ChatActions.sendMessageFailure({ error: res.errors || res.message });
            }
        }),
          catchError(error => of(ChatActions.sendMessageFailure({ error: error.message || 'Failed to send message' })))
        )
      )
    )
  );

  updateMessage$ = createEffect(() =>
    this.actions$.pipe(
      ofType(ChatActions.updateMessage),
      mergeMap(({ messageId, content }) =>
        this.chatService.updateMessage(messageId, content).pipe(
          map(res => {
            if (res.success) {
              return ChatActions.updateMessageSuccess({ message: res.data });
            } else {
              return ChatActions.updateMessageFailure({ error: res.errors || res.message });
            }
          }),
          catchError(error => of(ChatActions.updateMessageFailure({ error: error.message || 'Failed to update message' })))
        )
      )
    )
  );

  deleteMessage$ = createEffect(() =>
    this.actions$.pipe(
      ofType(ChatActions.deleteMessage),
      mergeMap(({ messageId, chatRoomId }) =>
        this.chatService.deleteMessage(messageId, chatRoomId).pipe(
          map(res => {
            if (res.success) {
              return ChatActions.deleteMessageSuccess({ messageId });
            } else {
              return ChatActions.deleteMessageFailure({ error: res.errors || res.message });
            }
          }),
          catchError(error => of(ChatActions.deleteMessageFailure({ error: error.message || 'Failed to delete message' })))
        )
      )
    )
  );

  markMessageAsRead$ = createEffect(() =>
    this.actions$.pipe(
      ofType(ChatActions.markMessageAsRead),
      mergeMap(({ messageId, chatRoomId, senderId }) =>
        this.chatService.markMessageAsRead(messageId, chatRoomId, senderId).pipe(
          map(res => {
            if (res.success) {
              return ChatActions.markMessageAsReadSuccess({ message: res.data });
            } else {
              return ChatActions.markMessageAsReadFailure({ error: res.errors || res.message });
            }
          }),
          catchError(error => of(ChatActions.markMessageAsReadFailure({ error: error.message || 'Failed to mark as read' })))
        )
      )
    )
  );
}
