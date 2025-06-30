import { createReducer, on } from "@ngrx/store";
import * as ProjectActions from "./project.actions";
import { initialProjectState } from "./projectState";

export const projectReducer = createReducer(initialProjectState,
    on(ProjectActions.loadProjects, state => ({ ...state, 
        loading: true, error: null 
    })),
    on(ProjectActions.loadProjectsSuccess, (state, { projects, pagination }) => ({
        ...state, projects, pagination,
        loading: false, error: null
    })),
    on(ProjectActions.loadProjectsFailure, (state, { error }) => ({
        ...state, loading: false, error
    })),

    on(ProjectActions.addProjectSuccess, (state, { project }) => ({
        ...state,
        projects: [...state.projects, project],
        loading: false,
        error: null
    })),
    on(ProjectActions.addProjectFailure, (state, { error }) => ({
        ...state, loading: false, error
    })),

    on(ProjectActions.updateProjectSuccess, (state, { project }) => ({
        ...state,
        projects: state.projects.map(p => p.id === project.id ? project : p),
        loading: false,
        error: null
    })),
    on(ProjectActions.updateProjectFailure, (state, { error }) => ({
        ...state, loading: false, error
    })),
    on(ProjectActions.deleteProjectSuccess, (state, { project }) => ({
        ...state,
        projects: state.projects.filter(p => p.id !== project.id),
        loading: false,
        error: null
    })),
    on(ProjectActions.deleteProjectFailure, (state, { error }) => ({
        ...state, loading: false, error
    })),
);
