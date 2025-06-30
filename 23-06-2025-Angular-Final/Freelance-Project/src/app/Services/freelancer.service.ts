import { Injectable } from "@angular/core";
import { environment } from "../../environments/environment";
import { HttpClient } from "@angular/common/http";
import { ApiResponse } from "../Models/ApiResponse.model";
import { CreateFreelancerModel, FreelancerModel, UpdateFreelancerModel } from "../Models/Freelancer.model";
import { PaginationModel } from "../Models/PaginationModel";
import { SkillModel } from "../Models/Skill.model";

@Injectable({providedIn: "root"})
export class FreelancerService {
    private baseUrl: string;
    
    constructor(private http: HttpClient) {
        this.baseUrl = environment.baseUrl;
    }

    createFreelancer(freelancer: CreateFreelancerModel) {
        return this.http.post<ApiResponse<FreelancerModel>>(`${this.baseUrl}/freelancer/create`, freelancer);
    }

    getFreelancerById(freelancerId: string) {
        return this.http.get<ApiResponse<FreelancerModel>>(`${this.baseUrl}/freelancer/${freelancerId}`);
    }
    getAllFreelancers(
        page: number = 1,
        pageSize: number = 10,
        search?: string,
        sortBy?: string
    ) {
        let params = `?page=${page}&pageSize=${pageSize}`;
        if (search) params += `&search=${encodeURIComponent(search)}`;
        if (sortBy) params += `&sortBy=${encodeURIComponent(sortBy)}`;
        return this.http.get<ApiResponse<{ data: FreelancerModel[]; pagination: PaginationModel }>>(
            `${this.baseUrl}/freelancer${params}`
        );
    }
    updateFreelancer(freelancerId: string, freelancer: UpdateFreelancerModel) {
        return this.http.put<ApiResponse<FreelancerModel>>(`${this.baseUrl}/freelancer/${freelancerId}`, freelancer);
    }
    deleteFreelancer(freelancerId: string) {
        return this.http.delete<ApiResponse<FreelancerModel>>(`${this.baseUrl}/freelancer/${freelancerId}`);
    }
}