<script setup lang="ts">
  import {useArticleStore} from "@/stores/articleStore";
  import {onMounted, ref} from "vue";
  import ArticleItem from "@/components/ArticleItem.vue";
  import NewArticleForm from "@/components/NewArticleForm.vue";
  import MyLocalStorage from "@/services/myLocalStorage";
  import {useRouter} from "vue-router";

  const articleStore = useArticleStore();
  const showDialog = ref(false);
  const showEditBtn = true;
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
    <v-dialog
        v-model="showDialog"
        persistent
        width="1024"
    >
      <template v-slot:activator="{ props }">
        <v-btn
            fab
            dark
            small
            v-bind="props"
            color="primary"
            class="circular-btn"
        >
          <v-icon>mdi-plus</v-icon>
        </v-btn>
      </template>

      <NewArticleForm
          :scienceList=articleStore.sciences
          :scienceSectionList=articleStore.scientificSections
          @close="closeDialog"
      />

    </v-dialog>
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
          :show-edit="showEditBtn"
      />
    </v-col>
  </v-row>

</template>

<style scoped>
.circular-btn {
  position: fixed;
  top: 100px;
  right: 20px;
  box-shadow: 0px 0px 10px rgba(0, 0, 0, 0.2);
}
</style>