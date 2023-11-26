<script setup lang="ts">
import {ref} from "vue";
import {useArticleStore} from "@/stores/articleStore";
import {useRoute, useRouter} from "vue-router";

const articleStore = useArticleStore();
const emits = defineEmits(['show'])
const drawer = ref(false);
const searchStr = ref('');
const router = useRouter();
function showSideBar(){
  drawer.value=!drawer.value;
  emits('show', drawer.value);
}
function onSearch(){
  router.push({ name: 'search_article', params: { search: searchStr.value } });
}
</script>

<template>
      <v-toolbar
          color="my-dark"
          class="pa-2"
          density="compact">
        <v-app-bar-nav-icon @click="showSideBar"></v-app-bar-nav-icon>
        <RouterLink to="/" >
          <v-toolbar-title class="text-decoration-underline">
            Доцент
          </v-toolbar-title>
        </RouterLink>

        <v-spacer></v-spacer>

        <v-text-field
            class="my-1 me-6"
            density="compact"
            variant="outlined"
            label="Пошук"
            append-inner-icon="mdi-magnify"
            single-line
            hide-details
            v-model="searchStr"
            @click:append-inner="onSearch"
        ></v-text-field>
        <v-btn
            class="mx-1"
            to="/new_article"
            variant="outlined"
            size="small">
          new_article
        </v-btn>
        <v-btn
            class="mx-1"
            variant="outlined"
            size="small">
          Обране
        </v-btn>
        <v-btn
            class="mx-1"
            to="/my_article"
            variant="outlined"
            size="small">
          Мої статті
        </v-btn>

        <v-btn
            class="mx-1"
            to="/login"
            variant="outlined"
            size="small"
        >
          Вхід
        </v-btn>
      </v-toolbar>

</template>

<style scoped>

</style>