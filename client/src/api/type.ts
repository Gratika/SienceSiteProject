export interface IRole{
    id:bigint;
    name:string;
}
export interface IUser{
    id:bigint;
    login:string;
    password:string;
    email:string;
    isActive:boolean;
    name:string;
    surname:string;
    birthday:Date;
    created: Date;
    updated: Date;
    roles:Array<IRole>;

}
export interface GenericResponse {
    status: string;
    message: string;
}

export interface ILoginInput {
    email: string;
    password: string;
}

export interface ISignUpInput {
    email: string;
    password: string;
    passwordConfirm: string;
}

export interface ILoginResponse {
    login:string;
    token: string;
}

export interface ISignUpResponse {
    status: string;
    message: string;
}

export interface IUserResponse {
    user: IUser|null;

}

