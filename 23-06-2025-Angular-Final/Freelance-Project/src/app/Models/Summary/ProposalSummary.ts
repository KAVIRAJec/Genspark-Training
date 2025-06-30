export class ProposalSummary {
    public id: string;
    public freelancerId: string;
    public proposedAmount: number;
    public proposedDuration: string | null;
    public isActive: boolean;

    constructor() {
        this.id = '';
        this.freelancerId = '';
        this.proposedAmount = 0;
        this.proposedDuration = null;
        this.isActive = true;
    }
}
