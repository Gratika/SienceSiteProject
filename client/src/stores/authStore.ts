import { defineStore } from 'pinia';


import MyLocalStorage from "@/services/myLocalStorage";
import type {ILoginInput, IUser, ISignUpInput} from "@/api/type";
import {
    getRepeatCodeFn,
    loginUserFn,
    logoutUserFn, showErrorMessage,
    signUpUserFn,
    verifyEmailFn
} from '@/api/authApi'
import { createToast } from 'mosha-vue-toastify';
import router from '@/router'





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
        getPeopleId(state): string {
            if (state.authUser) {
                return state.authUser.people_id ;
            }
            return '';
        },
        getUserId(state): string {
            if (state.authUser) {
                return state.authUser.id ;
            }
            return '';
        },
    },

    actions: {

        onRegistration(user:ISignUpInput){
            this.isLoading=true;
            signUpUserFn(user).then(
                res=>{
                    this.isLoading = false;
                    createToast(res.message, {
                        position: 'top-right',
                    });
                    MyLocalStorage.setItem('email', user.email);
                   router.push('/verify_email');
                }
            ).catch(error => {
                this.isLoading = false;
                console.log('error from onRegistration: ',error);

                showErrorMessage(error);
            })
        },
        onLogin(user:ILoginInput){
            loginUserFn(user).then(
                res=>{
                    console.log("authUser: ",res)
                    this.token = res.message.user.access_token;
                    console.log("token: ",res.message.user.access_token);
                    this.username = res.message.user.login;
                    this.isLogin =true;
                    this.authUser = res.message.user;
                    MyLocalStorage.setItem('token',this.token);
                    MyLocalStorage.setItem('username',this.username);
                    MyLocalStorage.setItem('isLogin',this.isLogin);
                    if(this.authUser!=null){
                        MyLocalStorage.setItem('user',this.authUser.toString());
                        MyLocalStorage.setItem('userId',this.authUser.id);
                        MyLocalStorage.setItem('peopleId',this.authUser.people_id);
                        MyLocalStorage.setItem('email', this.authUser.email);
                        MyLocalStorage.setItem('bucketName', this.authUser.people_!.file_bucket);
                    }
                    if(res.message.user.email_is_checked ===0){
                        router.push('/verify_email');
                    }
                    createToast("Log in!", {
                        position: 'bottom-center',
                    });
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
                    MyLocalStorage.setItem('peopleId','');
                    MyLocalStorage.setItem('email','');
                    MyLocalStorage.setItem('bucketName', '');

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
                    //MyLocalStorage.setItem('email','');
                    router.push('/login');
                }).catch(err=>{
                this.isLoading = false;
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