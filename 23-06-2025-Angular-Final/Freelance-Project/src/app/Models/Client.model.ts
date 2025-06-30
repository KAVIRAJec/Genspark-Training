import { ProjectSummary } from "./Summary/ProjectSummary";

export class ClientModel {
    public id: string; // Use string for guid
    public profileUrl: string;
    public username: string;
    public email: string;
    public companyName: string;
    public location: string;
    public isActive: boolean;
    public createdAt: Date | null;
    public updatedAt: Date | null;
    public deletedAt: Date | null;
    public projects: ProjectSummary[];

    constructor() {
        this.id = '';
        this.profileUrl = '';
        this.username = '';
        this.email = '';
        this.companyName = '';
        this.location = '';
        this.isActive = true;
        this.createdAt = null;
        this.updatedAt = null;
        this.deletedAt = null;
        this.projects = [];
    }
}

export class CreateClientModel {
    public profileUrl?: string;
    public username: string;
    public email: string;
    public companyName?: string;
    public location?: string;
    public password: string;
    public confirmPassword: string;

    constructor() {
        this.profileUrl = '';
        this.username = '';
        this.email = '';
        this.companyName = '';
        this.location = '';
        this.password = '';
        this.confirmPassword = '';
    }
}

export class UpdateClientModel {
    public profileUrl?: string;
    public username?: string;
    public companyName?: string;
    public location?: string;

    constructor() {
        this.profileUrl = '';
        this.username = '';
        this.companyName = '';
        this.location = '';
    }
}