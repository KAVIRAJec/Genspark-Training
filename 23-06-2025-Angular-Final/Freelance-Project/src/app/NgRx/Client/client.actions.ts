import { createAction, props } from "@ngrx/store";
import { ClientModel, CreateClientModel, UpdateClientModel } from "../../Models/Client.model";
import { PaginationModel } from "../../Models/PaginationModel";

export const loadClients = createAction('[Client] Load Clients',
    props<{ page?: number; pageSize?: number }>()
);
export const loadClientsSuccess = createAction('[Client] Load Clients Success', 
    props<{ clients: ClientModel[], pagination: PaginationModel }>());
export const loadClientsFailure = createAction('[Client] Load Clients Failure', props<{ error: string }>());

export const addClient = createAction('[Client] Add Client', props<{ client: CreateClientModel }>());
export const addClientSuccess = createAction('[Client] Add Client Success', props<{ client: ClientModel }>());
export const addClientFailure = createAction('[Client] Add Client Failure', props<{ error: string }>());

export const updateClient = createAction('[Client] Update Client', props<{ clientId: string, client: UpdateClientModel }>());
export const updateClientSuccess = createAction('[Client] Update Client Success', props<{ client: ClientModel }>());
export const updateClientFailure = createAction('[Client] Update Client Failure', props<{ error: string }>());

export const deleteClient = createAction('[Client] Delete Client', props<{ clientId: string }>());
export const deleteClientSuccess = createAction('[Client] Delete Client Success', props<{ client: ClientModel }>());
export const deleteClientFailure = createAction('[Client] Delete Client Failure', props<{ error: string }>());