export class ProjectSummary {
    public id: string;
    public title: string;
    public status: string;
    public isActive: boolean;
    public clientId: string;
    public freelancerId: string | null;

    constructor() {
        this.id = '';
        this.title = '';
        this.status = '';
        this.isActive = true;
        this.clientId = '';
        this.freelancerId = null;
    }
}