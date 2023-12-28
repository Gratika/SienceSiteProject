<script setup lang="ts">
import MyToolbar from "@/components/MyToolbar.vue";


</script>
<template>
  <v-app id="inspire">

    <v-app-bar class="app-bar">
      <MyToolbar/>
    </v-app-bar>

    <v-main>
      <v-container>
          <RouterView />

      </v-container>
    </v-main>
  </v-app>
</template>
<style scoped>
</style>




