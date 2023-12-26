<script setup lang="ts">
import {useArticleStore} from "@/stores/articleStore";
import {onMounted, ref} from "vue";

import ArticleItem from "@/components/ArticleItem.vue";
import Heder from "@/components/Heder.vue";
import MyLocalStorage from "@/services/myLocalStorage";

const articleStore = useArticleStore();
const showSelected = ref(false);
const showMenu = false; //меню показуємо тільки в кабінеті користувача
onMounted(() => {
  const isLoginString = MyLocalStorage.getItem('isLogin');
  if (isLoginString!=null){
    showSelected.value = isLoginString;
  }
  articleStore.getNewArticleList();
  //articleStore.getPopularArticleList();

});

</script>

<template>
  <Heder/>
  <v-row class="justify-center">
    <v-col cols="12"  md="10" sm="12">
      <v-overlay :model-value="articleStore.isLoading"
               class="align-center justify-center">
      <v-progress-circular
          indeterminate
          color="primary"
      ></v-progress-circular>
    </v-overlay>
      <ArticleItem
          v-for="article in articleStore.newArticles"
          :key="article.id"
          :article="article"
          :show-selected="showSelected"
          :show-menu="showMenu"
      />
    </v-col>
  </v-row>

</template>

<style scoped>

</style>