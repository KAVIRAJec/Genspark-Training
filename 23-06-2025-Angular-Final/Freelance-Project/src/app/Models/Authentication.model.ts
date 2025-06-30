export class AuthenticationModel {
    public email: string;
    public role: string;
    public token: string;
    public refreshToken: string;
    public refreshExpiresAt: Date | null;

    constructor() {
        this.email = '';
        this.role = '';
        this.token = '';
        this.refreshToken = '';
        this.refreshExpiresAt = null;
    }
}

export class LoginModel {
    public email: string;
    public password: string;

    constructor() {
        this.email = '';
        this.password = '';
    }
}