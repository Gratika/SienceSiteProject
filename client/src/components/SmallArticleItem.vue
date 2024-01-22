<script setup lang="ts">
import type {IArticle} from "@/api/type";
import {useRouter} from "vue-router";
import {useArticleStore} from "@/stores/articleStore";
import moment from 'moment';
import {computed} from "vue";
import Like from "@/components/Like.vue";

//пропси від батьківського елементу
const props = defineProps<{
  article:IArticle
}>();
const setReaction = computed(()=>{
  return props.article.reaction !== null;
})
const router = useRouter();


//функція для виводу автора
const author_=():string|undefined=>{
  if( props.article.author_ !=null){
    if(props.article.author_.surname!=null ){
      //console.log('surname=',props.article.author_.surname)
      return props.article.author_.surname +' '+ props.article.author_.name;
    }

  }else{
    return 'unanimous'
  }
}
function formatDate(date: null | string): string {
  if (date == null) return '';
  const formattedDate: Date = new Date(date);
  return (moment(formattedDate)).format('DD.MM.YYYY HH:mm')
}


//для налаштування переходу на сторінку перегляду статті
function readArticle(){
  router.push({ name: 'read_article', params: { id: props.article.id } });
}
</script>

<template>
  <div class="card-box">
    <v-card
        class="left-border pt-6 pb-3 ps-4 pe-5 d-flex flex-column justify-space-between"
        variant="elevated"
        min-width="300"
        width="350"
        height="300"
        @click="readArticle"
    >
      <v-card-text
          class="pa-0 my-auto text-h5 font-weight-medium"
          style="line-height: 1.5rem"
      >
        {{ props.article.title }}
      </v-card-text>
      <v-card-actions class=" flex-grow-1 flex-column card-align pa-0 my-auto">
        <div class="my-auto card-text-size font-weight-medium" >
          <div>
            Автор: {{ author_() }}
          </div>
          <div class="my-1">
            Дата: {{ formatDate(props.article.date_created) }}
          </div>
          <div>
            Мова: українська
          </div>
        </div>
        <div class="wrapper">
          <div class="my-auto d-flex">
            <v-chip
                v-for="(item, index) in props.article.tagItems.slice(0,2)"
                :key="index"
                class="text-subtitle-2 mt-1 me-4 d-flex font-weight-bold"
            >
              {{ item }}
            </v-chip>
          </div>
        </div>

        <div class=" d-flex flex-row justify-end mt-2">
          <Like :is-selected="setReaction"/>
          <span class="d-inline text-subtitle-2 font-weight-bold ms-3">{{article.countLike}}</span>
        </div>


      </v-card-actions>
    </v-card>
  </div>


</template>

<style scoped>
.card-box{
  background-color: transparent;
  height: 300px;
  margin: 0 10px;
  padding: 0 10px;
  width: 390px;
}
.card-text-size{
  font-size: 18px!important;
  line-height: normal;
}
.left-border{
  border-left-width: 21px;
  border-left-color: #2A3759;
  border-radius: 5px;
  box-shadow: 0 0 15px 0 rgba(0, 0, 0, 0.25);
}
.card-align{
  align-items: normal!important;
}
.wrapper{
  overflow: hidden;
  margin: auto 0;
}

</style>