<script setup lang="ts">
import { ref } from 'vue'
import MyNavigationDrawes from "@/components/MyNavigationDrawes.vue";
import {useAuthStore} from "@/stores/authStore";
import MyToolbar from "@/components/MyToolbar.vue";

const authStore = useAuthStore();
const drawer = ref(false)
function showSideBar(show:boolean){
  drawer.value=show;
}
</script>
<template>
  <v-app id="inspire">
    <MyNavigationDrawes
        :user="authStore.authUser"
        :drawer="drawer"
    />

    <v-app-bar>
      <MyToolbar
      @show="showSideBar"
      />
    </v-app-bar>

    <v-main>
      <v-container fluid>
        <v-fade-transition>
          <RouterView />
        </v-fade-transition>

      </v-container>
    </v-main>
  </v-app>
</template>




