
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
    people:IPeople;

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
    modified_date: string|null;
}
export interface IPeople{
    id:string;
    surname:string;
    name:string;
    birthday:string|null;
    path_bucket:string;
    date_create: string|null;
    modified_date: string|null;
}

export interface IUser{
    id:string;
    login:string;
    password:string;
    email:string;
    access_token:string;
    refresh_token:string;
    date_create: string|null;
    modified_date: string|null;
    role_id:number;
    email_is_checked:number;
    people_id: string;
    people_:IPeople|null;
}
export interface IArticle {
    id: string;
    doi: string|null;
    author_id: string|null;
    author_: IPeople|null;
    title: string;
    tag: string|null;
    text: string;
    views: number;
    date_created: string|null;
    modified_date: string|null;
    theory_id: string;
    theory_:IScientificTheory|null;
    path_file: string;
    tagItems:Array<string>;
    reaction: IEmotion|null;
    countLike:number;
    selected:boolean;
    isActive:boolean;
}
export interface IComment {
    id: string;
    parent_id: string;
    user_id: string;
    article_id: string;
    text: string;
    date_create: string|null;
    modified_date: string|null;
}
export interface  IEmotion{
    id: string;
    Name: string;
    Emoji: string
    isSelected:boolean;
}
export interface IReaction
{
    id: string|null;
    people_id: string|null;
    article_id: string;
    reaction_id: string;
    date_create: string|null;
    modified_date: string|null;
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
    people_id: string;
    people_: IPeople;
    article_id: string;
    article_:IArticle;
    Date_view: string|null;

}
export interface ISearchResponse<T>{
    articles: Array<T>;
    allPages: number;
    sciences?:IScience;
    theories?:Array<IScientificTheory>

}
export interface IArticleResponse{
   // articles: Array<IFullArticle<IArticle>>;
    articleId: string;
    response: string;
}

export interface IFullArticle<T>{
    articles: T;
    emotion: IEmotion;
    countReactions:number;
    selected:boolean;
}
