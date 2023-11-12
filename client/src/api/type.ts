
export interface GenericResponse {
    //status: string;
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
    answer:string;
    user: IUser;
}

export interface ISignUpResponse {
    status: string;
    message: string;
}

export interface IUserResponse {
    user: IUser|null;

}
export interface IRole{
    id:number;
    role_name:string;
    modified_date: Date;
}
export interface IUser{
    id:number;
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
    role_id:number;
    roles:Array<IRole>;
    email_is_checked:number;
}
export interface IArticle {
    id: number|undefined;
    DOI: string;
    author_id: number|undefined;
    title: string;
    tag: string;
    text: string;
    views: number;
    date_create: Date;
    modified_date: Date;
    theory_id: number|undefined;
    path_file: string;
    author_: IUser|null;
}
export interface IComment {
    id: number;
    parent_id: number;
    user_id: number;
    article_id: number;
    text: string;
    date_create: Date;
    modified_date: Date;
}
export interface  IEmotion{
    id: number;
    Name: string;
    Emoji: string
}
export interface IReaction
{
    id: number;
    user_id: number;
    article_id: number;
    reaction_id: number;
    date_create: Date;
    modified_date: Date;
}

export interface IScience {
    id: number;
    name: string;
    note: string;
}
export interface IScientificTheory {
    id: number;
    science_id: number;
    name: string;
    note: string;

}
export interface ISelectedArticle{
    id: number|null;
    user_id: number;
    article_id: number;
    Date_view: Date|null;

}