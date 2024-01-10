<script setup lang="ts">
import {useArticleStore} from "@/stores/articleStore";
import {onMounted, ref} from "vue";

import ArticleItem from "@/components/ArticleItem.vue";
import Heder from "@/components/Heder.vue";
import MyLocalStorage from "@/services/myLocalStorage";
import ArticleCarousels from "@/components/ArticleCarousels.vue";
import CategoryCard from "@/components/CategoryCard.vue";
import ScienceCarousels from "@/components/ScienceCarousels.vue";

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
  articleStore.getScienceList();

});

</script>

<template>
  <v-container>
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
  <Heder/>
  <ArticleCarousels :articles="articleStore.newArticles"/>
  <ScienceCarousels :science="articleStore.sciences"/>
  <v-container>
    <v-row>
      <v-col cols="12">
        <div class="sub-title mb-6">
          Популярне
        </div>
      </v-col>
    </v-row>
    <v-row class="justify-center">
      <v-col cols="12"  md="12" sm="12">
        <ArticleItem
            v-for="article in articleStore.articles"
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
.sub-title{
  color: #000;
  font-family: Mariupol;
  font-size: 38px;
  font-style: normal;
  font-weight: 700;
  line-height: normal;
}
</style>