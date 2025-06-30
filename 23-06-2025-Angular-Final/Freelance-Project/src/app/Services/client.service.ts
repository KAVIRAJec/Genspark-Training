import { Injectable } from "@angular/core";
import { environment } from "../../environments/environment";
import { HttpClient } from "@angular/common/http";
import { ClientModel, CreateClientModel, UpdateClientModel } from "../Models/Client.model";
import { ApiResponse } from "../Models/ApiResponse.model";
import { PaginationModel } from "../Models/PaginationModel";

@Injectable({providedIn: "root"})
export class ClientService{
    private baseUrl: string;
    
    constructor(private http: HttpClient) {
        this.baseUrl = environment.baseUrl;
    }

    createClient(client: CreateClientModel) {
        return this.http.post<ApiResponse<ClientModel>>(`${this.baseUrl}/client/create`, client);
    }
    getClientById(clientId: string) {
        return this.http.get<ApiResponse<ClientModel>>(`${this.baseUrl}/client/${clientId}`);
    }
    getAllClients(page: number = 1, pageSize: number = 10) {
        return this.http.get<ApiResponse<{ data: ClientModel[]; pagination: PaginationModel }>>(
            `${this.baseUrl}/client?page=${page}&pageSize=${pageSize}`);
    }
    updateClient(clientId: string, client: UpdateClientModel) {
        return this.http.put<ApiResponse<ClientModel>>(`${this.baseUrl}/client/${clientId}`, client);
    }
    deleteClient(clientId: string) {
        return this.http.delete<ApiResponse<ClientModel>>(`${this.baseUrl}/client/${clientId}`);
    }
}