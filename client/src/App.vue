<script setup lang="ts">
import MyToolbar from "@/components/MyToolbar.vue";
import Footer from "@/components/Footer.vue";
import {useRoute} from "vue-router";
import {computed} from "vue";
const route = useRoute();

// Обчислюємо поточний шлях
const isNotFoundPage = computed(() => {
  return route.name === 'nofFound';

});

const isDevelopmentPage = computed(() => {
  return route.name === 'developmentPage';
 });

const isLoginPage = computed(() => {
  return route.name === 'login';
});
</script>
<template>
  <v-app id="inspire">

    <v-app-bar v-if="!isNotFoundPage && !isDevelopmentPage">
      <MyToolbar/>
    </v-app-bar>

    <v-main>
       <RouterView />
    </v-main>

    <v-footer v-if="!isNotFoundPage && !isDevelopmentPage &&!isLoginPage" class="pa-0">
      <Footer/>
    </v-footer>

  </v-app>
</template>
<style>
.v-input .v-label {
  font-size: 1.2rem;
}
.v-field{
  font-size: 1.3rem;
}
</style>




