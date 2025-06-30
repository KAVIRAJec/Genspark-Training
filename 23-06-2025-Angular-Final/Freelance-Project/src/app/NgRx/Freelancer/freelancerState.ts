import { FreelancerModel } from "../../Models/Freelancer.model";
import { PaginationModel } from "../../Models/PaginationModel";

export interface FreelancerState {
  freelancers: FreelancerModel[];
  loading: boolean;
  error: string | null;
  pagination: PaginationModel | null;
};

export const initialFreelancerState: FreelancerState = {
  freelancers: [],
  loading: false,
  error: null,
  pagination: null
};