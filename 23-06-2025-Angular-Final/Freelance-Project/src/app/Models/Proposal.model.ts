import { FreelancerSummary } from "./Summary/FreelancerSummary";
import { ProjectSummary } from "./Summary/ProjectSummary";

export class ProposalModel {
    public id: string;
    public description: string;
    public proposedAmount: number;
    public proposedDuration: string;
    public isActive: boolean;
    public isAccepted: boolean;
    public isRejected: boolean;
    public createdAt: Date | null;
    public updatedAt: Date | null;
    public deletedAt: Date | null;
    public freelancer: FreelancerSummary;
    public project: ProjectSummary;

    constructor() {
        this.id = '';
        this.description = '';
        this.proposedAmount = 0;
        this.proposedDuration = '';
        this.isActive = true;
        this.isAccepted = false;
        this.isRejected = false;
        this.createdAt = null;
        this.updatedAt = null;
        this.deletedAt = null;
        this.freelancer = new FreelancerSummary();
        this.project = new ProjectSummary();
    }
}

export class CreateProposalModel{
    public description: string;
    public proposedAmount: number;
    public proposedDuration?: string;
    public freelancerId: string;
    public projectId: string;

    constructor() {
        this.description = '';
        this.proposedAmount = 0;
        this.proposedDuration = '';
        this.freelancerId = '';
        this.projectId = '';
    }
}

export class UpdateProposalModel {
    public description?: string;
    public proposedAmount?: number;
    public proposedDuration?: string;

    constructor() {
        this.description = '';
        this.proposedAmount = 0;
        this.proposedDuration = '';
    }
}