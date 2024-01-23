<script setup lang="ts">
import type {IArticle} from "@/api/type";
import {useRouter} from "vue-router";
import {useArticleStore} from "@/stores/articleStore";
import moment from "moment/moment";
import {boolean} from "zod";
import {computed, ref} from "vue";
import Like from "@/components/Like.vue";

//пропси від батьківського елементу
const props = defineProps<{
  article:IArticle,
  showSelected:boolean,
  showMenu:boolean,
  showBtnPublic:boolean
}>()
const isActive = ref<boolean>(props.article.isActive);
const setReaction = computed(()=>{
  return props.article.reaction !== null;
})
const borderColor = computed(()=> {
  if (isActive.value)
    return '#2A3759'
  else
    return '#778FD2';
})
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
        //console.log('surname=',props.article.author_.surname)
        return (props.article.author_.surname +' '+ props.article.author_.name).trim();
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
function publicationArticle(){
  let article_id= props.article?.id as string;
  articleStore.publicationArticle(article_id).then(()=>{
    isActive.value=true;
  });
}
function formatDate(date: null | string): string {
  //console.log('date=',date)
 // console.log('typeof date=',typeof date)
  if (date == null) return '';
  const formattedDate: Date = new Date(date);
  return (moment(formattedDate)).format('DD.MM.YYYY HH:mm')
}


</script>

<template>

  <v-card
      class="ma-4 pa-5 left-border"
      :style="{ 'border-left-color': borderColor }"
      variant="elevated"
  >

      <v-row>
        <v-col cols="11">
          <v-card-text
              class="card-title-size font-weight-bold myClickableObject"
              @click="readArticle">
            {{ props.article.title }}
          </v-card-text>
        </v-col>
        <v-col cols="1">
          <div v-if="props.showSelected">
            <v-icon
                class="flex-grow-1"
                @click="changeArticleSelect"
            >
              <svg
                  xmlns="http://www.w3.org/2000/svg"
                  width="40"
                  height="40"
                  viewBox="0 0 40 40"
                  :fill="props.article.selected ? '#778FD2' : '#FFFFFF'"
                  stroke="#778FD2"
                  stroke-width="2"
                  stroke-linecap="round"
                  stroke-linejoin="round"
              >
                <path d="M30.8574 37.6429L20.0002 26.7857L9.14307 37.6429V5.07146C9.14307 4.35158 9.42904 3.6612 9.93805 3.15217C10.4471 2.64315 11.1375 2.35718 11.8574 2.35718H28.1431C28.8629 2.35718 29.5534 2.64315 30.0623 3.15217C30.5713 3.6612 30.8574 4.35158 30.8574 5.07146V37.6429Z" />
              </svg>
            </v-icon>

          </div>
          <div v-else-if="props.showMenu" class="text-center">
            <v-menu :location="location">
              <template v-slot:activator="{ props }">
                <v-btn
                    icon="mdi-dots-horizontal"
                    variant="text"
                    class="style-btn-article-card flex-grow-1"
                    v-bind="props"
                />


              </template>

              <v-list color="black">
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


    <v-card-text>
      <div class="card-text-size font-weight-medium">
        <div>
         Автор: {{ author_() }}
        </div>
        <div class="my-1">
        Дата: {{ formatDate(props.article.date_created)}}
        </div>
        <div>
          Мова: українська
        </div>
      </div>
    </v-card-text>
    <v-card-actions>
      <v-row>
        <v-col cols="11" md="10">
          <v-chip
              v-for="(item, index) in props.article.tagItems"
              :key="index"
              class="ma-2 card-chips-text-size font-weight-bold"
              color="primary-darken-1"
              variant="flat"
          >
            {{ item }}
          </v-chip>
        </v-col>
        <v-col cols="1" md="2">
          <div v-if="!isActive && showBtnPublic" class="d-flex justify-end">
            <v-chip variant="outlined" class="rounded-btn card-chips-text-size" >
              Чернетка
            </v-chip>
          </div>
          <div v-else class="d-flex justify-end">
            <Like :is-selected="setReaction"/>
            <span class="ms-4">{{props.article.countLike}}</span>
          </div>


        </v-col>
      </v-row>

    </v-card-actions>
  </v-card>
</template>

<style scoped>
.card-text-size{
  font-size: 22px!important;
  line-height: normal;
}

.card-title-size{
  font-size: 32px!important;
  line-height: 1.5rem;
}
.card-chips-text-size{
  font-size: 18px!important;
}
.myClickableObject {
  cursor: pointer;
}
.left-border{
  border-left-width: 21px;
 /* border-left-color: #2A3759;*/
  border-radius: 5px;
  box-shadow: 0 0 15px 0 rgba(0, 0, 0, 0.25);
}
.rounded-btn{
  border-radius: 20px!important;
  margin: 0 5px;
}
.style-btn-article-card{
  border-radius: 50%!important;
}
</style>