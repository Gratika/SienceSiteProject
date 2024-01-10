
<script setup lang="ts">
import {onMounted, ref} from 'vue';
import type {IPeople, IUser} from "@/api/type";
import {useAuthStore} from "@/stores/authStore";
import {useField, useForm} from "vee-validate";
import moment from "moment";

const authStore = useAuthStore();
let userName =ref('');
let userSurname =ref('');
let userBirthday =ref<string|null>('');
let user = ref<IUser>({
    id:'',
    login:'',
    password:'',
    email:'',
    access_token:'',
    refresh_token:'',
    date_create: '',
    modified_date: '',
    role_id:1,
    email_is_checked:0,
    people_id:'',
    people_:null,
});
let people_ = ref<IPeople>(
    {
      id:'',
      surname:'',
      name:'',
      birthday:'',
      path_bucket:'',
      date_create: '',
      modified_date:''
    }
)
onMounted(()=>{
  if (authStore.authUser!==null){
    user.value = authStore.authUser;
    userName.value = getName();
    userSurname.value = getSurname();
    userBirthday.value = getBirthday();
    email.value.value=user.value.email;
  }
});
function formatDate(date: null | string): string {
  console.log('date=',date)
  console.log('typeof date=',typeof date)
  if (date == null) return '';
  const formattedDate: Date = new Date(date);
  return (moment(formattedDate)).format('DD.MM.YYYY HH:mm')
}
function getName():string {
  if (user.value.people_ ==null) return '';
  return user.value.people_.name;
}
function getSurname():string {
  if (user.value.people_ ==null) return '';
  return user.value.people_.surname;
}
function getBirthday():string|null {
  if (user.value.people_ ==null) return '';
  return formatDate(user.value.people_.birthday);
}
/*валідація форм*/
const { handleSubmit, handleReset } = useForm({
  validationSchema: {
    email (value:string) {
      if (/^.+@[a-z.-]+\.[a-z]+$/i.test(value)) return true

      return 'Введіть валідну електронну адресу'
    },
  },
})
const email= useField('email');

const submitLogin= handleSubmit(()=>{
  if (typeof email.value.value === "string") {
    user.value.email = email.value.value;
  }
  if (user.value.people_ ==null){
    people_.value.name = userName.value;
    people_.value.surname=userSurname.value;
    people_.value.birthday=userBirthday.value;
    user.value.people_=people_.value;
  }else{
    user.value.people_.surname=userSurname.value;
    user.value.people_.name = userName.value;
    user.value.people_.birthday = userBirthday.value;
  }
  //authStore.onLogin(userLogin.value);
  alert(JSON.stringify(user.value));
})
</script>

<template>
  <v-container>
    <v-sheet class="mt-10 pa-10">
      <v-row>
        <v-col cols="12">
          <div class="gradient"/>
          <div class="my-10">
            <v-avatar image="avatar.png" size="x-large"></v-avatar>
          </div>
          <v-form @submit.prevent="submitLogin" >

              <div class="mt-4 mb-3">
                <h2>Ім'я</h2>
              </div>
              <v-text-field
                  v-model.trim="userName"
                  label="Ім'я"
                  id="userName"
              />
            <div class="mt-4 mb-3">
              <h2>Прізвище</h2>
            </div>
            <v-text-field
                clearable
                v-model.trim="userSurname"
                label="Прізвище"
                id="surname"
            />
              <div class="mt-2 mb-3">
                <h2>Дата народження</h2>
              </div>
              <v-text-field
                  clearable
                  v-model.trim="userBirthday"
                  label="01.01.2000"
                  id="birthday"
              />
            <div class="mt-4 mb-3">
              <h2>Пошта</h2>
            </div>
            <v-text-field
                v-model="email.value.value"
                label="email@example.com"
                id="email"
                :error-messages="email.errorMessage.value"
            />
            <div class="d-flex justify-end">
              <v-btn type="submit" class="mt-4">Зберегти</v-btn>
            </div>
          </v-form>
        </v-col>
      </v-row>
    </v-sheet>
  </v-container>
</template>

<style scoped>
.gradient{
  background: linear-gradient(to bottom right, #ead9ff, #c3f2ff);
  max-height: 100px;
  min-height: 100px;
  position: relative;
  width: 100%;
}
</style>
