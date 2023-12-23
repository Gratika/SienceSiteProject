<script setup lang="ts">
  import {useArticleStore} from "@/stores/articleStore";
  import {onMounted, ref} from "vue";
  import ArticleItem from "@/components/ArticleItem.vue";
  import NewArticleForm from "@/components/NewArticleForm.vue";
  import MyLocalStorage from "@/services/myLocalStorage";
  import {useRouter} from "vue-router";
  import DateComponent from "@/components/DateComponent.vue";

  const articleStore = useArticleStore();
  const showDialog = ref(false);
  const peopleId=MyLocalStorage.getItem('peopleId');
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
</script>

<template>
  <!--відкрити діалог створення нової статті-->
  <v-row justify="center">
    <v-col cols="2">
        <v-combobox
            label="За тегом"
            :items="['California', 'Colorado', 'Florida', 'Georgia', 'Texas', 'Wyoming']"
            variant="outlined"
        ></v-combobox>
    </v-col>
    <v-col cols="2">
    <v-combobox
            label="За датою публікації"
            :items="['2018', '2019', '2020', '2021', '2022', '2023']"
            variant="outlined"
        ></v-combobox>
    </v-col>
    <v-col cols="2">
     <DateComponent/>
    </v-col>
    <v-col cols="4">
      <div>
        <v-dialog
            v-model="showDialog"
            persistent
            width="1024"
        >
          <template v-slot:activator="{ props }">
            <v-btn
                variant="outlined"
                v-bind="props"
                color="primary"
                class="circular-btn"
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
    <v-col cols="12"  md="10" sm="12">
      <v-overlay :model-value="articleStore.isLoading"
                 class="align-center justify-center">
        <v-progress-circular
            indeterminate
            color="primary"
        ></v-progress-circular>
      </v-overlay>
      <ArticleItem
          v-for="article in articleStore.myArticles"
          :key="article.id"
          :article="article"
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