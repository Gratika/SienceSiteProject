<script setup lang="ts">
import {computed, ref} from "vue";
  import { useField, useForm } from "vee-validate"
  import type { Ref } from "vue";
  import type {ILoginInput}  from "@/api/type";
  import {useAuthStore} from "@/stores/authStore";
import {useRouter} from "vue-router";

  const userLogin: Ref<ILoginInput> = ref({
    email:'',
    password:'',
  })

  const authStore = useAuthStore();
  const router = useRouter();
  const previousRoute = computed(()=>{
    const lastPath = router.options.history.state['back'];
    return (lastPath ? lastPath : '/') as string;

  })


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

    authStore.onLogin(userLogin.value, previousRoute.value);
    //alert(JSON.stringify(userLogin.value));
  })
</script>

<template>
<v-row class="py-7 justify-center">
  <v-col cols="12"  md="6" sm="10" xs="12">
    <v-overlay :model-value="isLoading"
    class="align-center justify-center">
      <v-progress-circular
          indeterminate
          color="primary"
      ></v-progress-circular>
    </v-overlay>
    <v-form @submit.prevent="submitLogin" >
      <div class="mt-4 mb-3">
        <h2>Пошта або логін</h2>
      </div>
      <v-text-field
          v-model="email.value.value"
          label="email@example.com"
          id="email"
          :error-messages="email.errorMessage.value"
      />
      <div class="mt-3 mb-3">
      <h2>Пароль</h2>
      </div>
      <v-text-field
          type="password"
          clearable
          v-model="password.value.value"
          label="Пароль"
          id="password"
          :error-messages="password.errorMessage.value"
      />
      <!--v-text-field
          clearable
          v-model="email.value.value"
          label="Електронна пошта"
          prepend-inner-icon="mdi-email"
          id="email"
          :error-messages="email.errorMessage.value"
      /-->
      <!--v-text-field
          type="password"
          clearable
          v-model="password.value.value"
          label="Пароль"
          prepend-inner-icon="mdi-key"
          id="password"
          :error-messages="password.errorMessage.value"
      /-->

      <v-btn type="submit" block class="mt-4">Увійти</v-btn>
    </v-form>
    <div class="my-4">
      <h3 class="text-center">або</h3>
    </div>
    <div class="d-flex justify-space-between ">
      <v-btn
          class="d-flex flex-grow-1"
          color="black"
          variant="outlined"
      >
        <v-img
            class="me-3"
            :width="16"
            src="/logo/Google__G__Logo 1.png"
        ></v-img>
        Google
      </v-btn>
      <v-btn
          class="d-flex w-50 ms-2"
          color="black"
          variant="outlined"
      >
        <v-img
            class="me-3"
            :width="16"
            src="/logo/microsoft-5 1.png"
        ></v-img>
        Microsoft
      </v-btn>
    </div>
    <!--v-card class="my-8">
      <v-card-title class="text-center">
        Вхід
      </v-card-title>
      <v-card-item>
        <v-form @submit.prevent="submitLogin" >
          <v-text-field
              v-model="email.value.value"
              label="Електронна пошта"
              id="email"
              :error-messages="email.errorMessage.value"
          />
          <v-text-field
              clearable
              v-model="email.value.value"
              label="Електронна пошта"
              prepend-inner-icon="mdi-email"
              id="email"
              :error-messages="email.errorMessage.value"
          />
          <v-text-field
              type="password"
              clearable
              v-model="password.value.value"
              label="Пароль"
              prepend-inner-icon="mdi-key"
              id="password"
              :error-messages="password.errorMessage.value"
          />

          <v-btn type="submit" block class="mt-2" color="my-accent" >Submit</v-btn>
        </v-form>
      </v-card-item>
      <v-card-actions>
        <v-btn block to="/register">Зареєструватися</v-btn>
      </v-card-actions>
    </v-card-->

  </v-col>
</v-row>
</template>

<style scoped>
input .v-field__input{
  border-radius: 0;
  border: 1px;
}
</style>