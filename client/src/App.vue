<script setup lang="ts">
import {RouterView } from 'vue-router'
import MyToolbar from "@/components/MyToolbar.vue";
import MyNavigationDrawes from "@/components/MyNavigationDrawes.vue";
import {useAuthStore} from "@/stores/authStore";

const authStore = useAuthStore();
</script>

<template>
  <v-app>
    <v-app-bar>
      <MyToolbar/>
    </v-app-bar>
    <MyNavigationDrawes
        :user="authStore.authUser"
    />

    <v-main  style="height: 250px">
      <v-container fluid>
        <v-fade-transition>
          <RouterView />
        </v-fade-transition>

      </v-container>
    </v-main>
  </v-app>
</template>

<style scoped>

</style>
