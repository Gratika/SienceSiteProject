import { defineStore } from 'pinia';


import MyLocalStorage from "@/services/myLocalStorage";
import type {ILoginInput, IUser, ISignUpInput} from "@/api/type";
import {
    getRepeatCodeFn,
    loginUserFn,
    logoutUserFn,
     /*showErrorMessage, */
    signUpUserFn,
    verifyEmailFn
} from '@/api/authApi'
import { createToast } from 'mosha-vue-toastify';
import router from '@/router'
import axios from "axios";




export type AuthStoreState ={
    authUser:IUser|null;
    token:string|'';
    username:string;
    isLogin:boolean;
    isLoading:boolean;
}

export const useAuthStore = defineStore({
    id: 'auth',
    state: ():AuthStoreState => ({
        // initialize state from local storage to enable user to stay logged in
        authUser: null,//JSON.parse(MyLocalStorage.getItem('user')) ,
        token:'',// MyLocalStorage.getItem('token') || '',
        isLogin:false, //MyLocalStorage.getItem('isLogin'),
        username:'',// MyLocalStorage.getItem('username'),
        isLoading:false,
    } ),
    getters:{
        getUserId():number|undefined{
            return this.authUser?.id;
        }
    },

    actions: {

        onRegistration(user:ISignUpInput){
            this.isLoading=true;
            signUpUserFn(user).then(
                res=>{
                    this.isLoading = false;
                    console.log('res from onRegistration: ',res)
                    createToast(res.message, {
                        position: 'top-right',
                    });
                    MyLocalStorage.setItem('email', user.email);
                   router.push('/verify_email');
                }
            ).catch(error => {
                this.isLoading = false;
                console.log('error from onRegistration: ',error);

                //showErrorMessage(error);
            })
        },
        onLogin(user:ILoginInput){
            loginUserFn(user).then(
                res=>{
                    createToast(res.answer, {
                        position: 'top-right',
                    });
                    this.token = res.user.access_token;
                    console.log("token: ",res.user.access_token);
                    this.username = res.user.login;
                    this.isLogin =true;
                    this.authUser = res.user;
                    MyLocalStorage.setItem('token',this.token);
                    MyLocalStorage.setItem('username',this.username);
                    MyLocalStorage.setItem('isLogin',this.isLogin);
                    if(this.authUser!=null){
                        MyLocalStorage.setItem('user',this.authUser.toString());
                        MyLocalStorage.setItem('userId',this.authUser.id);
                    }
                    if(res.user.email_is_checked ===0){
                        router.push('/verify_email');
                    }
                    console.log('res from onLogin: ',res)
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
                    this.authUser =null;
                    MyLocalStorage.setItem('token',this.token);
                    MyLocalStorage.setItem('username',this.username);
                    MyLocalStorage.setItem('isLogin',this.isLogin);
                    MyLocalStorage.setItem('user','');
                    MyLocalStorage.setItem('userId','');

                }
            )

        },
        onVerifEmail(code: string){
            this.isLoading = true;
            verifyEmailFn(code)
                .then(res=>{
                    this.isLoading = false;
                    createToast(res, {
                        position: 'top-right',
                    });
                    console.log(res);
                    MyLocalStorage.setItem('email','');
                    router.push('/login');
                }).catch(err=>{
                console.log('error from onVerifEmail: ',err);
            })
        },
        onRepeatVerificationCode(){
            let email:string = MyLocalStorage.getItem('email');
            getRepeatCodeFn(email)
                .then(res=>{
                    console.log(res)
                })
                .catch(err=>{
                    console.log('err from RepeatVerificationCode: ',err)
                })
        }

    }
});