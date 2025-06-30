import { inject, Injectable } from "@angular/core";
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { ClientService } from "../../Services/client.service"
import { catchError, map, mergeMap, of } from "rxjs";
import * as ClientActions from "./client.actions";

@Injectable()
export class ClientEffects {
    private actions$ = inject(Actions);
    private clientService = inject(ClientService);

    loadClients$ = createEffect(() =>
        this.actions$.pipe(
            ofType(ClientActions.loadClients),
            mergeMap(({ page, pageSize }) =>
                this.clientService.getAllClients(page, pageSize).pipe(
                    map(res => {
                        if (res.success) {
                        // const clients = res.data.data.map((client: any) => ({
                        //     ...client,
                        //     userName: client.username
                        // }));
                        return ClientActions.loadClientsSuccess({ 
                          clients: res.data.data,
                          pagination: res.data.pagination
                        });
                    } else {
                        return ClientActions.loadClientsFailure({ error: res.errors || res.message });
                    }
                    }),
                    catchError(error => of(ClientActions.loadClientsFailure({ error })))
                )
            )
        )
    );

  createClient$ = createEffect(() =>
    this.actions$.pipe(
      ofType(ClientActions.addClient),
      mergeMap(({ client }) =>
        this.clientService.createClient(client).pipe(
          map(res => {
            if (res.success) {
              return ClientActions.addClientSuccess({ client: res.data });
            } else {
              return ClientActions.addClientFailure({ error: res.errors || res.message });
            }
          }),
          catchError(error => of(ClientActions.addClientFailure({ error })))
        )
      )
    )
  );

  updateClient$ = createEffect(() =>
      this.actions$.pipe(
        ofType(ClientActions.updateClient),
        mergeMap(({ clientId, client }) =>
          this.clientService.updateClient(clientId, client).pipe(
            map(res => {
              if (res.success) {
                console.log('Update Client Response:', res.data);
                return ClientActions.updateClientSuccess({ client: res.data });
              } else {
                return ClientActions.updateClientFailure({ error: res.errors || res.message });
              }
            }),
            catchError(error => of(ClientActions.updateClientFailure({ error })))
          )
        )
      )
    );

    deleteClient$ = createEffect(() =>
      this.actions$.pipe(
        ofType(ClientActions.deleteClient),
        mergeMap(({ clientId }) =>
          this.clientService.deleteClient(clientId).pipe(
            map(res => {
              if (res.success) {
                return ClientActions.deleteClientSuccess({ client: res.data });
              } else {
                return ClientActions.deleteClientFailure({ error: res.errors || res.message });
              }
            }),
            catchError(error => of(ClientActions.deleteClientFailure({ error })))
          )
        )
      )
    );
}