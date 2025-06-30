import { createAction, props } from "@ngrx/store";
import { ProposalModel, CreateProposalModel, UpdateProposalModel } from "../../Models/Proposal.model";
import { PaginationModel } from "../../Models/PaginationModel";

export const loadProposals = createAction('[Proposal] Load Proposals',
    props<{ 
        clientId?: string;
        freelancerId?: string;
        page?: number; 
        pageSize?: number;
        search?: string; 
        sortBy?: string; 
     }>()
);
export const loadProposalsSuccess = createAction('[Proposal] Load Proposals Success', 
    props<{ proposals: ProposalModel[], pagination: PaginationModel }>());
export const loadProposalsFailure = createAction('[Proposal] Load Proposals Failure', props<{ error: string }>());

export const addProposal = createAction('[Proposal] Add Proposal', props<{ proposal: CreateProposalModel }>());
export const addProposalSuccess = createAction('[Proposal] Add Proposal Success', props<{ proposal: ProposalModel }>());
export const addProposalFailure = createAction('[Proposal] Add Proposal Failure', props<{ error: string }>());

export const updateProposal = createAction('[Proposal] Update Proposal', props<{ proposalId: string, proposal: UpdateProposalModel }>());
export const updateProposalSuccess = createAction('[Proposal] Update Proposal Success', props<{ proposal: ProposalModel }>());
export const updateProposalFailure = createAction('[Proposal] Update Proposal Failure', props<{ error: string }>());

export const deleteProposal = createAction('[Proposal] Delete Proposal', props<{ proposalId: string }>());
export const deleteProposalSuccess = createAction('[Proposal] Delete Proposal Success', props<{ proposal: ProposalModel }>());
export const deleteProposalFailure = createAction('[Proposal] Delete Proposal Failure', props<{ error: string }>());

export const loadProposalsByFreelancer = createAction('[Proposal] Load Proposals By Freelancer',
    props<{ freelancerId: string; page?: number; pageSize?: number; search?: string; sortBy?: string }>()
);
export const loadProposalsByFreelancerSuccess = createAction('[Proposal] Load Proposals By Freelancer Success',
    props<{ proposals: ProposalModel[], pagination: PaginationModel }>());
export const loadProposalsByFreelancerFailure = createAction('[Proposal] Load Proposals By Freelancer Failure', props<{ error: string }>());

export const loadProposalsByProject = createAction('[Proposal] Load Proposals By Project',
    props<{ projectId: string; page?: number; pageSize?: number; search?: string; sortBy?: string }>()
);
export const loadProposalsByProjectSuccess = createAction('[Proposal] Load Proposals By Project Success',
    props<{ proposals: ProposalModel[], pagination: PaginationModel, projectId: string }>());
export const loadProposalsByProjectFailure = createAction('[Proposal] Load Proposals By Project Failure', props<{ error: string }>());
