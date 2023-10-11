import { defineStore } from 'pinia';


import MyLocalStorage from "@/services/myLocalStorage";
import type {ILoginInput, IUser, ISignUpInput, ISignUpResponse} from "@/api/type";
import { getUserFn, loginUserFn, logoutUserFn, /*showErrorMessage, */signUpUserFn } from '@/api/authApi'
import { createToast } from 'mosha-vue-toastify';
import router from '@/router'




export type AuthStoreState ={
    authUser:IUser|null;
    token:string|'';
    username:string;
    isLogin:boolean;
}

export const useAuthStore = defineStore({
    id: 'auth',
    state: ():AuthStoreState => ({
        // initialize state from local storage to enable user to stay logged in
        authUser: null,//JSON.parse(MyLocalStorage.getItem('user')) ,
        token:'',// MyLocalStorage.getItem('token') || '',
        isLogin:false, //MyLocalStorage.getItem('isLogin'),
        username:'',// MyLocalStorage.getItem('username'),

    } ),

    actions: {

        setAuthUser(user: IUser | null) {
            this.authUser = user;
        },
        onRegistration(user:ISignUpInput){
            signUpUserFn(user).then(
                res=>{
                    createToast(res, {
                        position: 'top-right',
                    });
                    router.push('/login');
                }
            ).catch(error => {
                console.log(error);
                //showErrorMessage(error);
            })
        },
        onLogin(user:ILoginInput){
            console.log("in auth.login")
            loginUserFn(user).then(
                res=>{
                    this.token = res.token;
                    console.log("token: ",res.token);
                    this.username = res.login;
                    this.isLogin =true;
                    MyLocalStorage.setItem('token',this.token);
                    MyLocalStorage.setItem('username',this.username);
                    MyLocalStorage.setItem('isLogin',this.isLogin);
                    this.getAuthUser();
                    router.push('/testList');

                }
            ).catch(error => {
                //showErrorMessage(error);
            })
        },
        onLogout(){
            logoutUserFn().then(
                res=>{
                    console.log(res.message);
                    this.token = '';
                    this.username = '';
                    this.isLogin =false;
                    MyLocalStorage.setItem('token',this.token);
                    MyLocalStorage.setItem('username',this.username);
                    MyLocalStorage.setItem('isLogin',this.isLogin);
                }
            )

        },
        getAuthUser(){
            getUserFn().then(
                res=>{
                    console.log(res);
                    this.setAuthUser(res.user)
                    if(this.authUser!=null){
                        MyLocalStorage.setItem('user',this.authUser.toString());
                        MyLocalStorage.setItem('userId',this.authUser.id);
                    }
                    

                }
            )
        },
    }
});