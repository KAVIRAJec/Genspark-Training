import { createReducer, on } from "@ngrx/store";
import * as ClientActions from "./client.actions";
import { initialClientState } from "./clientState";

export const clientReducer = createReducer(initialClientState,
    on(ClientActions.loadClients, state => ({ ...state, 
        loading: true, error: null 
    })),
    on(ClientActions.loadClientsSuccess, (state, { clients, pagination }) => ({
        ...state, clients, pagination,
        loading: false, error: null
    })),
    on(ClientActions.loadClientsFailure, (state, { error }) => ({
        ...state, loading: false, error
    })),

    on(ClientActions.addClient, state => ({
        ...state, loading: true, error: null
    })),
    on(ClientActions.addClientSuccess, (state, { client }) => ({
        ...state,
        clients: [...state.clients, client],
        loading: false,
        error: null
    })),
    on(ClientActions.addClientFailure, (state, { error }) => ({
        ...state, loading: false, error
    })),

    on(ClientActions.updateClientSuccess, (state, { client }) => ({
        ...state,
        clients: state.clients.map(c => c.id === client.id ? client : c),
        loading: false,
        error: null
    })),
    on(ClientActions.updateClientFailure, (state, { error }) => ({
        ...state, loading: false, error
    })),
    on(ClientActions.deleteClientSuccess, (state, { client }) => ({
        ...state,
        clients: state.clients.filter(c => c.id !== client.id),
        loading: false,
        error: null
    })),
    on(ClientActions.deleteClientFailure, (state, { error }) => ({
        ...state, loading: false, error
    })),
)