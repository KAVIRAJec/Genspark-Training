export class UserModel {
    constructor(
        public id: number = 0,
        public username: string = "",
        public email: string = "",
        public firstName: string = "",
        public lastName: string = "",
        public gender: string = "",
        public image: string = ""
    ) { }

    toUser(data: any) {
        return {
            id: data.id,
            username: data.username,
            email: data.email,
            firstName: data.firstName,
            lastName: data.lastName,
            gender: data.gender,
            image: data.image
        };
    }
}

export class UserLoginModel {
    constructor(
        public username: string = "",
        public password: string = ""
    ) { }

    toUserLogin(data: any) {
        return {
            username: data.username,
            password: data.password
        };
    }
}