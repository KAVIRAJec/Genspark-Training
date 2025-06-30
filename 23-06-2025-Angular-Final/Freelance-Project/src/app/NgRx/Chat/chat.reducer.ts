import { createReducer, on } from '@ngrx/store';
import * as ChatActions from './chat.actions';
import { ChatState, initialChatState } from './chatState';

export const chatReducer = createReducer(initialChatState,
  on(ChatActions.loadMessages, (state) => ({ ...state, 
    loading: true, error: null 
})),
  on(ChatActions.loadMessagesSuccess, (state, { messages, pagination }) => ({
    ...state, 
    messages: state.messages.length === 0 || pagination.page === 1
    ? messages
    : [...state.messages, ...messages], // append older messages to the end
    pagination,
    loading: false, error: null
  })),
  on(ChatActions.loadMessagesFailure, (state, { error }) => ({ 
    ...state, loading: false, error 
})),

  on(ChatActions.sendMessage, (state) => ({ ...state, loading: true, error: null })),
  on(ChatActions.sendMessageSuccess, (state, { message }) => {
    // Prevent duplicate messages (from SignalR and sendMessageSuccess)
    if (state.messages.some(m => m.id === message.id)) {
      return state;
    }
    return {
      ...state,
      messages: [message, ...state.messages],
      loading: false,
      error: null,
    };
  }),
  on(ChatActions.sendMessageFailure, (state, { error }) => ({ 
    ...state, loading: false, error 
})),

  on(ChatActions.updateMessage, (state) => ({ ...state, loading: true, error: null })),
  on(ChatActions.updateMessageSuccess, (state, { message }) => ({
    ...state,
    messages: state.messages.map(m => m.id === message.id ? message : m),
    loading: false,
    error: null,
  })),
  on(ChatActions.updateMessageFailure, (state, { error }) => ({ 
    ...state, loading: false, error 
})),

  on(ChatActions.deleteMessage, (state) => ({ ...state, loading: true, error: null })),
  on(ChatActions.deleteMessageSuccess, (state, { messageId }) => ({
    ...state,
    messages: state.messages.filter(m => m.id !== messageId),
    loading: false,
    error: null,
  })),
  on(ChatActions.deleteMessageFailure, (state, { error }) => ({ 
    ...state, loading: false, error 
})),

on(ChatActions.markMessageAsReadSuccess, (state, { message }) => ({
  ...state,
  messages: state.messages.map(m => m.id === message.id ? message : m),
  loading: false,
  error: null,
})),
on(ChatActions.markMessageAsReadFailure, (state, { error }) => ({
  ...state, loading: false, error
})),

on(ChatActions.receiveMessage, (state, { message }) => {
  // Prevent duplicate messages (from sendMessageSuccess and SignalR)
  if (state.messages.some(m => m.id === message.id)) {
    return state;
  }
  return {
    ...state,
    messages: [message, ...state.messages],
  };
}),
);
