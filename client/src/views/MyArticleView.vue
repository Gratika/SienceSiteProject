<script setup lang="ts">
  import {useArticleStore} from "@/stores/articleStore";
  import {onMounted} from "vue";
  import ArticleItem from "@/components/ArticleItem.vue";

  const articleStore = useArticleStore();
  const showEditBtn = true;
  onMounted(() => {
    //articleStore.getMyArticleList();
    articleStore.getArticleList(); //тимчасово для перевірки
  });
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
      />
    </v-col>
  </v-row>

</template>

<style scoped>

</style>