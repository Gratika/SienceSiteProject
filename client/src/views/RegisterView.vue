<script setup lang="ts">
import { ref} from "vue";
import type { Ref } from "vue";
//валідація
import { useField, useForm } from 'vee-validate'
//типи користувача
import type {ISignUpInput} from "@/api/type";
import {useAuthStore} from "@/stores/authStore";
import moment from "moment";
const userBirthday = ref('');

const userRegister: Ref<ISignUpInput>  = ref({
  people:{
    id:'',
    surname:'',
    name:'',
    birthday:'',
    path_bucket:'',
    date_create: (new Date()).toISOString(),
    modified_date:(new Date()).toISOString()
  },
  email:'',
  password:'',
  //passwordConfirm:''
});
const authStore = useAuthStore();

/*валідація форм*/
const { handleSubmit, handleReset } = useForm({
  validationSchema: {
    email (value:string) {
      if (/^.+@[a-z.-]+\.[a-z]+$/i.test(value)) return true

      return 'Введіть валідну електронну адресу'
    },
    /*birthDate (value:string) {
       if (/^(0[1-9]|[1-2][0-9]|3[0-1])\.(0[1-9]|1[0-2])\.\d{4}$/.test(value) || value?.length==0) return true

       return 'Дата повина відповідати вказаному формату, або лишитися пустою'
    },*/
    password (value:string) {
      if (value?.length >= 8) return true

      return 'Пароль повинен містити не менше 8 символів.'
    },
    passwordConfirm (value:string) {
      if (value === password.value.value) return true
      return 'Паролі не співпадають'
    }
  },
})
const email= useField('email');
const  password = useField('password');
const passwordConfirm = useField('passwordConfirm');
//const birthDate = useField('birthDate');

function formatDate(input: null | string): string {
  //console.log('date=',date)
  // console.log('typeof date=',typeof date)
  if (input == null) return '';
  const parts = input.split('.');
 // return parts[2] +'-'+ parts[1]+'-'+parts[0];
  // Перетворення компонентів дати на числа
  const day = parseInt(parts[0], 10);
  const month = parseInt(parts[1], 10);
  const year = parseInt(parts[2], 10);

  // Створення нового об'єкта Date
  const date = new Date(year, month - 1, day);
  return date.toISOString()
}

const submitRegister = handleSubmit(values => {
  if (typeof email.value.value === "string") {
    userRegister.value.email = email.value.value;
  }
  if (typeof password.value.value === "string") {
    userRegister.value.password = password.value.value;
  }
  /*if (typeof birthDate.value.value === "string") {
    console.log('birthDate=', birthDate.value.value)
    userRegister.value.people.birthday=formatDate(birthDate.value.value);
  }*/
  /*if (typeof passwordConfirm.value.value === "string") {
    userRegister.value.passwordConfirm = passwordConfirm.value.value;
  }*/
  userRegister.value.people.birthday=formatDate(userBirthday.value)
  console.log('userRegister = ',userRegister.value)
  authStore.onRegistration(userRegister.value);

})

</script>

<template>
  <v-row class="py-7 justify-center">
      <v-overlay :model-value="authStore.isLoading"
                 class="align-center justify-center">
        <v-progress-circular
            indeterminate
            color="primary"
        ></v-progress-circular>
      </v-overlay>
    <v-col cols="10" >
      <v-form @submit.prevent="submitRegister">
        <v-row>
          <v-col cols="6">
            <div>
              <div class="mt-4 mb-3">
                <h2>Ім'я</h2>
              </div>
              <v-text-field
                  clearable
                  v-model.trim="userRegister.people.name"
                  label="Ім'я"
                  id="userName"
              />
              <div class="mt-2 mb-3">
                <h2>Дата народження</h2>
              </div>
              <v-text-field
                  clearable
                  v-model="userBirthday"
                  label="01.01.2000"
                  id="birthDate"


              />
              <div class="mt-2 mb-3">
                <h2>Пароль</h2>
              </div>
              <v-text-field
                  type="password"
                  clearable
                  v-model="password.value.value"
                  label="Пароль"
                  :counter="8"
                  id="password"
                  :error-messages="password.errorMessage.value"
              />
            </div>
          </v-col>
          <v-col cols="6">
            <div>
              <div class="mt-4 mb-3">
                <h2>Прізвище</h2>
              </div>
              <v-text-field
                  clearable
                  v-model.trim="userRegister.people.surname"
                  label="Прізвище"
                  id="surname"
              />
              <div class="mt-2 mb-3">
                <h2>Пошта</h2>
              </div>
              <v-text-field
                  clearable
                  v-model.trim="email.value.value"
                  label="email@example.com"
                  id="email"
                  :error-messages="email.errorMessage.value"

              />
              <div class="mt-2 mb-3">
                <h2>Повторити пароль</h2>
              </div>
              <v-text-field
                  type="password"
                  clearable
                  v-model="passwordConfirm.value.value"
                  label="Повторити пароль"
                  :counter="8"
                  id="passwordConfirm"
                  :error-messages="passwordConfirm.errorMessage.value"
              />
            </div>
          </v-col>
        </v-row>
        <v-row class="justify-center">
          <v-col cols="6">
            <v-btn type="submit" block class="mt-2">Реєстрація</v-btn>
          </v-col>
        </v-row>


      </v-form>

    </v-col>
  </v-row>
</template>

<style scoped>

</style>