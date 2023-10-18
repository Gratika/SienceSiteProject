<script setup lang="ts">
import {useField, useForm} from "vee-validate";
import {verifyEmailFn} from "@/api/authApi";
import {ref} from "vue";
import router from "@/router";

const isLoading = ref(false);
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
  isLoading.value=true;
  let code_:string = '0';
  if (typeof code.value.value === 'string') code_ = code.value.value;

  verifyEmailFn(code_).then(
      res=>{
        isLoading.value=false;
        router.push('/login')
      })
})
</script>

<template>
 <v-row>
   <v-col cols="5">
     <v-overlay :model-value="isLoading"
                class="align-center justify-center">
       <v-progress-circular
           indeterminate
           color="primary"
       ></v-progress-circular>
     </v-overlay>
     <v-form @submit.prevent="submitCode">
       <v-text-field
           v-model="code.value.value"
           label="Код"
           id="code"
           :error-messages="code.errorMessage.value"
       ></v-text-field>
       <v-btn type="submit" block class="mt-2">Submit</v-btn>
     </v-form>
   </v-col>
 </v-row>
</template>

<style scoped>

</style>