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

const title = "Останні опубліковані статті";
const articleStore = useArticleStore();
const showSelected = ref(false);
const showMenu = false; //меню показуємо тільки в кабінеті користувача
const showBtnPublic = false; // ця кнопка буде відображатися тільки в кабінеті користувача
const page = ref(0);
const pointer = ref<Element|undefined>();
const newArticleList = ref<Array<IArticle>>([])
onMounted(() => {
  window.scroll(0,0);
  const isLoginString = MyLocalStorage.getItem('isLogin');
  if (isLoginString!=null){
    showSelected.value = isLoginString;
  }
  articleStore.getNewArticleList(page.value).then((res)=>{
    if (res!==undefined) newArticleList.value=res;
  });
  articleStore.getPopularArticleList(page.value);
  articleStore.getScienceList();
  //вказуємо елемент за яким потрібно слідкувати
  if(pointer.value!==undefined)  observer.observe(pointer.value)
});
//відслідковуємо видимість нашого pointer для виконання нового запиту до сервера
//для реалізації нескінченної стрічки
const options = {
  rootMargin: "0px",
  threshold: 1.0,
};
const callback : IntersectionObserverCallback = (entries, observer) => {

  if(entries[0].isIntersecting && (page.value+1<articleStore.totalPage)){
    page.value++;
    articleStore.getMorePopularArticle(page.value);
  }
}
const observer = new IntersectionObserver(callback, options);
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
  <div class="w-100 carousel-color">
    <ArticleCarousels :articles="newArticleList" :title="title"/>
  </div>
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
    <div ref="pointer" class="pointer"></div>

    <div class="footer-distance"></div>
  </v-container>

</template>

<style scoped>
.carousel-color{
  background-color: #E2E2E2;
}
.footer-distance{
  min-height: 100px;
}
.pointer{
  height: 5px;
}
</style>