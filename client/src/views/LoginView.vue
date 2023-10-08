<script setup lang="ts">
  import {ref} from "vue";

  const form = ref({
    login:'',
    email:'',
    password:'',
    remember:false
  })
  const rules={}
  const isLoading=ref(false)
  function submitLogin(){
    alert(JSON.stringify(form.value));
  }
</script>

<template>
<v-row class="justify-center">
  <v-col cols="4">
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
              v-model="form.login"
              label="Логін"
              prepend-inner-icon="mdi-account"
              variant="solo"
          ></v-text-field>
          <v-text-field
              clearable
              v-model="form.email"
              label="Електронна пошта"
              prepend-inner-icon="mdi-email"
              variant="solo"
          ></v-text-field>
          <v-text-field
              type="password"
              clearable
              v-model="form.password"
              label="Пароль"
              prepend-inner-icon="mdi-key"
              variant="solo"
          ></v-text-field>
          <v-checkbox
              v-model="form.remember"
              label="Запам'ятати мене"
              color="red"
              hide-details
          ></v-checkbox>
          <v-btn type="submit" block class="mt-2" color="red" >Submit</v-btn>
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