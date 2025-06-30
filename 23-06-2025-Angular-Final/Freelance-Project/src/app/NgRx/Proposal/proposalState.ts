import { PaginationModel } from "../../Models/PaginationModel";
import { ProposalModel } from "../../Models/Proposal.model";
 
export interface ProposalState {
  proposals: ProposalModel[];
  loading: boolean;
  error: string | null;
  pagination: PaginationModel | null;
}

export const initialProposalState: ProposalState = {
  proposals: [],
  loading: false,
  error: null,
  pagination: null
};