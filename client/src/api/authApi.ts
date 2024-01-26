import axios from "axios";
import { AxiosError } from "axios"
import type {
    GenericResponse,
    IAuthResponse,
    ILoginInput,
    ILoginResponse,
    ISignUpInput,
    IUserResponse,
} from './type';
import MyLocalStorage from "@/services/myLocalStorage";
import {ServerBadRequestError} from "@/api/appException";


const BASE_URL:string = '/api/';

//Створюємо новий екземпляр axios, задавши у ньому потрібну нам конфігурацію (див https://axios-http.com/docs/req_config)
//базовий URL та withCredentials: true - для відправки файлів cookie разом з запитами
const authApi = axios.create({
    baseURL: BASE_URL,
    headers: {'X-Requested-With': 'XMLHttpRequest',
        'Access-Control-Allow-Credentials': true,
        'Content-Type': 'application/json',
        'Access-Control-Allow-Origin': "*"},
    withCredentials: true,
});


//використовуючи методи створеного екземпляру axios опишемо ряд функцій

//функція для оновлення токену, у випадку якщо його термін сплив
export const refreshAccessTokenFn = async () => {
    const response = await authApi.get<ILoginResponse>('auth/refresh');
    if(response.status==200)return response.data;
    else {
        console.log("BadRequest=",response.data)
        throw new ServerBadRequestError(response.status,JSON.stringify(response.data))
    }
};
//перехоплювач (interceptors) відповідей чи запитів axios (до того як вони будуть опрацьована then або catch)
//автоматично виконується до запитів та після отримання відповіді
authApi.interceptors.request.use(
    (config) => {
        // Проверяем, есть ли токен в localStorage
        const token = MyLocalStorage.getItem('token');
        //console.log("token:");
       // console.log(token);
        if (token) {
            // Если есть, добавляем его в заголовок Authorization
            config.headers['Authorization'] = `Bearer ${token}`;
        }
        // Перевірка, чи дані є об'єктом FormData (для відправки файлів)
        if (config.data instanceof FormData) {
            config.headers['Content-Type'] = 'multipart/form-data';
        }
        return config;
    },
    (error) => {
        return Promise.reject(error);
    }
);
/*authApi.interceptors.response.use(
    //якщо отримали відповідь, то повертаємо її
    (response) => {
        return response;
    },
    //якщо сервер відповів помилкою
    async (error) => {
        console.log(error);
        const originalRequest = error.config;
        let errMessage = ''
        if (error.response?.data) {
          errMessage = error.response.data as string
        }else{
          errMessage = error.message;
        }
        if (errMessage.includes('not logged in') && !originalRequest._retry) { //яка містить 'not logged in'
            originalRequest._retry = true;
            await refreshAccessTokenFn(); //викликаємо функцію оновлення токена
            return authApi(originalRequest);
        }
        return Promise.reject(error);
    }
);*/

//створюємо(реєструємо нового користувача)
export const signUpUserFn = async (user: ISignUpInput) => {

    const response = await authApi.post<GenericResponse>('/User/CreateUser', user);
    response.data;
};

//авторизація, вхід в систему
export const loginUserFn = async (user: ILoginInput)=> {
            const response = await authApi.post<ILoginResponse>('auth/AuthUser', user);
            return response.data;
};


export const verifyEmailFn = async (verificationCode: string) => {
    const response = await authApi.get<string>('email/checkcode', {
        params:{
            code:verificationCode
        }
    });
    return response.data;
};
export const getRepeatCodeFn = async (email: string) => {
    const response = await authApi.get<string>('email/SentCode', {
        params: {
            email : email
        }
    });
    return response.data;
};

//вихід з системи
export const logoutUserFn = async () => {
    const response = await authApi.get<GenericResponse>('auth/logout');
    return response.data;

};

// Универсальная функция для отправки запроса
export const sendRequest = async <T>(
    method: string,
    url: string,
    params?: any, // параметри для GET-запиту
    data?: any): Promise<T> => {

    const response = await authApi.request<T>({
        method,
        url,
        params,
        data,
    });
    //console.log('response.status',response.status);
    //console.log('response.statusText',response.statusText);
    return response.data;

};
export function showErrorMessage ( error:any):string{
    if('name' in error && error.name==='AxiosError'){
        if(error.response?.status===502){
            return 'На сервері виникла тимчасова помилка. Спробуйте пізніше'
        }
        if (error?.response && error?.response.data ) {
            if (typeof  error.response.data ==='string')
                return error.response.data;
            else return JSON.stringify(error.response.data)
        } else {
            if(error?.message){
                return error.message;
            }
        }
    }
    if('message' in error) return error.message;
    return ''
}


