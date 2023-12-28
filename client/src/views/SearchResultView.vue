<script setup lang="ts">

import ArticleItem from "@/components/ArticleItem.vue";
import {useArticleStore} from "@/stores/articleStore";
import type {ISelectedArticle} from "@/api/type";
import {computed, onMounted, ref, watch} from "vue";
import {useRoute} from "vue-router";
import MyLocalStorage from "@/services/myLocalStorage";

const sortedValue = ref<number>(0);

const articleStore = useArticleStore();
const route = useRoute();
const {search} = route.params;
const showSelected = ref(false);
const showMenu = false; //меню показуємо тільки в кабінеті користувача
const filterDoi = ref<number|null>(null);
const selectedTag = ref<Array<string>>([])//модель для фільтру Теги
const selectedYear = ref<number|null>(null);
const delimiters = ['#',','] //масив рядків, що будуть створювати новий тег при вводі

let page = 0;
onMounted(()=>{
  if (typeof search === "string"){
    articleStore.searchArticlesByParam(search, currentPage.value,selectedYear.value, filters,sortedValue.value);
  }
  const isLoginString = MyLocalStorage.getItem('isLogin');
  if (isLoginString!=null){
    showSelected.value = isLoginString;
  }
  //формуємо масив тегів
  articleStore.getScienceList();

})
watch(
    () => route.params,
    (params) => {
      const { search } = params;
      if (typeof search === 'string') {
        articleStore.searchArticlesByParam(search, currentPage.value,selectedYear.value, filters,sortedValue.value);
      }
      selectedTag.value=[];
      selectedYear.value=null;
      filterDoi.value=null;

    }
);
function selectSortParam(){
  console.log("sortedValue=", sortedValue)
}
//функції фільтрації
function tagFiltered(focused:boolean){ //по тегу
  if (!focused)
  console.log('selectedTag =', selectedTag.value)
 // articleStore.searchArticlesByParam()
}
let filters:Array<number> = [];
function selectFilter(focused:boolean){//по типу статті (наукові, ненаукові)
  if (!focused){
    console.log('selectFilter =', filterDoi.value);
    if (filterDoi.value) filters.push(filterDoi.value);
    if (typeof search === 'string') {
      articleStore.searchArticlesByParam(search, currentPage.value,selectedYear.value, filters,sortedValue.value);
    }
  }
}
function clearFilters(){ //скинути фільтри
  selectedTag.value=[];
  filterDoi.value=null;
  selectedYear.value=null;
}
//пагінація
const currentPage = ref(1); // Поточна сторінка
const onPageChange = () => {
  // Оновлення поточної сторінки при зміні
  console.log('currentPage =',currentPage.value)
};

</script>

<template>
  <v-row class="justify-center">
    <v-col cols="3">
      <v-combobox
          label="Теги"
          :items="articleStore.tagItems"
          :delimiters="delimiters"
          v-model="selectedTag"
          multiple
          chips
          @update:focused="tagFiltered"
      ></v-combobox>
    </v-col>
    <v-col cols="3">
      <v-text-field
          label="Рік"
      ></v-text-field>
    </v-col>
    <v-col cols="3">
      <v-select
          v-model="filterDoi"
          :items="articleStore.filterOptions"
          item-title="value"
          item-value="key"
          label="Тип"
          @update:focused="selectFilter"
      ></v-select>


    </v-col>
    <v-col cols="2">
     <div class="btn_clear">
       <span  @click="clearFilters">
          <u>Очистити</u>
       </span>
     </div>
    </v-col>
    <!--v-col cols="3">
      <v-combobox
          label="Мова"
          :items="['Російська', 'Українська', 'Англійська', 'Німецька', 'Французька', 'Іспанська']"
          variant="outlined"
      ></v-combobox>
    </v-col-->
  </v-row>
  <v-row class="justify-space-between">
    <v-col cols="8"  md="10" sm="12">
      <div class="d-flex justify-space-around">
        <div class="search-header">Результати пошуку</div>
        <v-select
            v-model="sortedValue"
            hint="Оберіть параметр сортування"
            :items="articleStore.sortedOptions"
            item-title="value"
            item-value="key"
            label="Сортувати"
            @update:modelValue= "selectSortParam"
        ></v-select>
      </div>
    </v-col>
  </v-row>
  <v-row class="justify-center">
    <v-col cols="8"  md="10" sm="12">
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
            :show-selected="showSelected"
            :show-menu="showMenu"
        />
      </div>

    </v-col>
  </v-row>
  <v-row class="justify-center">
    <v-pagination
        v-model="currentPage"
        :length="articleStore.totalPage"
        @update:model-value="onPageChange"
    ></v-pagination>
  </v-row>

</template>

<style scoped>
  .btn_clear{
    display: flex;
    flex-direction: column;
    justify-content: center;
    min-height: 75%;
  }
  .search-header{
    font-family: "Mariupol",serif;
    font-size: 28px;
    font-style: normal;
    font-weight: 400;
    line-height: normal;
  }
</style>