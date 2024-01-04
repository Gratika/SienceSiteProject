<script setup lang="ts">
import type {IArticle} from "@/api/type";
import {useRouter} from "vue-router";
import {useArticleStore} from "@/stores/articleStore";

//пропси від батьківського елементу
const props = defineProps<{
  article:IArticle
}>()
const router = useRouter();
const articleStore = useArticleStore();

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

//для налаштування переходу на сторінку перегляду статті
function readArticle(){
  router.push({ name: 'read_article', params: { id: props.article.id } });
}
</script>

<template>

  <v-card
      class="left-border pa-2 me-2 font-card"
      variant="elevated"
      max-width="400"
      max-height="300"
      @click="readArticle"
  >
    <v-card-text class="font-title">
      {{ props.article.title }}
    </v-card-text>
    <v-card-actions class="flex-column card-align">
      <div class="font-text">
        <div>
          Автор: {{ author_() }}
        </div>
        <div>
          Дата: {{ props.article.date_created?.toDateString}}
        </div>
        <div>
          Мова:
        </div>
      </div>
        <v-chip-group class="mt-2">
          <v-chip
              v-for="(item, index) in props.article.tagItems"
              :key="index"
              class="mt-1 me-1"
              variant="flat"
              color="primary-darken-1"
          >
            {{ item }}
          </v-chip>
        </v-chip-group>

      <div class=" d-flex flex-row justify-end">
          <v-btn icon>
            <v-icon>mdi-thumb-up-outline</v-icon>
          </v-btn>
          <span class="d-inline">{{article.countLike}}</span>
      </div>


    </v-card-actions>
  </v-card>
</template>

<style scoped>
.left-border{
  border-left-width: 21px;
  border-left-color: #2A3759
}
.card-align{
  align-items: normal!important;
}
.font-card{
  color: #000;
  font-family: Mariupol;
  line-height: normal!important;
}
.font-title{
  font-size: 24px;
  font-style: normal;
  font-weight: 500;

}
.font-text{
  font-size: 18px;
  font-style: normal;
  font-weight: 500;
}
.font-chips{
  color: #FFF;
  font-size: 16px;
  font-style: normal;
  font-weight: 700;
}

</style>