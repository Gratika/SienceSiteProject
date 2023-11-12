<script setup lang="ts">
  import {ref} from "vue";
  import { useField, useForm } from "vee-validate"
  import type { Ref } from "vue";
  import type {ILoginInput}  from "@/api/type";
  import {useAuthStore} from "@/stores/authStore";

  const userLogin: Ref<ILoginInput> = ref({
    email:'',
    password:'',
  })
  const authStore = useAuthStore();;

  /*валідація форм*/
  const { handleSubmit, handleReset } = useForm({
    validationSchema: {
      email (value:string) {
        if (/^.+@[a-z.-]+\.[a-z]+$/i.test(value)) return true

        return 'Введіть валідну електронну адресу'
      },
      password (value:string) {
        if (value?.length > 0) return true

        return 'Введіть пароль'
      },
    },
  })
  const email= useField('email');
  const  password = useField('password');

  const isLoading=ref(false)

  const submitLogin= handleSubmit(()=>{
    if (typeof email.value.value === "string") {
      userLogin.value.email = email.value.value;
    }
    if (typeof password.value.value === "string") {
      userLogin.value.password = password.value.value;
    }
    authStore.onLogin(userLogin.value);
    //alert(JSON.stringify(userLogin.value));
  })
</script>

<template>
<v-row class="justify-center">
  <v-col cols="12"  md="4" sm="8" xs="12">
    <v-overlay :model-value="isLoading"
    class="align-center justify-center">
      <v-progress-circular
          indeterminate
          color="primary"
      ></v-progress-circular>
    </v-overlay>
    <v-card class="my-8">
      <v-card-title class="text-center">
        Вхід
      </v-card-title>
      <v-card-item>
        <v-form @submit.prevent="submitLogin" >

          <v-text-field
              clearable
              v-model="email.value.value"
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
              id="password"
              :error-messages="password.errorMessage.value"
          />

          <v-btn type="submit" block class="mt-2" color="my-accent" >Submit</v-btn>
        </v-form>
      </v-card-item>
      <v-card-actions>
        <v-btn block to="/register">Зареєструватися</v-btn>
      </v-card-actions>
    </v-card>

  </v-col>
</v-row>
</template>

<style scoped>

</style>