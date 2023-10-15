
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
export interface IRole{
    id:bigint;
    role_name:string;
    modified_date: Date;
}
export interface IUser{
    id:bigint;
    login:string;
    password:string;
    email:string;
    firstname:string;
    name:string;
    access_token:string;
    refresh_token:string;
    birthday:Date;
    date_create: Date;
    modified_date: Date;
    role_id:bigint;
    roles:Array<IRole>;
}
export interface IArticles {
    id: bigint;
    DOI: string;
    author_id: bigint;
    title: string;
    tag: string;
    text: string;
    views: number;
    date_create: Date;
    modified_date: Date;
    theory_id: bigint;
    path_file: string;
    author: IUser;
}
export interface IComments {
    id: bigint;
    parent_id: bigint;
    user_id: bigint;
    article_id: bigint;
    text: string;
    date_create: Date;
    modified_date: Date;
}
export interface  IEmotions{
    id: bigint;
    Name: string;
    Emoji: string
}
export interface IReactions
{
    id: bigint;
    user_id: bigint;
    article_id: bigint;
    reaction_id: bigint;
    date_create: Date;
    modified_date: Date;
}

export interface ISciences {
    id: bigint;
    name: string;
    note: string;
}
export interface IScientificTheories {
    id: bigint;
    science_id: bigint;
    name: string;
    note: string;

}
export interface ISelectedArticles{
    id: bigint;
    user_id: bigint;
    article_id: bigint;
    Date_view: Date;

}