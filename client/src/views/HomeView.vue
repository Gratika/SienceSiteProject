<script setup lang="ts">
import {useArticleStore} from "@/stores/articleStore";
import {onMounted, ref} from "vue";

import ArticleItem from "@/components/ArticleItem.vue";
import Heder from "@/components/Heder.vue";
import MyLocalStorage from "@/services/myLocalStorage";
import ArticleCarousels from "@/components/ArticleCarousels.vue";

const articleStore = useArticleStore();
const showSelected = ref(false);
const showMenu = false; //меню показуємо тільки в кабінеті користувача
onMounted(() => {
  const isLoginString = MyLocalStorage.getItem('isLogin');
  if (isLoginString!=null){
    showSelected.value = isLoginString;
  }
  articleStore.getNewArticleList();
  articleStore.getPopularArticleList(9);

});

</script>

<template>
  <v-container>
    <Heder/>
    <v-row class="justify-center">
      <v-col cols="12"  md="12" sm="12">
        <v-overlay :model-value="articleStore.isLoading"
                   class="align-center justify-center">
          <v-progress-circular
              indeterminate
              color="primary"
          ></v-progress-circular>
        </v-overlay>
      </v-col>
    </v-row>
  </v-container>
  <ArticleCarousels :articles="articleStore.articles"/>
  <v-container>
    <v-row class="justify-center">
      <v-col cols="12"  md="12" sm="12">
        <ArticleItem
            v-for="article in articleStore.newArticles"
            :key="article.id"
            :article="article"
            :show-selected="showSelected"
            :show-menu="showMenu"
        />
      </v-col>
    </v-row>
  </v-container>

</template>

<style scoped>

</style>