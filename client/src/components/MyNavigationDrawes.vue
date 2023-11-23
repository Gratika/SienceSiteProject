<script setup lang="ts">
  import type {IUser} from "@/api/type";
  import {ref} from "vue";
  import router from "@/router";

  const props = defineProps<{
    user:IUser|null,
    drawer:boolean
  }>();

  const userName=():string|undefined =>{
    if(props.user!=null){
      if (props.user.people_?.surname!=''){
        return props.user.people_?.surname;
      }else {
        if (props.user.people_?.name != '') {
          return props.user.people_?.name;
        }
      }
    }else return 'noname';

  }
  const userEmail = ():string|undefined =>{
    if(props.user!=null){
      if(props.user.email!=''){
        return props.user.email;
      }
    }else{
      return 'noname@email.com'
    }
  }
  /*function createNewArticle(){
    router.push('/new_article');
  }*/

</script>

<template>
  <!--v-card>
    <v-layout-->
      <v-navigation-drawer v-model="drawer">
        <v-list>
          <v-list-item
              prepend-avatar="https://randomuser.me/api/portraits/women/85.jpg"
              :title="userName()"
              :subtitle="userEmail()"
          ></v-list-item>
        </v-list>

        <v-divider></v-divider>

        <v-list density="compact" nav>
          <v-list-item prepend-icon="mdi-account" title="Мій профіль" value="profile"></v-list-item>
          <v-list-item prepend-icon="mdi-notebook" title="Мої статті" value="myArticle"></v-list-item>
          <v-list-item prepend-icon="mdi-pen" title="Опублікувати статтю" value="newArticle"></v-list-item>
          <v-list-item prepend-icon="mdi-star" title="Обране" value="starred"></v-list-item>
          <v-spacer></v-spacer>
          <v-list-item prepend-icon="mdi-exit-to-app" title="Вийти" value="exit"></v-list-item>
        </v-list>
      </v-navigation-drawer>

      <!--v-main style="height: 250px"></v-main-->
    <!--/v-layout>
  </v-card-->
</template>

<style scoped>

</style>