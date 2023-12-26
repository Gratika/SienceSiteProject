
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
    user: IUser;
    answer:string;
}
export interface IAuthResponse {
    message:ILoginResponse;
}

export interface ISignUpResponse {
    status: string;
    message: string;
}

export interface IUserResponse {
    user: IUser|null;

}
export interface IRole{
    id:string;
    role_name:string;
    modified_date: Date|number;
}
export interface IPeople{
    id:string;
    surname:string;
    name:string;
    birthday:Date;
    path_bucket:string;
    date_create: Date|number;
    modified_date: Date|number;
}

export interface IUser{
    id:string;
    login:string;
    password:string;
    email:string;
    access_token:string;
    refresh_token:string;
    date_create: Date|number;
    modified_date: Date|number;
    role_id:number;
    email_is_checked:number;
    people_id: string;
    people_:IPeople|null;
}
export interface IArticle {
    id: string;
    DOI: string|null;
    author_id: string|null;
    author_: IPeople|null;
    title: string;
    tag: string;
    text: string|null;
    views: number;
    date_create: Date|null;
    modified_date: Date|null;
    theory_id: string;
    Scientific_theories:IScientificTheory|null;
    path_file: string;
    tagItems:Array<string>;

}
export interface IComment {
    id: string;
    parent_id: string;
    user_id: string;
    article_id: string;
    text: string;
    date_create: Date|number;
    modified_date: Date|number;
}
export interface  IEmotion{
    id: string;
    Name: string;
    Emoji: string
}
export interface IReaction
{
    id: string;
    user_id: string;
    article_id: string;
    reaction_id: string;
    date_create: Date|number;
    modified_date: Date|number;
}

export interface IScience {
    id: string;
    name: string;
    note: string;
}
export interface IScientificTheory {
    id: string;
    science_id: string;
    science_: IScience|null;
    name: string;
    note: string;

}
export interface ISelectedArticle{
    id: string;
    user_id: string;
    user_: IUser|null;
    article_id: string;
    article_:IArticle|null;
    Date_view: Date|null;

}