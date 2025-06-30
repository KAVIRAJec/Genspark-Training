import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { environment } from "../../environments/environment";
import { ApiResponse } from "../Models/ApiResponse.model";
import { CreateProjectModel, ProjectModel, UpdateProjectModel } from "../Models/Project.model";
import { PaginationModel } from "../Models/PaginationModel";

@Injectable({providedIn: "root"})
export class ProjectService {
    private baseUrl: string;

    constructor(private http: HttpClient) {
        this.baseUrl = environment.baseUrl;
    }

    createProject(project: CreateProjectModel) {
        return this.http.post<ApiResponse<ProjectModel>>(`${this.baseUrl}/project/create`, project);
    }

    getProjectById(projectId: string) {
        return this.http.get<ApiResponse<ProjectModel>>(`${this.baseUrl}/project/${projectId}`);
    }

    getProjectsByClientId(
        clientId: string, 
        page: number = 1, 
        pageSize: number = 10,
        search?: string,
        sortBy?: string
    ) {
        let params = `?page=${page}&pageSize=${pageSize}`;
        if (search) params += `&search=${encodeURIComponent(search)}`;
        if (sortBy) params += `&sortBy=${encodeURIComponent(sortBy)}`;
        return this.http.get<ApiResponse<{ data: ProjectModel[]; pagination: PaginationModel }>>(
            `${this.baseUrl}/project/client/${clientId}${params}`
        );
    }

    getProjectsByFreelancerId(
        freelancerId: string, 
        page: number = 1, 
        pageSize: number = 10,
        search?: string,
        sortBy?: string
    ) {
        let params = `?page=${page}&pageSize=${pageSize}`;
        if (search) params += `&search=${encodeURIComponent(search)}`;
        if (sortBy) params += `&sortBy=${encodeURIComponent(sortBy)}`;
        return this.http.get<ApiResponse<{ data: ProjectModel[]; pagination: PaginationModel }>>(
            `${this.baseUrl}/project/freelancer/${freelancerId}${params}`
        );
    }
    getAllProjects(
        page: number = 1,
        pageSize: number = 10,
        search?: string,
        sortBy?: string
    ) {
        let params = `?page=${page}&pageSize=${pageSize}`;
        if (search) params += `&search=${encodeURIComponent(search)}`;
        if (sortBy) params += `&sortBy=${encodeURIComponent(sortBy)}`;
        return this.http.get<ApiResponse<{ data: ProjectModel[]; pagination: PaginationModel }>>(
            `${this.baseUrl}/project${params}`
        );
    }
    updateProject(projectId: string, project: UpdateProjectModel) {
        return this.http.put<ApiResponse<ProjectModel>>(`${this.baseUrl}/project/${projectId}`, project);
    }
    deleteProject(projectId: string) {
        return this.http.delete<ApiResponse<ProjectModel>>(`${this.baseUrl}/project/${projectId}`);
    }
}