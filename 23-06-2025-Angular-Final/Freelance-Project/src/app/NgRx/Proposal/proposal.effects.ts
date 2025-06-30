import { inject, Injectable } from "@angular/core";
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { ProposalService } from "../../Services/proposal.service"
import { ProjectProposalService } from "../../Services/projectProposal.service";
import { catchError, map, mergeMap, of } from "rxjs";
import * as ProposalActions from "./proposal.actions";

@Injectable()
export class ProposalEffects {
    private actions$ = inject(Actions);
    private proposalService = inject(ProposalService);
    private projectProposalService = inject(ProjectProposalService);

    loadProposals$ = createEffect(() => 
        this.actions$.pipe(
            ofType(ProposalActions.loadProposals),
            mergeMap(({ clientId, freelancerId, page, pageSize, search, sortBy }) => {
                if (clientId) {
                    return this.proposalService.GetProposalsByClientId(clientId, page, pageSize, search, sortBy).pipe(
                        map(res => {
                            if (res.success) {
                                return ProposalActions.loadProposalsSuccess({ 
                                    proposals: res.data.data, 
                                    pagination: res.data.pagination 
                                });
                            } else {
                                return ProposalActions.loadProposalsFailure({ error: res.errors || res.message });
                            }
                        }),
                        catchError(error => of(ProposalActions.loadProposalsFailure({ error })))
                    );
                } else if (freelancerId) {
                    return this.proposalService.GetProposalsByFreelancerId(freelancerId, page, pageSize, search, sortBy).pipe(
                        map(res => {
                            if (res.success) {
                                return ProposalActions.loadProposalsSuccess({ 
                                    proposals: res.data.data, 
                                    pagination: res.data.pagination 
                                });
                            } else {
                                return ProposalActions.loadProposalsFailure({ error: res.errors || res.message });
                            }
                        }),
                        catchError(error => of(ProposalActions.loadProposalsFailure({ error })))
                    );
                } else {
                    return of(ProposalActions.loadProposalsFailure({ error: 'No clientId or freelancerId provided' }));
                }
            })
        )
    );

    addProposal$ = createEffect(() => 
        this.actions$.pipe(
            ofType(ProposalActions.addProposal),
            mergeMap(({ proposal }) => 
                this.proposalService.CreateProposal(proposal).pipe(
                    map(res => {
                        if(res.success) {
                            return ProposalActions.addProposalSuccess({ proposal: res.data });
                        } else {
                            return ProposalActions.addProposalFailure({ error: res.errors || res.message });
                        }
                    }),
                    catchError(error => of(ProposalActions.addProposalFailure({ error })))
                )
            )
        )
    )

    updateProposal$ = createEffect(() =>
        this.actions$.pipe(
            ofType(ProposalActions.updateProposal),
            mergeMap(({ proposalId, proposal }) =>
                this.proposalService.UpdateProposal(proposalId, proposal).pipe(
                    map(res => {
                        if (res.success) {
                            return ProposalActions.updateProposalSuccess({ proposal: res.data });
                        } else {
                            return ProposalActions.updateProposalFailure({ error: res.errors || res.message });
                        }
                    }),
                    catchError(error => of(ProposalActions.updateProposalFailure({ error })))
                )
            )
        )
    );

    deleteProposal$ = createEffect(() =>
        this.actions$.pipe(
            ofType(ProposalActions.deleteProposal),
            mergeMap(({ proposalId }) =>
                this.proposalService.DeleteProposal(proposalId).pipe(
                    map(res => {
                        if (res.success) {
                            return ProposalActions.deleteProposalSuccess({ proposal: res.data });
                        } else {
                            return ProposalActions.deleteProposalFailure({ error: res.errors || res.message });
                        }
                    }),
                    catchError(error => of(ProposalActions.deleteProposalFailure({ error })))
                )
            )
        )
    );

    loadProposalsByFreelancer$ = createEffect(() =>
        this.actions$.pipe(
            ofType(ProposalActions.loadProposalsByFreelancer),
            mergeMap(({ freelancerId, page, pageSize, search, sortBy }) =>
                this.proposalService.GetProposalsByFreelancerId(freelancerId, page, pageSize, search, sortBy).pipe(
                    map(res => {
                        if (res.success) {
                            return ProposalActions.loadProposalsByFreelancerSuccess({
                                proposals: res.data.data,
                                pagination: res.data.pagination
                            });
                        } else {
                            return ProposalActions.loadProposalsByFreelancerFailure({ error: res.errors || res.message });
                        }
                    }),
                    catchError(error => of(ProposalActions.loadProposalsByFreelancerFailure({ error })))
                )
            )
        )
    );

    loadProposalsByProject$ = createEffect(() =>
        this.actions$.pipe(
            ofType(ProposalActions.loadProposalsByProject),
            mergeMap(({ projectId, page, pageSize, search, sortBy }) =>
                this.projectProposalService.GetProposalsByProjectId(projectId, page, pageSize, search, sortBy).pipe(
                    map(res => {
                        if (res.success) {
                            return ProposalActions.loadProposalsByProjectSuccess({
                                proposals: res.data.data,
                                pagination: res.data.pagination,
                                projectId
                            });
                        } else {
                            return ProposalActions.loadProposalsByProjectFailure({ error: res.errors || res.message });
                        }
                    }),
                    catchError(error => of(ProposalActions.loadProposalsByProjectFailure({ error })))
                )
            )
        )
    );
}