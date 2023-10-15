<script setup lang="ts">
import {useArticleStore} from "@/stores/articleStore";
import { onMounted} from "vue";

import ArticleItem from "@/components/ArticleItem.vue";

const articleStore = useArticleStore();
let isLoading = true;
onMounted(() => {
  articleStore.getArticleList()
      .then(()=> {
        isLoading = false;
      })

});
</script>

<template>
  <v-row class="justify-center">
    <v-col cols="12">
      <v-overlay :model-value="isLoading"
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
      />
    </v-col>
  </v-row>

</template>

<style scoped>

</style>