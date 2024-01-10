<script setup lang="ts">
  import {useArticleStore} from "@/stores/articleStore";
  import {onMounted, ref} from "vue";
  import ArticleItem from "@/components/ArticleItem.vue";
  import NewArticleForm from "@/components/NewArticleForm.vue";
  import MyLocalStorage from "@/services/myLocalStorage";

  const articleStore = useArticleStore();
  const showDialog = ref(false);
  const showSelected = false;//не показуємо в кабінеті користувача
  const showMenu = true; //меню показуємо тільки в кабінеті користувача
  const peopleId=MyLocalStorage.getItem('peopleId');
  const selectedTag = ref<Array<string>>([])//модель для фільтру Теги
  const delimiters = ['#',','] //масив рядків, що будуть створювати новий тег при вводі

  const sortedValue = ref<number>(0);

  onMounted(() => {
    articleStore.getMyArticleList(peopleId); //список моїх статей
    articleStore.getScienceList(); //отримуємо список наукових сфер
    articleStore.getScienceSectionList(); //отримуємо список підкатегорій

  });

  function handleButtonClick(){

  }
  function closeDialog(show:boolean){
    showDialog.value=show;
  }
  function tagFiltered(focused:boolean){ //по тегу
    if (!focused)
      console.log('selectedTag =', selectedTag.value)
    // articleStore.searchArticlesByParam()
  }
  function selectSortParam(){
    console.log("sortedValue=", sortedValue)
  }
</script>

<template>
  <!--відкрити діалог створення нової статті-->
  <v-row class="py-10 justify-space-between align-content-center">
      <v-col cols="6" class="ps-0">
        <div class="d-flex">
          <v-combobox
              class="mx-3 w-50"
              label="Теги"
              :items="articleStore.tagItems"
              :delimiters="delimiters"
              v-model="selectedTag"
              multiple
              chips
              @update:focused="tagFiltered"
          ></v-combobox>
          <v-select
              class="mx-3 w-50"
              v-model="sortedValue"
              hint="Оберіть параметр сортування"
              :items="articleStore.sortedOptions"
              item-title="value"
              item-value="key"
              label="Впорядкувати"
              @update:modelValue= "selectSortParam"
          ></v-select>
        </div>

      </v-col>

    <v-col cols="4" class="d-flex justify-end pe-0">
      <div>
        <v-dialog
            v-model="showDialog"
            persistent
            width="1024"
        >
          <template v-slot:activator="{ props }">
            <v-btn
                v-bind="props"
                class="circular-btn text-center"
            >
              Нова стаття
            </v-btn>
          </template>

          <NewArticleForm
              :scienceList=articleStore.sciences
              :scienceSectionList=articleStore.scientificSections
              @close="closeDialog"
          />

        </v-dialog>
      </div>
    </v-col>

  </v-row>

  <v-row class="justify-center">
    <v-col cols="12">
      <v-overlay :model-value="articleStore.isLoading"
                 class="align-center justify-center">
        <v-progress-circular
            indeterminate
            color="primary"
        ></v-progress-circular>
      </v-overlay>
      <ArticleItem
          v-for="article in articleStore.articles"
          :key="article.id"
          :article="article"
          :show-selected="showSelected"
          :show-menu="showMenu"
      />
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