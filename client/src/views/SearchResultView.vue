<script setup lang="ts">

import ArticleItem from "@/components/ArticleItem.vue";
import {useArticleStore} from "@/stores/articleStore";
import type {ISelectedArticle} from "@/api/type";
import {computed, onMounted, ref, watch} from "vue";
import {useRoute} from "vue-router";
import MyLocalStorage from "@/services/myLocalStorage";

const selectedValue = ref({key:'', value:''});

const articleStore = useArticleStore();
const route = useRoute();
const {search} = route.params;
const cntRec = ref(0);
const showSelected = ref(false);
const showMenu = false; //меню показуємо тільки в кабінеті користувача
const filterDoi = ref('');
const selectedTag = ref<Array<string>>([])//модель для фільтру Теги
const delimiters = ['#',','] //масив рядків, що будуть створювати новий тег при вводі
onMounted(()=>{
  if (typeof search === "string"){
  articleStore.searchArticlesByParam(search);
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
        articleStore.searchArticlesByParam(search);
      }
    }
);
function selectSortParam(){
  console.log("sortedValue=", selectedValue)
}
//функції фільтрації
function tagFiltered(){
  console.log('selectedTag =', selectedTag.value)
}
//функції фільтрації
function selectFilter(focused:boolean){
  console.log('focused=', focused)
  console.log('selectFilter =', filterDoi.value)
}
//пагінація
const currentPage = ref(1); // Поточна сторінка
const totalPages = 4; // Загальна кількість сторінок

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
          variant="outlined"
          @update:focused="tagFiltered"
      ></v-combobox>
    </v-col>
    <v-col cols="3">
      <v-select
          v-model="filterDoi"
          :items="articleStore.filterOptions"
          item-title="value"
          item-value="key"
          label="Науковість"
          single-line
          @update:focused="selectFilter"
      ></v-select>
      <!--v-combobox
          label="Рік"
          :items="['2018', '2019', '2020', '2021', '2022', '2023']"
          variant="outlined"
      ></v-combobox-->
    </v-col>
    <v-col cols="3">
      <v-combobox
          label="Рік"
          :items="['2018', '2019', '2020', '2021', '2022', '2023']"
          variant="outlined"
      ></v-combobox>
    </v-col>
    <v-col cols="3">
      <v-combobox
          label="Мова"
          :items="['Російська', 'Українська', 'Англійська', 'Німецька', 'Французька', 'Іспанська']"
          variant="outlined"
      ></v-combobox>
    </v-col>
  </v-row>
  <v-row class="justify-space-between">
    <v-col cols="8"  md="10" sm="12">
      <div class="d-flex justify-space-around">
        <h1>Результати пошуку:</h1>
        <v-select class="w-25"
                  v-model="selectedValue"
                  hint="Оберіть параметр сортування"
                  :items="articleStore.sortedOptions"
                  item-title="value"
                  item-value="key"
                  label="Сортувати"
                  single-line
                  @change="selectSortParam"
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
        :length="totalPages"
        @update:model-value="onPageChange"
    ></v-pagination>
  </v-row>

</template>

<style scoped>

</style>