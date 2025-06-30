import { createFeatureSelector, createSelector } from "@ngrx/store";
import { ClientState } from "./clientState";

export const selectClientState = createFeatureSelector<ClientState>('client');

export const selectAllClients = createSelector(selectClientState, state => state.clients);
export const selectClientPagination = createSelector(selectClientState, state => state.pagination);
export const selectClientLoading = createSelector(selectClientState, state => state.loading);
export const selectClientError = createSelector(selectClientState, state => state.error);