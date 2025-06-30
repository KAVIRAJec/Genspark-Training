import { SkillModel } from "./Skill.model";

export class FreelancerModel {
    public id: string;
    public profileUrl: string;
    public username: string;
    public email: string;
    public experienceYears: number;
    public hourlyRate: number;
    public location: string;
    public isActive: boolean;
    public createdAt: Date | null;
    public updatedAt: Date | null;
    public deletedAt: Date | null;
    public skills: SkillModel[];

    constructor() {
        this.id = '';
        this.profileUrl = '';
        this.username = '';
        this.email = '';
        this.experienceYears = 0;
        this.hourlyRate = 0;
        this.location = '';
        this.isActive = true;
        this.createdAt = null;
        this.updatedAt = null;
        this.deletedAt = null;
        this.skills = [];
    }
}

export class CreateFreelancerModel {
    public profileUrl?: string;
    public username: string;
    public email: string;
    public experienceYears: number;
    public hourlyRate: number;
    public location?: string;
    public password: string;
    public confirmPassword: string;
    public skills: SkillModel[];

    constructor() {
        this.profileUrl = '';
        this.username = '';
        this.email = '';
        this.experienceYears = 0;
        this.hourlyRate = 0;
        this.location = '';
        this.password = '';
        this.confirmPassword = '';
        this.skills = [];
    }
}

export class UpdateFreelancerModel {
    public profileUrl?: string;
    public username?: string;
    public experienceYears?: number;
    public hourlyRate?: number;
    public location?: string;
    public skills?: SkillModel[];

    constructor() {
        this.profileUrl = '';
        this.username = '';
        this.experienceYears = 0;
        this.hourlyRate = 0;
        this.location = '';
        this.skills = [];
    }
}