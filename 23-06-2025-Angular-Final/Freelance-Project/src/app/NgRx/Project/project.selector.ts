import { createFeatureSelector, createSelector } from "@ngrx/store";
import { ProjectState } from "./projectState";

export const selectProjectState = createFeatureSelector<ProjectState>('project');

export const selectAllProjects = createSelector(selectProjectState, state => state.projects);
export const selectProjectPagination = createSelector(selectProjectState, state => state.pagination);
export const selectProjectLoading = createSelector(selectProjectState, state => state.loading);
export const selectProjectError = createSelector(selectProjectState, state => state.error);
