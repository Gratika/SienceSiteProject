<script setup lang="ts">
import type {IArticle} from "@/api/type";
import {useRouter} from "vue-router";
import {useArticleStore} from "@/stores/articleStore";

//пропси від батьківського елементу
const props = defineProps<{
  article:IArticle,
  showSelected:boolean,
  showMenu:boolean
}>()
const router = useRouter();
const articleStore = useArticleStore();
// меню
const location= 'top'; //позиція меню
const menuItems = [
  {key:'edit', icon:'mdi-pencil', title: 'Редагувати'},
  {key:'del', icon:'mdi-delete', title: 'Видалити'}
]
function handleAction(action: string){
  switch (action){
    case 'edit': editArticle(); break;
    case 'del': deleteArticle();break;
    default: console.log('not action')
  }
}

//функція для виводу автора
const author_=():string|undefined=>{
  if( props.article.author_ !=null){
      if(props.article.author_.surname!=null ){
        console.log('surname=',props.article.author_.surname)
        return props.article.author_.surname +' '+ props.article.author_.name;
      }

  }else{
    return 'unanimous'
  }
}

//для налаштування переходу на сторінку редагування статті
function editArticle(){
  router.push({ name: 'edit_article', params: { id: props.article.id } });

}
//для налаштування переходу на сторінку перегляду статті
function readArticle(){
  router.push({ name: 'read_article', params: { id: props.article.id } });

}
//вилучення статті
function deleteArticle(){
  articleStore.deleteArticle(props.article);
}
//додати статтю до обраного
function changeArticleSelect(){
  let article_id= props.article?.id as string;
  articleStore.addToFavorites(article_id);
}


</script>

<template>

  <v-card class="ma-4 pa-5 left-border" variant="elevated">
    <v-card-title class="d-flex">
      <v-row>
        <v-col cols="11">
          <span class="d-inline font-weight-bold" @click="readArticle">{{ props.article.title }}</span>
        </v-col>
        <v-col cols="1">
          <div v-if="props.showSelected">
            <v-btn icon @click="changeArticleSelect">
              <v-icon>mdi-bookmark-outline</v-icon>
            </v-btn>
          </div>
          <div v-else-if="props.showMenu" class="text-center">
            <v-menu :location="location">
              <template v-slot:activator="{ props }">
                <v-btn icon v-bind="props">
                  <v-icon>mdi-dots-horizontal</v-icon>
                </v-btn>
              </template>

              <v-list>
                <v-list-item
                    v-for="(item) in menuItems"
                    :key=item.key
                    @click="handleAction(item.key)"
                >
                  <v-list-item-title>
                    <v-btn :prepend-icon="item.icon" variant="text">
                      {{ item.title }}
                    </v-btn>

                  </v-list-item-title>
                </v-list-item>
              </v-list>
            </v-menu>
          </div>
        </v-col>
      </v-row>
    </v-card-title>

    <v-card-text class="headline">
      <div>
        <div>
          Автор: {{ author_() }}
        </div>
        <div>
          Дата: {{ props.article.date_create?.toDateString()}}
        </div>
        <div>
          Мова:
        </div>
      </div>
    </v-card-text>
    <v-card-actions>
      <v-row>
        <v-col cols="11">
          <v-chip
              v-for="(item, index) in props.article.tagItems"
              :key="index"
              class="ma-2"
              color="primary-darken-1"
              variant="flat"
          >
            {{ item }}
          </v-chip>
        </v-col>
        <v-col cols="1">
          <v-btn icon>
            <v-icon>mdi-thumb-up-outline</v-icon>
          </v-btn>
        </v-col>
      </v-row>

    </v-card-actions>
  </v-card>
</template>

<style scoped>
.left-border{
  border-left-width: 5px;
  border-left-color: #2A3759
}
</style>