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
let searchSrt = ref('');
const showSelected = ref(false);
const showMenu = false; //меню показуємо тільки в кабінеті користувача
const filterDoi = ref<number|null>(null);
const selectedTag = ref<Array<string>>([])//модель для фільтру Теги
let tags = ref<string|null>('');//склеєні теги для відправки запиту
const selectedYearStr = ref<string|null>(null);
let selectedYear = ref<number|null>(null);
const delimiters = ['#',','] //масив рядків, що будуть створювати новий тег при вводі



let page = 0;
onMounted(()=>{
  if (typeof search === "string"){
    searchSrt.value = search;
    articleStore.searchArticlesByParam(searchSrt.value, currentPage.value-1,selectedYear.value,
        filterDoi.value,sortedValue.value,tags.value);
    console.log('Pages=', articleStore.totalPage)
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
        searchSrt.value = search;
        selectedTag.value=[];
        tags.value='';
        selectedYear.value=null;
        filterDoi.value=null;
        articleStore.searchArticlesByParam(searchSrt.value, currentPage.value-1,selectedYear.value,
            filterDoi.value,sortedValue.value,tags.value);
      }

    }
);
function selectSortParam(){
  console.log("sortedValue=", sortedValue.value)
  articleStore.searchArticlesByParam(searchSrt.value, currentPage.value-1,selectedYear.value,
      filterDoi.value,sortedValue.value,tags.value);
}
//функції фільтрації
function tagFiltered(focused:boolean){ //по тегу
  console.log("tagFilteredFocused=",focused)
  if (!focused){
    selectedTag.value.map((item)=>{
      tags.value= tags.value+','+item.trim();
    })
    tags.value = tags.value? tags.value?.substring(1):'';
    console.log('tags =', tags.value)
    articleStore.searchArticlesByParam(searchSrt.value, currentPage.value-1,selectedYear.value,
        filterDoi.value,sortedValue.value,tags.value);
  }

}

function updateYear(focused:boolean){
  console.log("updateYearVal=", selectedYearStr.value)
  if(selectedYearStr.value!=null && selectedYearStr.value!='')
  {selectedYear.value = parseInt(selectedYearStr.value,10);
  console.log("selectedYear=", selectedYearStr.value)}
  console.log("updateYearFocused=",focused)
  articleStore.searchArticlesByParam(searchSrt.value, currentPage.value-1,selectedYear.value,
      filterDoi.value,sortedValue.value,tags.value);
}
function selectFilter(focused:boolean){//по типу статті (наукові, ненаукові)
  console.log("selectFilterFocused=",focused)
  if (!focused){
    console.log("filterDoi=",filterDoi.value)
   articleStore.searchArticlesByParam(searchSrt.value, currentPage.value-1,selectedYear.value,
        filterDoi.value,sortedValue.value,tags.value);

  }
}
function clearFilters(){ //скинути фільтри
  selectedTag.value=[];
  filterDoi.value=null;
  selectedYear.value=null;
  tags.value=null;

  articleStore.searchArticlesByParam(searchSrt.value, currentPage.value-1,selectedYear.value,
      filterDoi.value,sortedValue.value,tags.value);
}
//пагінація
const currentPage = ref(1); // Поточна сторінка
const onPageChange = () => {
  // Оновлення поточної сторінки при зміні
  console.log('currentPage =',currentPage.value)
};

</script>

<template>
  <v-container>
    <v-row class="justify-space-between pt-7">
      <v-overlay :model-value="articleStore.isLoading"
                 class="align-center justify-center">
        <v-progress-circular
            indeterminate
            color="primary"
        ></v-progress-circular>
      </v-overlay>
      <v-col cols="3">
        <v-combobox
            label="Теги"
            :items="articleStore.tagItems"
            :delimiters="delimiters"
            density="compact"
            v-model="selectedTag"
            multiple
            chips
            @update:focused="tagFiltered"
        ></v-combobox>
      </v-col>
      <v-col cols="3">
        <v-text-field
            label="Рік"
            density="compact"
            v-model="selectedYearStr"
            @update:focused="updateYear"
        ></v-text-field>
      </v-col>
      <v-col cols="3">
        <v-select
            v-model="filterDoi"
            density="compact"
            :items="articleStore.filterOptions"
            item-title="value"
            item-value="key"
            label="Тип"
            @update:focused="selectFilter"
        ></v-select>


      </v-col>
      <v-col cols="2">
        <div class="d-flex justify-end">
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
      <v-col cols="8"  md="6" sm="9">
          <div class="text-h5">Результати пошуку</div>
      </v-col>
      <v-col cols="2" md="3" sm="3">
        <v-select
            v-model="sortedValue"
            hint="Оберіть параметр сортування"
            :items="articleStore.sortedOptions"
            item-title="value"
            item-value="key"
            label="Сортувати"
            @update:modelValue= "selectSortParam"
        ></v-select>
      </v-col>
    </v-row>
    <v-row class="justify-center">
      <v-col cols="12" >
        <div v-if="articleStore.cntRec===0" class="mt-6">
          <span class="text-h3">За вашим запитом нічого не знайдено</span>
        </div>
        <div v-else>
          <ArticleItem
              v-for="article in articleStore.articles"
              :key="article.id"
              :show-selected="showSelected"
              :show-menu="showMenu"
              :article="article"
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
  </v-container>
</template>

<style scoped>
  .btn_clear{
    display: flex;
    flex-direction: column;
    justify-content: end;
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