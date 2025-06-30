import { createAction, props } from "@ngrx/store";
import { FreelancerModel, CreateFreelancerModel, UpdateFreelancerModel } from "../../Models/Freelancer.model";
import { PaginationModel } from "../../Models/PaginationModel";

export const loadFreelancers = createAction('[Freelancer] Load Freelancers',
    props<{ page?: number; pageSize?: number; 
    search?: string;
    skill?: string;
    location?: string;
    sortBy?: string; }>()
);
export const loadFreelancersSuccess = createAction('[Freelancer] Load Freelancers Success', 
    props<{ freelancers: FreelancerModel[], pagination: PaginationModel }>());
export const loadFreelancersFailure = createAction('[Freelancer] Load Freelancers Failure', props<{ error: string }>());

export const addFreelancer = createAction('[Freelancer] Add Freelancer', props<{ freelancer: CreateFreelancerModel }>());
export const addFreelancerSuccess = createAction('[Freelancer] Add Freelancer Success', props<{ freelancer: FreelancerModel }>());
export const addFreelancerFailure = createAction('[Freelancer] Add Freelancer Failure', props<{ error: string }>());

export const updateFreelancer = createAction('[Freelancer] Update Freelancer', props<{ freelancerId: string, freelancer: UpdateFreelancerModel }>());
export const updateFreelancerSuccess = createAction('[Freelancer] Update Freelancer Success', props<{ freelancer: FreelancerModel }>());
export const updateFreelancerFailure = createAction('[Freelancer] Update Freelancer Failure', props<{ error: string }>());

export const deleteFreelancer = createAction('[Freelancer] Delete Freelancer', props<{ freelancerId: string }>());
export const deleteFreelancerSuccess = createAction('[Freelancer] Delete Freelancer Success', props<{ freelancer: FreelancerModel }>());
export const deleteFreelancerFailure = createAction('[Freelancer] Delete Freelancer Failure', props<{ error: string }>());