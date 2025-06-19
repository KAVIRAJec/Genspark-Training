export class User {
    constructor(
        public Username: string = '',
        public Email: string = '',
        public Password: string = '',
        public ConfirmPassword: string = '',
        public Role: string = ''
    ) { }
    ToUserModel(data: any): User {
        return new User(
            this.Username = data?.username,
            this.Email = data?.email,
            this.Password = data?.password,
            this.ConfirmPassword = data?.confirmPassword,
            this.Role = data?.role || 'User'
        );
    }
}