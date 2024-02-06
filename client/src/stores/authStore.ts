import { defineStore } from 'pinia';


import MyLocalStorage from "@/services/myLocalStorage";
import type {ILoginInput, IUser, ISignUpInput, ILoginResponse, IPeople} from "@/api/type";
import {
    getRepeatCodeFn,
    loginUserFn,
    sendRequest, showErrorMessage,
    signUpUserFn,
    verifyEmailFn
} from '@/api/authApi'
import { createToast } from 'mosha-vue-toastify';
import router from '@/router'
import {ServerBadRequestError} from "@/api/appException";
import Swal from "sweetalert2";


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
        authUser: MyLocalStorage.getItem('user')?(MyLocalStorage.getItem('user')):null,
        token: MyLocalStorage.getItem('token')? MyLocalStorage.getItem('token'): '',
        isLogin:MyLocalStorage.getItem('isLogin')?MyLocalStorage.getItem('isLogin')===true:false,
        username: MyLocalStorage.getItem('username')?MyLocalStorage.getItem('username'):'',
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
                   MyLocalStorage.setItem('email', user.email);
                   router.push('/verify_email');
                }
            ).catch((error) => {
                console.log('error=',error);
                Swal.fire({
                    icon: 'error',
                    title: 'Помилка реєстрації',
                    text: showErrorMessage(error)
                });
            }).finally(()=>{
                this.isLoading = false;
            })
        },
        onLogin(user:ILoginInput, previousRoute:string){
            this.isLoading=true;
            loginUserFn(user).then(
                res=>{
                    console.log(res);
                    this.token = res.user.access_token;
                    this.username = res.user.login;
                    this.isLogin =true;
                    this.authUser = res.user;
                    console.log('res.user',res.user);
                    console.log('this.authUser',this.authUser);
                    MyLocalStorage.setItem('token',this.token);
                    MyLocalStorage.setItem('username',this.username);
                    MyLocalStorage.setItem('isLogin',this.isLogin);
                    if(this.authUser!=null){
                        MyLocalStorage.setItem('user', this.authUser);
                        MyLocalStorage.setItem('userId',this.authUser.id);
                        MyLocalStorage.setItem('peopleId',this.authUser.people_id);
                        MyLocalStorage.setItem('email', this.authUser.email);
                        MyLocalStorage.setItem('bucketName', this.authUser.people_?.path_bucket);
                    }
                    if(res.user.email_is_checked ===0){
                        router.push({path:'/verify_email'}).then((res)=>{
                            console.log('push_res=',res);
                        });
                    }else{
                        router.push({path: previousRoute});
                    }

                }
            ).catch(error => {
                console.log('error login=', error);
                    Swal.fire({
                        icon: 'error',
                        title: 'Помилка авторизації',
                        text: showErrorMessage(error)
                    });
                //throw error
                //this.showErrorMessage(error)

            }).finally(()=>{
                this.isLoading = false;
            })
        },
        onLogout(){
            //logoutUserFn().then(
              //  res=>{
                    //console.log(res.message);
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
                    router.push({path:'/'})

               // }
            //)

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
        },
        async getPeople():Promise<IPeople|undefined>{
            const peopleId = this.getPeopleId;
            try{
                const  res = await sendRequest<IPeople>(
                    'GET',
                    'people/getPeople',
                    {id: peopleId}
                );
                console.log('people = ',res)
                return res;
            }catch(error) {
                Swal.fire({
                    icon: 'error',
                    title: 'Не вдалося завантажити дані користувача',
                    text: showErrorMessage(error)
                });
            }
        },
        async onUpdateUser( updatePeople:IPeople, email:string){

            sendRequest<string>(
                    'POST',
                    'people/redactPeople',
                    undefined,
                updatePeople
            ).then(res=>{
                console.log('user email=', email)
                if(this.authUser !== null ) {
                    this.authUser.email = email;
                    MyLocalStorage.setItem('email', email);
                }

                Swal.fire({
                    icon: 'info',
                    title: res ? res : 'Виконано',
                    text: ''
                });


            }).catch(error => {
                console.log('error login=', error);
                Swal.fire({
                    icon: 'error',
                    title: 'Помилка при збереженні даних користувача',
                    text: showErrorMessage(error)
                });
            })
        },
        async onSaveAvatar( imageData:FormData){
            console.log("imageData = ", imageData)
            /*sendRequest<ILoginResponse>(
                    'POST',
                    'auth/updateUser',
                    undefined,
                    imageData
            ).then(res=>{
                console.log("res: ",res);
            }).catch(error => {
                //showErrorMessage(error);
            })*/
        },

    }
});