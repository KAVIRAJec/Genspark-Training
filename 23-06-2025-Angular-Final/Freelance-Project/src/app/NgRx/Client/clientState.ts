import { ClientModel } from "../../Models/Client.model";
import { PaginationModel } from "../../Models/PaginationModel";

 
export interface ClientState {
  clients: ClientModel[];
  loading: boolean;
  error: string | null;
  pagination: PaginationModel | null;
}

export const initialClientState: ClientState = {
  clients: [],
  loading: false,
  error: null,
  pagination: null
};