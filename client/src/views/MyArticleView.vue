<script setup lang="ts">
  import {useArticleStore} from "@/stores/articleStore";
  import {onMounted, ref} from "vue";
  import ArticleItem from "@/components/ArticleItem.vue";
  import NewArticleForm from "@/components/NewArticleForm.vue";
  import MyLocalStorage from "@/services/myLocalStorage";
  import FilterForMyArticle from "@/components/FilterForMyArticle.vue";
  import {useRouter} from "vue-router";
  //import type {IArticle} from "@/api/type";

  const articleStore = useArticleStore();
  const router = useRouter();
 /* const props = defineProps({
    myArticles:{
      type: Array<IArticle>,
      default:[],
      required: true
    }
  })*/
  const showDialog = ref(false);
  const showSelected = false;//не показуємо в кабінеті користувача
  const showMenu = true; //меню показуємо тільки в кабінеті користувача
  const showBtnPublic = true; // ця кнопка буде відображатися тільки в кабінеті користувача
  const peopleId=MyLocalStorage.getItem('peopleId');

  let tags = ref<string|null>('');
  let sortParam = ref<number|null>(0)

  /*onMounted(() => {
    articleStore.getMyArticleList(peopleId); //список моїх статей
    articleStore.getScienceList(); //отримуємо список наукових сфер
    articleStore.getScienceSectionList(); //отримуємо список підкатегорій

  });*/

  function closeDialog(show:boolean, saveArticleId: string|undefined){
    showDialog.value=show;
    console.log('saveArticleId', saveArticleId);
    if(saveArticleId!==undefined)
        router.push({ name: 'edit_article', params: { id: saveArticleId } });
  }
  function tagChoose(data:Array<string>|undefined){ //по тегу
    tags.value='';
    if(data!==undefined){
      data.map((item) => {
        tags.value = tags.value + ',' + item.trim();
      })
      tags.value = tags.value ? tags.value?.substring(1) : '';
    }
    console.log('tags =', tags.value)
    articleStore.searchArticlesByParam(currentPage.value-1,undefined,undefined,
        undefined,sortParam.value,tags.value,peopleId);
  }
  function sortParamChoose(data:number){
    sortParam.value = data;
    console.log("sortedParam=", sortParam.value)
    articleStore.searchArticlesByParam(currentPage.value-1,undefined,undefined,
        undefined,sortParam.value,tags.value,peopleId);
  }

  //пагінація
  const currentPage = ref(1); // Поточна сторінка
  const onPageChange = () => {
    // Оновлення поточної сторінки при зміні
    console.log('currentPage =',currentPage.value)
    articleStore.searchArticlesByParam(currentPage.value-1,undefined,undefined,
        undefined,sortParam.value,tags.value,peopleId);
  };
</script>

<template>
  <!--відкрити діалог створення нової статті-->
  <v-row class="py-10 justify-space-between align-content-center">
      <v-col cols="6" class="ps-0">
        <FilterForMyArticle
            :sorted-options="articleStore.sortedOptions"
            :tag-items="articleStore.tagItems"
            @choose-sorted="sortParamChoose"
            @choose-tag="tagChoose"
        />

      </v-col>

    <v-col cols="4" class="d-flex justify-end">
      <div>
        <v-dialog
            v-model="showDialog"
            persistent
            width="1024"
        >
          <template v-slot:activator="{ props }">
            <v-btn
                v-bind="props"
                class="text-center text-h6"
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

</style>