<script setup lang="ts">
import {useArticleStore} from "@/stores/articleStore";
import {onMounted, ref} from "vue";
import ArticleItem from "@/components/ArticleItem.vue";
import NewArticleForm from "@/components/NewArticleForm.vue";
import MyLocalStorage from "@/services/myLocalStorage";

const articleStore = useArticleStore();

const showSelected = true;//показуємо на сторінці Збережене в кабінеті користувача
const showMenu = false; //меню не показуємо на сторінці Збережене в кабінеті користувача
const showBtnPublic = false; // ця кнопка буде відображатися тільки в кабінеті користувача
const peopleId=MyLocalStorage.getItem('peopleId');

//пагінація
const currentPage = ref(1); // Поточна сторінка
const onPageChange = () => {
  // Оновлення поточної сторінки при зміні
  console.log('currentPage =',currentPage.value)
  articleStore.getMySelectedArticleList(peopleId,currentPage.value-1); //список моїх обраних статей
};
onMounted(()=>{
  articleStore.getMySelectedArticleList(peopleId,0); //список моїх обраних статей
})






</script>

<template>
  <v-row class="justify-center">
    <v-col cols="12">
      <v-overlay :model-value="articleStore.isLoading"
                 class="align-center justify-center">
        <v-progress-circular
            indeterminate
            color="primary"
        ></v-progress-circular>
      </v-overlay>
      <div v-if="articleStore.cntRec==0" class="d-flex justify-center py-16 text-h4">
        Ви ще не додали до "Обраного" жодної статті :(
      </div>
      <ArticleItem v-else
          v-for="article in articleStore.articles"
          :key="article.id"
          :article="article"
          :show-selected="showSelected"
          :show-menu="showMenu"
          :show-btn-public="showBtnPublic"
      />
    </v-col>
  </v-row>
  <v-row v-if="articleStore.totalPage>0" class="justify-center">
    <v-col cols="8">
      <v-container>
        <v-pagination
            v-model="currentPage"
            class="my-4"
            :length="articleStore.totalPage"
            @update:model-value="onPageChange"
        ></v-pagination>
      </v-container>
    </v-col>

  </v-row>

</template>

<style scoped>
.filter-zone{
  display: flex;
  flex-direction: row;
  justify-content: flex-start;
  width: 100%;
}

</style>