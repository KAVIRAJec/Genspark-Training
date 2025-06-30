import { inject, Injectable } from "@angular/core";
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { ProjectService } from "../../Services/project.service";
import { catchError, map, mergeMap, of } from "rxjs";
import * as ProjectActions from "./project.actions";

@Injectable()
export class ProjectEffects {
    private actions$ = inject(Actions);
    private projectService = inject(ProjectService);

    loadProjects$ = createEffect(() =>
        this.actions$.pipe(
            ofType(ProjectActions.loadProjects),
            mergeMap(({ page, pageSize, search, sortBy }) =>
                this.projectService.getAllProjects(page, pageSize, search, sortBy).pipe(
                    map(res => {
                        if (res.success) {
                            return ProjectActions.loadProjectsSuccess({ 
                                projects: res.data.data, 
                                pagination: res.data.pagination 
                            });
                        } else {
                            return ProjectActions.loadProjectsFailure({ error: res.errors || res.message });
                        }
                    }),
                    catchError(error => of(ProjectActions.loadProjectsFailure({ error })))
                )
            )
        )
    );

    addProject$ = createEffect(() =>
        this.actions$.pipe(
            ofType(ProjectActions.addProject),
            mergeMap(({ project }) =>
                this.projectService.createProject(project).pipe(
                    map(res => {
                        if (res.success) {
                            return ProjectActions.addProjectSuccess({ project: res.data });
                        } else {
                            return ProjectActions.addProjectFailure({ error: res.errors || res.message });
                        }
                    }),
                    catchError(error => of(ProjectActions.addProjectFailure({ error })))
                )
            )
        )
    );

    updateProject$ = createEffect(() =>
        this.actions$.pipe(
            ofType(ProjectActions.updateProject),
            mergeMap(({ projectId, project }) =>
                this.projectService.updateProject(projectId, project).pipe(
                    map(res => {
                        if (res.success) {
                            return ProjectActions.updateProjectSuccess({ project: res.data });
                        } else {
                            return ProjectActions.updateProjectFailure({ error: res.errors || res.message });
                        }
                    }),
                    catchError(error => of(ProjectActions.updateProjectFailure({ error })))
                )
            )
        )
    );

    deleteProject$ = createEffect(() =>
        this.actions$.pipe(
            ofType(ProjectActions.deleteProject),
            mergeMap(({ projectId }) =>
                this.projectService.deleteProject(projectId).pipe(
                    map(res => {
                        if (res.success) {
                            return ProjectActions.deleteProjectSuccess({ project: res.data });
                        } else {
                            return ProjectActions.deleteProjectFailure({ error: res.errors || res.message });
                        }
                    }),
                    catchError(error => of(ProjectActions.deleteProjectFailure({ error })))
                )
            )
        )
    );
}
