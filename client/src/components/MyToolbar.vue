<script setup lang="ts">
import {onMounted, ref} from "vue";
import {useRouter} from "vue-router";
import MyLocalStorage from "@/services/myLocalStorage";


const router = useRouter();
const showLogin = ref(true);

onMounted(()=>{
  showLogin.value = MyLocalStorage.getItem('isLogin')===false;

})
</script>

<template>
      <v-toolbar
          class="pa-2"
          density="compact">
        <div class="w-50 d-inline-flex justify-center flex-wrap flex-row">
          <RouterLink to="/" >
            <v-toolbar-title class="text-decoration-underline">
              SciFindHub
            </v-toolbar-title>
          </RouterLink>
        </div>

        <div class="w-50 d-inline-flex d-inline-flex justify-center flex-wrap flex-row">
          <v-btn
              class="mx-1"
              prepend-icon="mdi-magnify"
              variant="text"
              size="small">
            Пошук
          </v-btn>
          <v-btn
              class="mx-1"
              variant="text"
              size="small"
              disabled
          >
            Блог
          </v-btn>
          <v-btn
              class="mx-1"
              variant="text"
              size="small">
           Про нас
          </v-btn>

          <v-btn v-if="showLogin"
              class="mx-1"
              to="/login"
              variant="outlined"
              size="small"
          >
            Вхід
          </v-btn>
          <v-avatar v-else image="avatar.png" size="x-small"></v-avatar>
        </div>


      </v-toolbar>

</template>

<style scoped>
v-toolbar{
  max-height: 100px;
}


</style>