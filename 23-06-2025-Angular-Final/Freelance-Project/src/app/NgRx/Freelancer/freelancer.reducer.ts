import { createReducer, on } from "@ngrx/store";
import * as FreelancerActions from "./freelancer.actions";
import { initialFreelancerState } from "./freelancerState";

export const freelancerReducer = createReducer(initialFreelancerState,
    on(FreelancerActions.loadFreelancers, state => ({ ...state, 
        loading: true, error: null 
    })),
    on(FreelancerActions.loadFreelancersSuccess, (state, { freelancers, pagination }) => ({
        ...state, freelancers, loading: false, error: null, pagination
    })),
    on(FreelancerActions.loadFreelancersFailure, (state, { error }) => ({
        ...state, loading: false, error
    })),

    on(FreelancerActions.addFreelancer, state => ({
        ...state, loading: true, error: null
    })),
    on(FreelancerActions.addFreelancerSuccess, (state, { freelancer }) => ({
        ...state,
        freelancers: [...state.freelancers, freelancer],
        loading: false,
        error: null
    })),
    on(FreelancerActions.addFreelancerFailure, (state, { error }) => ({
        ...state, loading: false, error
    })),

    on(FreelancerActions.updateFreelancerSuccess, (state, { freelancer }) => ({
        ...state,
        freelancers: state.freelancers.map(f => f.id === freelancer.id ? freelancer : f),
        loading: false,
        error: null
    })),
    on(FreelancerActions.updateFreelancerFailure, (state, { error }) => ({
        ...state, loading: false, error
    })),
    on(FreelancerActions.deleteFreelancerSuccess, (state, { freelancer }) => ({
        ...state,
        freelancers: state.freelancers.filter(f => f.id !== freelancer.id),
        loading: false,
        error: null
    })),
    on(FreelancerActions.deleteFreelancerFailure, (state, { error }) => ({
        ...state, loading: false, error
    })),
);