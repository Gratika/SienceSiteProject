<script setup lang="ts">
import {useArticleStore} from "@/stores/articleStore";
import {onMounted, ref} from "vue";

import ArticleItem from "@/components/ArticleItem.vue";
import type {ISelectedArticle} from "@/api/type";

const articleStore = useArticleStore();
const showEditBtn = false;
onMounted(() => {
  articleStore.getArticleList();

});
function addArticleToFavorites(newFavorite:ISelectedArticle) {
  console.log('Отримано дані з дочірнього компонента:', newFavorite);
  articleStore.addToFavorites(newFavorite);
}
</script>

<template>
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
          v-for="article in articleStore.articles"
          :key="article.id"
          :article="article"
          :show-edit="showEditBtn"
          @add_selected="addArticleToFavorites"
      />
    </v-col>
  </v-row>

</template>

<style scoped>

</style>