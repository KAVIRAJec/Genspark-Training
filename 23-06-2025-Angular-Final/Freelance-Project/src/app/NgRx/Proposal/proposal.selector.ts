import { createFeatureSelector, createSelector } from "@ngrx/store";
import { ProposalState } from "./proposalState";

export const selectProposalState = createFeatureSelector<ProposalState>('proposal');

export const selectAllProposals = createSelector(selectProposalState, state => state.proposals);
export const selectProposalPagination = createSelector(selectProposalState, state => state.pagination);
export const selectProposalLoading = createSelector(selectProposalState, state => state.loading);
export const selectProposalError = createSelector(selectProposalState, state => state.error);