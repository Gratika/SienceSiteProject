<script setup lang="ts">
import type {IArticle, ISelectedArticle} from "@/api/type";
import {useAuthStore} from "@/stores/authStore";
import router from "@/router";
//пропси від батьківського елементу
const props = defineProps<{
  article:IArticle|null,
  showEdit:boolean
}>()
const authStore = useAuthStore();
//функція для виводу автора
const author_=():string|undefined=>{
  if(props.article != null && props.article.author_ !=null){
      if(props.article.author_.firstname!=null){
        return props.article.author_.firstname +' '+ props.article.author_.name;
      }else{
          if (props.article.author_.email!=null){
            return props.article.author_.email;
          }

      }

  }else{
    return 'unanimous'
  }
}
//дані, які будемо надсилати в батьківський елемент
const emits = defineEmits(['add_selected'])
function addArticleSelect(){
  let selectedArticle: ISelectedArticle = {
    id:null,
    article_id: props.article?.id as number,
    user_id: authStore.getUserId as number,
    Date_view: null
  };
  emits('add_selected', selectedArticle);
}
function editArticle(){
  router.push('/new_article')
}
</script>

<template>

  <v-card class="ma-4 pa-5" >
    <v-card-title class="d-flex">
      <span class="d-inline font-weight-bold">{{ props.article?.title }}</span>
      <v-spacer></v-spacer>
      <small class="d-inline ">
        <v-tooltip location="top center" origin="end center">
        <template v-slot:activator="{ props }">
          <v-icon  v-bind="props" class="mr-1">mdi-eye</v-icon>
        </template>
          <div>Кількість переглядів</div>
        </v-tooltip>
          <span class="subheading mr-2">{{ props.article?.views }}</span>
      </small>


    </v-card-title>

    <v-card-text class="headline">
      {{ article?.text }}
    </v-card-text>

    <v-card-actions>
      <span>
        Автор: {{ author_() }}
      </span>
      <v-spacer></v-spacer>
      <span v-if="props.showEdit">
         <v-tooltip location="top center" origin="end center">
        <template v-slot:activator="{ props }">
          <v-btn icon v-bind="props" @click="editArticle">
            <v-icon>mdi-pen</v-icon>
          </v-btn>
        </template>
        <div>Редагувати</div>
      </v-tooltip>
      </span>

      <v-tooltip location="top center" origin="end center">
        <template v-slot:activator="{ props }">
          <v-btn icon v-bind="props" @click="addArticleSelect">
            <v-icon>mdi-star-outline</v-icon>
          </v-btn>
        </template>
        <div>Додати в обране</div>
      </v-tooltip>

    </v-card-actions>
  </v-card>
</template>

<style scoped>

</style>