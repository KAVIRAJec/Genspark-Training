import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { environment } from "../../environments/environment";
import { ProposalModel, CreateProposalModel, UpdateProposalModel } from "../Models/Proposal.model";
import { ApiResponse } from "../Models/ApiResponse.model";
import { PaginationModel } from "../Models/PaginationModel";

@Injectable({providedIn: "root"})
export class ProposalService{
    private baseUrl: string;
    
    constructor(private http: HttpClient) {
        this.baseUrl = environment.baseUrl;
    }

    CreateProposal(proposal : CreateProposalModel) {
        return this.http.post<ApiResponse<ProposalModel>>(`${this.baseUrl}/proposal/create`, proposal);
    }
    GetProposalById(proposalId: string) {
        return this.http.get<ApiResponse<ProposalModel>>(`${this.baseUrl}/proposal/${proposalId}`);
    }
    GetProposalsByFreelancerId(freelancerId: string, page: number = 1, pageSize: number = 10, search?: string, sortBy?: string) {
        let params = `?page=${page}&pageSize=${pageSize}`;
        if (search) params += `&search=${encodeURIComponent(search)}`;
        if (sortBy) params += `&sortBy=${encodeURIComponent(sortBy)}`;
        return this.http.get<ApiResponse<{ data: ProposalModel[]; pagination: PaginationModel }>>(
            `${this.baseUrl}/proposal/freelancer/${freelancerId}${params}`);
    }
    GetProposalsByClientId(clientId: string, page: number = 1, pageSize: number = 10, search?: string, sortBy?: string) {
        let params = `?page=${page}&pageSize=${pageSize}`;
        if (search) params += `&search=${encodeURIComponent(search)}`;
        if (sortBy) params += `&sortBy=${encodeURIComponent(sortBy)}`;
        return this.http.get<ApiResponse<{ data: ProposalModel[]; pagination: PaginationModel }>>(
            `${this.baseUrl}/proposal/client/${clientId}${params}`);
    }
    GetAllProposals(page: number = 1, pageSize: number = 10, search?: string, sortBy?: string) {
        let params = `?page=${page}&pageSize=${pageSize}`;
        if (search) params += `&search=${encodeURIComponent(search)}`;
        if (sortBy) params += `&sortBy=${encodeURIComponent(sortBy)}`;
        return this.http.get<ApiResponse<{ data: ProposalModel[]; pagination: PaginationModel }>>(
            `${this.baseUrl}/proposal${params}`);
    }
    UpdateProposal(proposalId: string, proposal: UpdateProposalModel) {
        return this.http.put<ApiResponse<ProposalModel>>(`${this.baseUrl}/proposal/${proposalId}`, proposal);
    }
    DeleteProposal(proposalId: string) {
        return this.http.delete<ApiResponse<ProposalModel>>(`${this.baseUrl}/proposal/${proposalId}`);
    }
}