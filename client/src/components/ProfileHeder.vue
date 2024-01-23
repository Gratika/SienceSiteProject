<script setup lang="ts">
  import {computed} from "vue";
  import  type {ComputedRef} from "vue";
  import type {IUser} from "@/api/type";
  import MyLocalStorage from "@/services/myLocalStorage";
  import {useRouter} from "vue-router";

  const user:ComputedRef<null | IUser>= computed(()=>{
    let userStorage = MyLocalStorage.getItem('user');
    if (userStorage == null) return null;
    return userStorage as IUser;
  })
  const router = useRouter();
  function goToUserProfile(){
    router.push({ name: 'profile'});
  }
</script>

<template>
<v-row>
  <v-col cols="12" class="pt-0">
    <div class="gradient">
      <div class="avatar-overlay">
        <v-avatar image="avatar.png" size="x-large"></v-avatar>
      </div>
    </div>
    <div class="general-size content-zone">
      <div class="btn-zone general-size">
        <v-chip color="black" variant="tonal" class="rounded-btn text-subtitle-1" size="small" @click="goToUserProfile">
          Редагувати профіль
        </v-chip>
        <v-chip variant="outlined" class="rounded-btn text-subtitle-1" size="small" color="black">
          Налаштування
        </v-chip>
      </div>
      <div class="general-size info-align username-text-size">{{user?.people_?.surname}} {{user?.people_?.name}} </div>
      <div class="general-size info-align text-h5">{{user?.email}}</div>
    </div>

  </v-col>
</v-row>
</template>

<style scoped>

 .avatar-overlay {
   position: absolute;
   top: 100px;
   left: 60px; /*  відступ від лівого краю */
   transform: translate(-50%, -50%);
   z-index: 1; /* Підняти над content-zone */
 }

 .btn-zone{
   flex-direction: row;
   justify-content: flex-end;

 }
 .username-text-size{
   font-size: 30px!important;
   font-weight: 500;
   line-height: normal;
 }
 .content-zone{
   background-color: white;
   flex-direction:column;
   padding: 10px 10px 30px 30px;
   z-index: 0;
 }
 .general-size{
   display: flex;
   width: 100%;
   padding-top: 10px;
 }
 .gradient{
   background: linear-gradient(to bottom right, #ead9ff, #c3f2ff);
   max-height: 100px;
   min-height: 100px;
   position: relative;
   width: 100%;
 }

 .info-align{
   justify-content: flex-start;
 }
 .rounded-btn{
   border-radius: 20px!important;
   margin: 0 5px;

 }
</style>