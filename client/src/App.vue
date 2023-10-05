<script setup lang="ts">
import { RouterLink, RouterView } from 'vue-router'
import MyToolbar from "@/components/MyToolbar.vue";
</script>

<template>
  <v-app>
    <v-app-bar>
      <MyToolbar/>
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

<style scoped>

</style>
