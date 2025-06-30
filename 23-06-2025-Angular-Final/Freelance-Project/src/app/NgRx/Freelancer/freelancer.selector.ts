import { createFeatureSelector, createSelector } from "@ngrx/store";
import { FreelancerState } from "./freelancerState";

export const selectFreelancerState = createFeatureSelector<FreelancerState>('freelancer');

export const selectAllFreelancers = createSelector(selectFreelancerState, state => state.freelancers);
export const selectFreelancerPagination = createSelector(selectFreelancerState, state => state.pagination);
export const selectFreelancerLoading = createSelector(selectFreelancerState, state => state.loading);
export const selectFreelancerError = createSelector(selectFreelancerState, state => state.error);