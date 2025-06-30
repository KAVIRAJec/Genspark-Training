import { inject, Injectable } from "@angular/core";
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { FreelancerService } from "../../Services/freelancer.service";
import { catchError, map, mergeMap, of } from "rxjs";
import * as FreelancerActions from "./freelancer.actions";

@Injectable()
export class FreelancerEffects {
    private actions$ = inject(Actions);
    private freelancerService = inject(FreelancerService);

    loadFreelancers$ = createEffect(() =>
        this.actions$.pipe(
            ofType(FreelancerActions.loadFreelancers),
            mergeMap(({ page, pageSize, search, sortBy }) =>
                this.freelancerService.getAllFreelancers(page, pageSize, search, sortBy).pipe(
                    map(res => {
                        if (res.success) {
                            return FreelancerActions.loadFreelancersSuccess({ 
                                freelancers: res.data.data, 
                                pagination: res.data.pagination 
                            });
                        } else {
                            return FreelancerActions.loadFreelancersFailure({ error: res.errors || res.message });
                        }
                    }),
                    catchError(error => of(FreelancerActions.loadFreelancersFailure({ error })))
                )
            )
        )
    );

    addFreelancer$ = createEffect(() =>
        this.actions$.pipe(
            ofType(FreelancerActions.addFreelancer),
            mergeMap(({ freelancer }) =>
                this.freelancerService.createFreelancer(freelancer).pipe(
                    map(res => {
                        if (res.success) {
                            return FreelancerActions.addFreelancerSuccess({ freelancer: res.data });
                        } else {
                            return FreelancerActions.addFreelancerFailure({ error: res.errors || res.message });
                        }
                    }),
                    catchError(error => of(FreelancerActions.addFreelancerFailure({ error })))
                )
            )
        )
    );

    updateFreelancer$ = createEffect(() =>
        this.actions$.pipe(
            ofType(FreelancerActions.updateFreelancer),
            mergeMap(({ freelancerId, freelancer }) =>
                this.freelancerService.updateFreelancer(freelancerId, freelancer).pipe(
                    map(res => {
                        if (res.success) {
                            return FreelancerActions.updateFreelancerSuccess({ freelancer: res.data });
                        } else {
                            return FreelancerActions.updateFreelancerFailure({ error: res.errors || res.message });
                        }
                    }),
                    catchError(error => of(FreelancerActions.updateFreelancerFailure({ error })))
                )
            )
        )
    );

    deleteFreelancer$ = createEffect(() =>
        this.actions$.pipe(
            ofType(FreelancerActions.deleteFreelancer),
            mergeMap(({ freelancerId }) =>
                this.freelancerService.deleteFreelancer(freelancerId).pipe(
                    map(res => {
                        if (res.success) {
                            return FreelancerActions.deleteFreelancerSuccess({ freelancer: res.data });
                        } else {
                            return FreelancerActions.deleteFreelancerFailure({ error: res.errors || res.message });
                        }
                    }),
                    catchError(error => of(FreelancerActions.deleteFreelancerFailure({ error })))
                )
            )
        )
    );
}