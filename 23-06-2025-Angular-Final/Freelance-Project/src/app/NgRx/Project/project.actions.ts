import { createAction, props } from "@ngrx/store";
import { CreateProjectModel, ProjectModel, UpdateProjectModel } from "../../Models/Project.model";
import { PaginationModel } from "../../Models/PaginationModel";

export const loadProjects = createAction('[Project] Load Projects',
    props<{ page?: number; 
        pageSize?: number;
        search?: string; 
        sortBy?: string; 
     }>()

);
export const loadProjectsSuccess = createAction('[Project] Load Projects Success', 
    props<{ projects: ProjectModel[], pagination: PaginationModel }>());
export const loadProjectsFailure = createAction('[Project] Load Projects Failure', props<{ error: string }>());

export const addProject = createAction('[Project] Add Project', props<{ project: CreateProjectModel }>());
export const addProjectSuccess = createAction('[Project] Add Project Success', props<{ project: ProjectModel }>());
export const addProjectFailure = createAction('[Project] Add Project Failure', props<{ error: string }>());

export const updateProject = createAction('[Project] Update Project', props<{ projectId: string, project: UpdateProjectModel }>());
export const updateProjectSuccess = createAction('[Project] Update Project Success', props<{ project: ProjectModel }>());
export const updateProjectFailure = createAction('[Project] Update Project Failure', props<{ error: string }>());

export const deleteProject = createAction('[Project] Delete Project', props<{ projectId: string }>());
export const deleteProjectSuccess = createAction('[Project] Delete Project Success', props<{ project: ProjectModel }>());
export const deleteProjectFailure = createAction('[Project] Delete Project Failure', props<{ error: string }>());
