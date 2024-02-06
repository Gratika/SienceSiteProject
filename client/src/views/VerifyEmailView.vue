<script setup lang="ts">
import {useField, useForm} from "vee-validate";
import {useAuthStore} from "@/stores/authStore";
import myLocalStorage from "@/services/myLocalStorage";
import {ref} from "vue";

const authStore = useAuthStore();

const vCode = ref('');
function codeIsValid() {
  return  vCode.value.length === 4;
}
/*const { handleSubmit, handleReset } = useForm({
  validationSchema: {
    code (value:string) {
      if (value?.length > 0) return true
      return 'Введіть отриманий код'
    },
  },
})
const code= useField('code');
const submitCode = handleSubmit(()=>{
  let code_:string = '';
  if (typeof code.value.value === 'string') code_ = code.value.value;
  console.log('code_ = ', code_);
  authStore.onVerifEmail(code_);
})*/
function submitCode() {
  if (codeIsValid()){
    console.log('code_ = ', vCode.value);
    authStore.onVerifEmail(vCode.value.trim());
  }
}

function getRepeatCode(){
  authStore.onRepeatVerificationCode();
}
function getUserEmail(){
  const email=myLocalStorage.getItem('email');
  if (email!==null){
    const pos=(email as string).indexOf('@');
    if(pos>-1){
      const len=pos>4 ? 3 : 1;
      return email.substring(0,len)+'...'+email.substring(pos);
    }else return '????@?????'
  }
  return '????@?????'

}
</script>

<template>
  <v-container>
    <v-row class="justify-center">
      <v-col cols="12" md="5" sm="8" xs="12">
        <v-overlay :model-value="authStore.isLoading"
                   class="align-center justify-center">
          <v-progress-circular
              indeterminate
              color="primary"
          ></v-progress-circular>
        </v-overlay>
        <div class="base-area">
          <v-card class="pa-8">
            <div class="verif-title">
              Підтвердити електронну адресу
            </div>
            <div class="text-h6 text-center">
              <div >Ми надіслали код підтвердження на адресу {{getUserEmail()}}.
               Перевірте свою електронну пошту та вставте код нижче.</div>
            </div>

            <v-card-item>
              <v-form @submit.prevent="submitCode">
                <v-otp-input
                    class="text-h5 font-weight-medium"
                    :error="!codeIsValid()"
                    length="4"
                    type="text"
                    variant="outlined"
                    v-model="vCode"
                    @blur="codeIsValid"
                ></v-otp-input>
                <!--v-text-field
                    v-model="code.value.value"
                    label="Код"
                    id="code"
                    :error-messages="code.errorMessage.value"
                ></v-text-field-->
                <v-btn
                    color="primary"
                    type="submit"
                    block
                    class="mt-2 text-h5"
                >Надіслати</v-btn>
              </v-form>
            </v-card-item>
            <v-card-actions>
              <v-btn
                  block
                  @click="getRepeatCode"
                  class="text-h6"
              >
                Отримати знову
              </v-btn>
            </v-card-actions>
          </v-card>
        </div>

      </v-col>
    </v-row>
  </v-container>

</template>

<style scoped>
.verif-title{
  display: flex;
  flex-direction: row;
  font-size: 30px;
  justify-content: center;
  padding: 12px 0;
  text-align: center;
}
.base-area{
  display: flex;
  flex-direction: column;
  justify-content: center;
  min-height: 500px;
}
</style>