<script setup lang="ts">
import {useArticleStore} from "@/stores/articleStore";
import {onMounted, ref} from "vue";

import ArticleItem from "@/components/ArticleItem.vue";
import Heder from "@/components/Heder.vue";
import MyLocalStorage from "@/services/myLocalStorage";
import ArticleCarousels from "@/components/ArticleCarousels.vue";
import CategoryCard from "@/components/CategoryCard.vue";
import ScienceCarousels from "@/components/ScienceCarousels.vue";
import {useAuthStore} from "@/stores/authStore";
import type {IArticle} from "@/api/type";

const authStore = useAuthStore();
const articleStore = useArticleStore();
const showSelected = ref(false);
const showMenu = false; //меню показуємо тільки в кабінеті користувача
const showBtnPublic = false; // ця кнопка буде відображатися тільки в кабінеті користувача
const newArticleList = ref<Array<IArticle>>([])
onMounted(() => {
  const isLoginString = MyLocalStorage.getItem('isLogin');
  if (isLoginString!=null){
    showSelected.value = isLoginString;
  }
  articleStore.getNewArticleList(9).then((res)=>{
    if (res!==undefined) newArticleList.value=res;
  });
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
  <ArticleCarousels :articles="newArticleList"/>
  <ScienceCarousels :science="articleStore.sciences"/>
  <v-container>
    <v-row>
      <v-col cols="12">
        <div class="text-h4 font-weight-bold mb-6">
          Популярне
        </div>
      </v-col>
    </v-row>
    <v-row class="justify-center">
      <v-col cols="12"  md="12" sm="12">
        <ArticleItem
            v-for="article in articleStore.popularArticles"
            :key="article.id"
            :article="article"
            :show-selected="showSelected"
            :show-menu="showMenu"
            :show-btn-public="showBtnPublic"
        />
      </v-col>
    </v-row>

    <div class="footer-distance"></div>
  </v-container>

</template>

<style scoped>
.footer-distance{
  min-height: 100px;
}
</style>