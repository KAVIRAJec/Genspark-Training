import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { environment } from "../../environments/environment";
import { ProposalModel } from "../Models/Proposal.model";
import { ProjectModel } from "../Models/Project.model";
import { ApiResponse } from "../Models/ApiResponse.model";
import { PaginationModel } from "../Models/PaginationModel";

@Injectable({providedIn: "root"})
export class ProjectProposalService{
    private baseUrl: string;
    
    constructor(private http: HttpClient) {
        this.baseUrl = environment.baseUrl;
    }

    GetProposalsByProjectId(projectId: string, page: number = 1, pageSize: number = 10, search?: string, sortBy?: string) {
        let params = `?page=${page}&pageSize=${pageSize}`;
        if (search) params += `&search=${encodeURIComponent(search)}`;
        if (sortBy) params += `&sortBy=${encodeURIComponent(sortBy)}`;
        return this.http.get<ApiResponse<{ data: ProposalModel[]; pagination: PaginationModel }>>(
            `${this.baseUrl}/ProjectProposal/ProjectId/${projectId}${params}`);
    }
    AcceptProposal(projectId: string, proposalId: string) {
        return this.http.post<ApiResponse<ProjectModel>>(`${this.baseUrl}/ProjectProposal/Accept`, { projectId, proposalId });
    }
    RejectProposal(projectId: string, proposalId: string) {
        return this.http.post<ApiResponse<ProposalModel>>(`${this.baseUrl}/ProjectProposal/Reject`, { projectId, proposalId });
    }
    CancelProject(projectId: string) {
        return this.http.post<ApiResponse<ProjectModel>>(`${this.baseUrl}/ProjectProposal/Cancel`, { projectId });
    } 
    CompleteProject(projectId: string) {
        return this.http.post<ApiResponse<ProjectModel>>(`${this.baseUrl}/ProjectProposal/Complete`, { projectId });
    }
}