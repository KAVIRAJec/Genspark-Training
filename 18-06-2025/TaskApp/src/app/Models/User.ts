export interface UserLoginModel {
    username: string | null;
    password: string | null;
}

export interface UserModel {
    id: number;
    firstName: string;
    lastName: string;
    email: string;
    password: string;
    phone: string;
    gender: string;
    age: number;
    address: string;
    city: string;
    state: string;
    postalCode: string;
    country: string;
    role: string;
}