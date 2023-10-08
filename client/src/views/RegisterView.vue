<script setup lang="ts">
import {computed, ref} from "vue";
import type { Ref } from "vue";
import { useVuelidate } from '@vuelidate/core'
import { required, email, minLength } from '@vuelidate/validators'


// Оголосимо типи для об'єкту форми та правил
interface Form {
  email: string;
  password: string;
}

const form: Ref<Form>  = ref({
  email:'',
  password:'',
});
const isLoading=ref(false);

const rules = {
  email: { required, email },
  password: { required, minLength: minLength(8) },
};

const $v = useVuelidate(rules, form);
//опис помилок невалідних полів
const emailErrors = computed(() => {
  console.log('computed email' );
  const errors:string[] = [];
  // Перевірка, чи був введений текст в поле
 /* if (!$v.value.email.$dirty) {
    return errors;
  }
  // Перевірка наявності помилок та додавання їх до масиву errors
  if (!$v.value.email.email) {
    errors.push('Неправильна електронна адреса');
  }
  if (!$v.value.email.required) {
    errors.push('Електронна адреса є обов\'язковим полем.');
  }*/
  if ($v.value.email?.$pending) return errors;  // Return if validation is pending
  if (!$v.value.email?.$model) {
    errors.push('Електронна адреса обов\'язкова.');
  } else if (!$v.value.email?.$valid) {
    errors.push('Неправильна електронна адреса.');
  }
  console.log(errors)
  return errors;
});
const passwordErrors = computed(() => {
  const errors:string[] = [];
  if (!$v.value.password.$dirty) return errors;
  !$v.value.password.required && errors.push('Пароль є обов\'язковим полем.');
  !$v.value.password.minLength &&  errors.push('Мінімальна довжина пароля 8 символів');
  return errors;
});
function submitLogin(){
  console.log('Form submitted!');
    // Якщо форма є валідною, відправляємо дані
  $v.value.$touch();
  if (!$v.value.$pending && !$v.value.$invalid)
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
          Реєстрація нового користувача
        </v-card-title>
        <v-card-item>
          <v-form @submit.prevent="submitLogin">
            <v-text-field
                clearable
                v-model="form.email"
                label="Електронна пошта"
                prepend-inner-icon="mdi-email"
                variant="solo"
                :error-messages="emailErrors"
                @input="$v.email.$touch()"
            ></v-text-field>
            <v-text-field
                type="password"
                clearable
                v-model="form.password"
                label="Пароль"
                prepend-inner-icon="mdi-key"
                variant="solo"
                :error-messages="passwordErrors"
                @input="$v.password.$touch()"
                @blur="$v.password.$touch()"

            ></v-text-field>

            <v-btn type="submit" block class="mt-2" color="red" >Submit</v-btn>
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