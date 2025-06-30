import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { environment } from "../../environments/environment";
import { ApiResponse } from "../Models/ApiResponse.model";
import { ChatMessageModel } from "../Models/ChatMessage.model";
import { PaginationModel } from "../Models/PaginationModel";
import { ChatRoomModel } from "../Models/ChatRoom.model";

@Injectable({providedIn: "root"})
export class ChatService{
    private baseUrl: string;
    
    constructor(private http: HttpClient) {
        this.baseUrl = environment.baseUrl;
    }

    sendMessage(content: string, chatRoomId: string, senderId: string) {
        return this.http.post<ApiResponse<ChatMessageModel>>(`${this.baseUrl}/Chat/SendMessage`, { content, chatRoomId, senderId });
    }
    updateMessage(messageId: string, content: string) {
        return this.http.put<ApiResponse<ChatMessageModel>>(`${this.baseUrl}/Chat/UpdateMessage/${messageId}`, { content });
    }
    markMessageAsRead(messageId: string, chatRoomId: string, senderId: string) {
        return this.http.put<ApiResponse<ChatMessageModel>>(`${this.baseUrl}/Chat/SetMessageRead/${messageId}`, { chatRoomId, senderId });
    }
    deleteMessage(messageId: string, chatRoomId: string) {
        return this.http.delete<ApiResponse<{ success: boolean }>>(`${this.baseUrl}/Chat/DeleteMessage/${messageId}/ChatRoom/${chatRoomId}`);
    }
    getMessagesByChatRoomId(chatRoomId: string, page: number = 1, pageSize: number = 10) {
        return this.http.get<ApiResponse<{ data: ChatMessageModel[]; pagination: PaginationModel }>>(`${this.baseUrl}/Chat/Messages/${chatRoomId}?page=${page}&pageSize=${pageSize}`);
    }
    getChatRoomByProjectId(projectId: string) {
        return this.http.get<ApiResponse<ChatRoomModel>>(`${this.baseUrl}/Chat/RoomByProject/${projectId}`);
    }
    getChatRoomsByUserId(userId: string, page: number = 1, pageSize: number = 10) {
        return this.http.get<ApiResponse<{ data: ChatRoomModel[]; pagination: PaginationModel }>>(`${this.baseUrl}/Chat/RoomsByUser/${userId}?page=${page}&pageSize=${pageSize}`);
    }
}