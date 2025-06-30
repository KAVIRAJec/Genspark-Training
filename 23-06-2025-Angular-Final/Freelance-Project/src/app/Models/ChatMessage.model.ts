export class ChatMessageModel {
    public id: string;
    public content: string;
    public sentAt: Date | null;
    public updatedAt: Date | null;
    public deletedAt: Date | null;
    public isRead: boolean;
    public isActive: boolean;
    public senderId: string;
    public chatRoomId: string;

    constructor() {
        this.id = '';
        this.content = '';
        this.sentAt = null;
        this.updatedAt = null;
        this.deletedAt = null;
        this.isRead = false;
        this.isActive = true;
        this.senderId = '';
        this.chatRoomId = '';
    }
}

export class CreateChatMessageModel {
    public content: string;
    public senderId: string;
    public chatRoomId: string;

    constructor() {
        this.content = '';
        this.senderId = '';
        this.chatRoomId = '';
    }
}

export class UpdateChatMessageModel {
    public content: string;

    constructor() {
        this.content = '';
    }
}