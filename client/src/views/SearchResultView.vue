<script setup lang="ts">

import ArticleItem from "@/components/ArticleItem.vue";
import {useArticleStore} from "@/stores/articleStore";
import type {ISelectedArticle} from "@/api/type";
import {computed, onMounted, ref, watch} from "vue";
import {useRoute} from "vue-router";

const selectedValue = ref({key:'', value:''});
const sortedOptions = [
  {key:'1', value:'Спочатку з DUI'},
  {key:'2', value:'Спочатку з DUI'},
  {key:'3', value:'За рейтингом'},
  {key:'4', value:'Спочатку більш нові'},
  {key:'5', value:'Спочатку більш старі'}
]
const articleStore = useArticleStore();
const showEditBtn = false;
const route = useRoute();
const {search} = route.params;
const cntRec = ref(0);
onMounted(()=>{
  if (typeof search === "string"){
  articleStore.searchArticlesByParam(search);
  }
})
watch(
    () => route.params,
    (params) => {
      const { search } = params;
      if (typeof search === 'string') {
        articleStore.searchArticlesByParam(search);
      }
    }
);
function addArticleToFavorites(newFavorite:ISelectedArticle) {
  console.log('Отримано дані з дочірнього компонента:', newFavorite);
  articleStore.addToFavorites(newFavorite);
}
function selectSortParam(){
  console.log("sortedValue=", selectedValue)
}
</script>

<template>
<h1>Результати пошуку:</h1>
  <div class="d-flex justify-end">
    <v-select class="w-25"
        v-model="selectedValue"
        density="compact"
        hint="Оберіть параметр сортування"
        :items="sortedOptions"
        item-title="value"
        item-value="key"
        label="Оберіть параметр сортування"
        single-line
        @change="selectSortParam"
    ></v-select>
  </div>
  <v-row class="justify-center">
    <v-col cols="10"  md="10" sm="12">
      <v-overlay :model-value="articleStore.isLoading"
                 class="align-center justify-center">
        <v-progress-circular
            indeterminate
            color="primary"
        ></v-progress-circular>
      </v-overlay>
      <div v-if="articleStore.cntRec===0">
        <h5 >За вашим запитом нічого не знайдено</h5>
      </div>
      <div v-else>
        <ArticleItem
            v-for="article in articleStore.searchArticles"
            :key="article.id"
            :article="article"
            :show-edit="showEditBtn"
            @add_selected="addArticleToFavorites"
        />
      </div>

    </v-col>
  </v-row>
</template>

<style scoped>

</style>