import { createFeatureSelector, createSelector } from '@ngrx/store';
import { ChatState } from './chatState';

export const selectChatState = createFeatureSelector<ChatState>('chat');

export const selectChatMessages = createSelector(selectChatState, state => state.messages);

export const selectChatPagination = createSelector(selectChatState, state => state.pagination);

export const selectChatLoading = createSelector(selectChatState, state => state.loading);

export const selectChatError = createSelector(selectChatState, state => state.error);
