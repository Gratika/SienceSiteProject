<script setup lang="ts">
import { ref} from "vue";
import type { Ref } from "vue";
//валідація
import { useField, useForm } from 'vee-validate'
//типи користувача
import type {ISignUpInput} from "@/api/type";
import {useAuthStore} from "@/stores/authStore";

const userRegister: Ref<ISignUpInput>  = ref({
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

const submitRegister = handleSubmit(values => {
  if (typeof email.value.value === "string") {
    userRegister.value.email = email.value.value;
  }
  if (typeof password.value.value === "string") {
    userRegister.value.password = password.value.value;
  }
  /*if (typeof passwordConfirm.value.value === "string") {
    userRegister.value.passwordConfirm = passwordConfirm.value.value;
  }*/
  authStore.onRegistration(userRegister.value)

})

</script>

<template>
  <v-row class="justify-center">
    <v-col cols="12" md="4" sm="8" xs="12">
      <v-overlay :model-value="authStore.isLoading"
                 class="align-center justify-center">
        <v-progress-circular
            indeterminate
            color="primary"
        ></v-progress-circular>
      </v-overlay>
      <v-card class="my-8">
        <v-card-title class="text-center">
          Реєстрація
        </v-card-title>
        <v-card-item>
          <v-form @submit.prevent="submitRegister">

            <v-text-field
                clearable
                v-model.trim="email.value.value"
                label="Електронна пошта"
                prepend-inner-icon="mdi-email"
                variant="solo"
                id="email"
                :error-messages="email.errorMessage.value"

            />

            <v-text-field
                type="password"
                clearable
                v-model="password.value.value"
                label="Пароль"
                prepend-inner-icon="mdi-key"
                variant="solo"
                :counter="8"
                id="password"
                :error-messages="password.errorMessage.value"
            />
            <v-text-field
                type="password"
                clearable
                v-model="passwordConfirm.value.value"
                label="Повторіть пароль"
                prepend-inner-icon="mdi-key"
                variant="solo"
                :counter="8"
                id="passwordConfirm"
                :error-messages="passwordConfirm.errorMessage.value"
            />

            <v-btn type="submit" block class="mt-2" color="my-accent" >Submit</v-btn>
          </v-form>
        </v-card-item>
        <v-card-actions>
          <v-btn block to="/login">Увійти</v-btn>
        </v-card-actions>
      </v-card>

    </v-col>
  </v-row>
</template>

<style scoped>

</style>