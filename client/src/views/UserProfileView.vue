
<script setup lang="ts">
import {onMounted, ref} from 'vue';
import type {IPeople, IUser} from "@/api/type";
import {useAuthStore} from "@/stores/authStore";
import type { Ref} from "vue"

const authStore = useAuthStore();
const user = ref<IUser>({
    id:'',
    login:'',
    password:'',
    email:'',
    access_token:'',
    refresh_token:'',
    date_create: new Date(),
    modified_date: new Date(),
    role_id:1,
    email_is_checked:0,
    people_id:'',
    people_:{
      id:'',
      surname:'',
      name:'',
      birthday:new Date(),
      path_bucket:'',
      date_create: new Date(),
      modified_date: new Date()
    },
});
onMounted(()=>{
  if (authStore.authUser){
    user.value = authStore.authUser;
  }
});
const imageToUpload: Ref<FileList | null> = ref(null);
const avatarUrl = ref<string>('');
const standartURL ='@/assets/image/avatar.png';
const handleFileChange = (event: Event & { target: HTMLInputElement & { files: FileList }}) => {
  imageToUpload.value = event.target.files;
  const file = event.target.files?.[0];
  if (file) {
    const reader = new FileReader();
    reader.onload = () => {
      avatarUrl.value = reader.result as string;
    };
    reader.readAsDataURL(file);
  } else {
    avatarUrl.value = standartURL;
  }
};
/*function saveUserData(){
  authStore.onUpdateUser(user.value);
}*/
function saveAvatar(){
  if (!imageToUpload.value) {
    console.error('Будь ласка, оберіть файли для завантаження');
    return;
  }
  const formData = new FormData();
  for (let i = 0; i < imageToUpload.value.length; i++) {
    formData.append('files[]', imageToUpload.value[i]);
  }
  authStore.onSaveAvatar(formData);


}

</script>

<template>
  <v-container>
    <v-row>
      <v-col cols="4">
        <v-card>
          <v-card-title>Avatar</v-card-title>
          <v-file-input
              clearable
              label="Завантажити файли"
              variant="solo-inverted"
              @change="handleFileChange"
          ></v-file-input>
          <img class="rounded" :src="avatarUrl" alt="Avatar"/>
          <v-btn @click="saveAvatar" v-if="avatarUrl">Save Avatar</v-btn>
        </v-card>
      </v-col>

      <v-col cols="8">

            <v-form ><!--@submit.prevent="saveUserData"-->
              <v-card>
                <v-card-title>Profile</v-card-title>
                <v-card-text>
                  <v-row>
                    <v-col cols="6">

                      <v-text-field v-model="user.login" label="Login"></v-text-field>
                      <v-text-field v-model="user.password" label="Password" type="password"></v-text-field>
                      <v-text-field v-model="user.email" label="Email"></v-text-field>
                      <v-checkbox v-model="user.email_is_checked" label="Email Verified"></v-checkbox>


                    </v-col>
                    <!--v-col cols="6">

                      <v-text-field v-model="user.people_.surname" label="Surname"></v-text-field>
                      <v-text-field v-model="user.people_.name" label="Name"></v-text-field>
                      <v-date-picker v-model="user.people_.birthday.toLocaleDateString()" label="Birthday"></v-date-picker>

                    </v-col-->
                  </v-row>
                </v-card-text>
                <v-card-actions>
                   <v-btn class="mx-2" type="submit">Зберегти</v-btn>
                   <v-btn class="mx-2">Скасувати</v-btn>
                </v-card-actions>

              </v-card>
            </v-form>

      </v-col>
    </v-row>
  </v-container>
</template>

<style>

</style>
