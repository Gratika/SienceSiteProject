
<script setup lang="ts">
import {onMounted, ref} from 'vue';
import type {IPeople, IUser} from "@/api/type";
import {useAuthStore} from "@/stores/authStore";
import {useField, useForm} from "vee-validate";
import moment from "moment";
import {useLayoutStore} from "@/stores/layoutStore";
import {useRouter} from "vue-router";
import Swal from "sweetalert2";
import {showErrorMessage} from "@/api/authApi";

const authStore = useAuthStore();
const layoutStore = useLayoutStore();
const router = useRouter();
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
  if(!restoreLayout()) {
    authStore.getPeople()
        .then((res: IPeople | undefined) => {
          if (res !== undefined) {
            initialContent(res);
          }
        });
  }
  email.value.value = authStore.authUser?.email;
  window.scroll(0,0)
});
function initialContent(res:IPeople){
  people_.value = res
  people_.value.birthday = formatDate(people_.value.birthday);
  birthDate.value.value=people_.value.birthday;
}
function saveLayout(people:IPeople){
  layoutStore.saveLayoutProfile(people);
}
function restoreLayout():boolean{
  let savePeople = layoutStore.restoreLayoutProfile();
  if(savePeople!==null){
    initialContent(savePeople);
    layoutStore.saveLayoutProfile(null)
    return true;
  }
  return false;
}
function formatDate(date: null | string): string {
  //console.log('date=',date)
  //console.log('typeof date=',typeof date)
  if (date == null) return '';
  const formattedDate: Date = new Date(date);
  return (moment(formattedDate)).format('DD.MM.YYYY')
}
//"dd.mm.yyyy" => "yyyy-mm-dd"
function getFormatDate(input: undefined | string): string {
  //console.log('date=',date)
  // console.log('typeof date=',typeof date)
  if (input === undefined || input==='') return (new Date()).toISOString();
  const parts = input.split('.');
  // return parts[2] +'-'+ parts[1]+'-'+parts[0];
  // Перетворення компонентів дати на числа
  const day = parseInt(parts[0], 10);
  //console.log('day=',day)
  const month = parseInt(parts[1], 10);
  //console.log('month=',month)
  const year = parseInt(parts[2], 10);
  //console.log('year=',year)

  // Створення нового об'єкта Date
  const date = new Date(year, month - 1, day);
  return date.toISOString()
}

/*валідація форм*/
const { handleSubmit, handleReset } = useForm({
  validationSchema: {
    email (value:string) {
      if (/^.+@[a-z.-]+\.[a-z]+$/i.test(value)) return true

      return 'Введіть валідну електронну адресу'
    },
    birthDate (value:string) {
      if(dateIsValid(value))return true
      return 'Дата повина відповідати вказаному формату, або лишитися пустою'
    },
  },
})
const email= useField('email');
const birthDate = useField('birthDate');
function dateIsValid(date: string|undefined):boolean{
  //console.log('birdDay=', date)
  if(date===undefined || date?.length===0) return true;
  return /^(0[1-9]|[1-2][0-9]|3[0-1])\.(0[1-9]|1[0-2])\.\d{4}$/.test(date);

}
const submitLogin= handleSubmit(()=>{
  let userEmail = '';
  if (typeof email.value.value === "string") {
    userEmail = email.value.value;
  }
  if (typeof birthDate.value.value === "string") {
    //console.log('birthDate=', birthDate.value.value)
    people_.value.birthday=getFormatDate(birthDate.value.value);
  }else{
    people_.value.birthday=(new Date()).toISOString();
  }
  authStore.onUpdateUser(people_.value, userEmail)
      .catch((error)=>{
        Swal.fire({
          icon: 'error',
          title: 'Помилка при збереженні даних користувача',
          text: showErrorMessage(error)
        });
        if('name' in error
            && error.name==='AxiosError'
            && error.response?.status===401){
          saveLayout(people_.value);
          authStore.delUserData();
          router.push('/login');
        }
      });
  //alert(JSON.stringify(people_.value));
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
                  v-model.trim="people_.name"
                  label="Ім'я"
                  id="userName"
              />
            <div class="mt-4 mb-3">
              <h2>Прізвище</h2>
            </div>
            <v-text-field
                clearable
                v-model.trim="people_.surname"
                label="Прізвище"
                id="surname"
            />
              <div class="mt-2 mb-3">
                <h2>Дата народження</h2>
              </div>
              <v-text-field
                  clearable
                  v-model.trim="birthDate.value.value"
                  label="01.01.2000"
                  id="birthday"
                  :error-messages="birthDate.errorMessage.value"
              />
            <div class="mt-4 mb-3">
              <h2>Пошта</h2>
            </div>
            <v-text-field
                :disabled="true"
                v-model="email.value.value"
                label="email@example.com"
                id="email"
                :error-messages="email.errorMessage.value"
            />
            <div class="d-flex justify-end">
              <v-btn type="submit" class="mt-4 text-h6">Зберегти</v-btn>
            </div>
          </v-form>
        </v-col>
      </v-row>
    </v-sheet>
    <v-row>
      <div class="footer-distance"></div>
    </v-row>
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
.footer-distance{
  min-height: 100px;
}
</style>
