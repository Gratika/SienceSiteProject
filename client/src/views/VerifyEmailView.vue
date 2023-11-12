<script setup lang="ts">
import {useField, useForm} from "vee-validate";
import {useAuthStore} from "@/stores/authStore";


const authStore = useAuthStore();
const { handleSubmit, handleReset } = useForm({
  validationSchema: {
    code (value:string) {
      if (value?.length > 0) return true

      return 'Введіть отриманий код'
    },
  },
})
const code= useField('code');
const submitCode = handleSubmit(()=>{
  let code_:string = '0';
  if (typeof code.value.value === 'string') code_ = code.value.value;
  authStore.onVerifEmail(code_);
})
function getRepeatCode(){
  authStore.onRepeatVerificationCode();
}
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
       Підтвердити електронну адресу
     </v-card-title>
     <v-card-item>
     <v-form @submit.prevent="submitCode">
       <v-text-field
           v-model="code.value.value"
           label="Код"
           id="code"
           :error-messages="code.errorMessage.value"
       ></v-text-field>
         <v-btn color="my-accent" type="submit" block class="mt-2">Submit</v-btn>
     </v-form>
     </v-card-item>
       <v-card-actions>
         <v-btn block @click="getRepeatCode">Отримати знову</v-btn>
       </v-card-actions>
     </v-card>
   </v-col>
 </v-row>
</template>

<style scoped>

</style>