import { SkillModel } from "./Skill.model";
import { ProposalSummary } from "./Summary/ProposalSummary";

export class ProjectModel {
    public id: string;
    public title: string;
    public description: string;
    public budget: number;
    public duration: string;
    public isActive: boolean;
    public status: string;
    public createdAt: Date | null;
    public updatedAt: Date | null;
    public deletedAt: Date | null;
    public clientId: string;
    public freelancerId: string | null;
    public proposals: ProposalSummary[];
    public requiredSkills: SkillModel[];

    constructor() {
        this.id = '';
        this.title = '';
        this.description = '';
        this.budget = 0;
        this.duration = '';
        this.isActive = true;
        this.status = '';
        this.createdAt = null;
        this.updatedAt = null;
        this.deletedAt = null;
        this.clientId = '';
        this.freelancerId = null;
        this.requiredSkills = [];
        this.proposals = [];
    }
}

export class CreateProjectModel {
    public title: string;
    public description: string;
    public budget: number;
    public duration?: string;
    public clientId: string;
    public requiredSkills: SkillModel[];

    constructor() {
        this.title = '';
        this.description = '';
        this.budget = 0;
        this.duration = '';
        this.clientId = '';
        this.requiredSkills = [];
    }
}

export class UpdateProjectModel {
    public title?: string;
    public description?: string;
    public budget?: number;
    public duration?: string;
    public requiredSkills?: SkillModel[];

    constructor() {
        this.title = '';
        this.description = '';
        this.budget = 0;
        this.duration = '';
        this.requiredSkills = [];
    }
}