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
const {scienceId} = route.params;
let scienceIdSrt = ref('');

const showSelected = ref(false);
const showMenu = false; //меню показуємо тільки в кабінеті користувача
const showBtnPublic = false; // ця кнопка буде відображатися тільки в кабінеті користувача
const filterDoi = ref<number|null>(null);
const selectedTag = ref<Array<string>>([])//модель для фільтру Теги
let tags = ref<string|null>(null);//склеєні теги для відправки запиту
let scienceSectionId = ref<string|null>('');

const delimiters = ['#',','] //масив рядків, що будуть створювати новий тег при вводі



let page = 0;
onMounted(()=>{
  if (typeof scienceId === "string"){
    scienceIdSrt.value = scienceId;
    articleStore.searchArticlesByParam( currentPage.value-1,undefined, /*selectedYear.value*/undefined,
        undefined,undefined,undefined, undefined,scienceIdSrt.value);
    articleStore.getScienceTheoryByScienceId(scienceIdSrt.value);// перелік категорій певної наукової сфери

  }
  const isLoginString = MyLocalStorage.getItem('isLogin');
  if (isLoginString!=null){
    showSelected.value = isLoginString;
  }
  //формуємо масив тегів
  articleStore.getScienceList();
  window.scroll(0,0);
})

function selectSortParam(){
  console.log("sortedValue=", sortedValue.value)
  articleStore.searchArticlesByParam( currentPage.value-1,undefined, /*selectedYear.value*/undefined,
      filterDoi.value,sortedValue.value,tags.value, undefined,scienceIdSrt.value, scienceSectionId.value);
}
//функції фільтрації
function tagFiltered(focused:boolean){ //по тегу
  //console.log("tagFilteredFocused=",focused)
  if (!focused){
    if (selectedTag.value.length==0) tags.value=null;
    selectedTag.value.map((item)=>{
      if (tags.value==null) tags.value=item.trim();
      else tags.value= tags.value+','+item.trim();
    })
    tags.value = tags.value? tags.value?.substring(1):null;
    //console.log('tags =', tags.value)
    articleStore.searchArticlesByParam( currentPage.value-1,undefined, /*selectedYear.value*/undefined,
        filterDoi.value,sortedValue.value,tags.value, undefined,scienceIdSrt.value, scienceSectionId.value);
  }

}
function setScienceSection(){
  console.log("update=", scienceSectionId.value);
  articleStore.searchArticlesByParam( currentPage.value-1,undefined, /*selectedYear.value*/undefined,
      filterDoi.value,sortedValue.value,tags.value, undefined,scienceIdSrt.value, scienceSectionId.value);
}

function selectFilter(focused:boolean){//по типу статті (наукові, ненаукові)
  //console.log("selectFilterFocused=",focused)
  if (!focused){
    //console.log("filterDoi=",filterDoi.value)
    articleStore.searchArticlesByParam( currentPage.value-1,undefined, /*selectedYear.value*/undefined,
        filterDoi.value,sortedValue.value,tags.value, undefined,scienceIdSrt.value, scienceSectionId.value);

  }
}
function clearFilters(){ //скинути фільтри
  selectedTag.value=[];
  filterDoi.value=null;
  //selectedYear.value=null;
  scienceSectionId.value=null;
  tags.value=null;

  articleStore.searchArticlesByParam( currentPage.value-1,undefined, /*selectedYear.value*/undefined,
      filterDoi.value,sortedValue.value,tags.value, undefined,scienceIdSrt.value, scienceSectionId.value);
}
//пагінація
const currentPage = ref(1); // Поточна сторінка
const onPageChange = () => {
  // Оновлення поточної сторінки при зміні
  console.log('currentPage =',currentPage.value)
  articleStore.searchArticlesByParam( currentPage.value-1,undefined, /*selectedYear.value*/undefined,
      filterDoi.value,sortedValue.value,tags.value, undefined,scienceIdSrt.value, scienceSectionId.value);
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
        <v-autocomplete
            density="compact"
            item-title="name"
            item-value="id"
            :items="articleStore.scientificSections"
            label="Розділ"
            no-data-text="Розділів не знайдено"
            variant="outlined"
            v-model="scienceSectionId"
            @update:modelValue="setScienceSection"
        ></v-autocomplete>

      </v-col>
      <v-col cols="4">
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
        <v-select
            v-model="filterDoi"
            :items="articleStore.filterOptions"
            item-title="value"
            item-value="key"
            label="Тип"
            @update:focused="selectFilter"
        ></v-select>


      </v-col>
      <v-col cols="1">
        <div class="d-flex justify-end">
       <span  @click="clearFilters" class="text-h6">
          <u>Очистити</u>
       </span>
        </div>
      </v-col>

    </v-row>
    <v-row class="justify-space-between">
      <v-col cols="8"  md="6" sm="9" class="pb-0 mt-3">
        <div class="text-h4">Статті в категорії </div>
      </v-col>
      <v-col cols="2" md="3" sm="3" class="pb-0 mt-3">
        <v-select
            density="compact"
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
        <div v-if="articleStore.cntRec===0" class="mt-6 not-found">
          <span class="text-h3">За вашим запитом нічого не знайдено</span>
        </div>
        <div v-else>
          <ArticleItem
              v-for="article in articleStore.articles"
              :key="article.id"
              :show-selected="showSelected"
              :show-menu="showMenu"
              :article="article"
              :show-btn-public="showBtnPublic"
          />
        </div>

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
    <v-row>
      <div class="footer-distance"></div>
    </v-row>
  </v-container>
</template>

<style scoped>
.footer-distance{
  min-height: 100px;
}
.not-found{
  align-items: center;
  height: 350px;
  display: flex;
  justify-content: center;
}

</style>