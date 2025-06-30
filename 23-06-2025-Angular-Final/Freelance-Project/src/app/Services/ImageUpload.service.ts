import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { environment } from "../../environments/environment";
import { ApiResponse } from "../Models/ApiResponse.model";

@Injectable({providedIn: "root"})
export class ImageUploadService {
    private baseUrl: string;
    
    constructor(private http: HttpClient) {
        this.baseUrl = environment.baseUrl;
    }
    
    UploadImage(file: File) {
        const formData = new FormData();
        formData.append("image", file);
        return this.http.post<ApiResponse<{ imageUrl: string }>>(`${this.baseUrl}/ImageUpload/upload`, formData);
    }

    DeleteImage(imageUrl: string) {
        return this.http.delete<ApiResponse<boolean>>(`${this.baseUrl}/ImageUpload/delete`, { params: { imageUrl } });
    }
}