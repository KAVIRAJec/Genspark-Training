import { ChatMessageModel } from "./ChatMessage.model";

export class ChatRoomModel {
    public id: string;
    public name: string;
    public createdAt: Date | null;
    public updatedAt: Date | null;
    public deletedAt: Date | null;
    public isActive: boolean;
    public clientId: string;
    public freelancerId: string;
    public projectId: string;
    public clientName: string;
    public freelancerName: string;
    public messages: ChatMessageModel[];

    constructor() {
        this.id = '';
        this.name = '';
        this.createdAt = null;
        this.updatedAt = null;
        this.deletedAt = null;
        this.isActive = true;
        this.clientId = '';
        this.freelancerId = '';
        this.projectId = '';
        this.clientName = '';
        this.freelancerName = '';
        this.messages = [];
    }
}

export class CreateChatRoomModel {
    public name: string;
    public clientId: string;
    public freelancerId: string;
    public projectId: string;

    constructor() {
        this.name = '';
        this.clientId = '';
        this.freelancerId = '';
        this.projectId = '';
    }
}