export class ClientSummary {
    public id: string;
    public userName: string;
    public email: string;
    public isActive: boolean;

    constructor() {
        this.id = '';
        this.userName = '';
        this.email = '';
        this.isActive = true;
    }
}
