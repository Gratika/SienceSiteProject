<script setup lang="ts">
import type {IArticle, ISelectedArticle} from "@/api/type";
import {useAuthStore} from "@/stores/authStore";
import {useRouter} from "vue-router";
import {onMounted, ref} from "vue";
import MyLocalStorage from "@/services/myLocalStorage";
//пропси від батьківського елементу
const props = defineProps<{
  article:IArticle
}>()
const showEdit = ref(false);
const router = useRouter();
const authStore = useAuthStore();
onMounted(()=>{
  const isLoginString = MyLocalStorage.getItem('isLogin');
  if (isLoginString!=null){
    showEdit.value = isLoginString===true;
  }
})
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
//дані, які будемо надсилати в батьківський елемент
const emits = defineEmits(['add_selected'])
function addArticleSelect(){
  let selectedArticle: ISelectedArticle = {
    id:'',
    article_id: props.article?.id as string,
    article_:props.article,
    user_id: authStore.getUserId as string,
    user_:authStore.authUser,
    Date_view: new Date(),
  };
  emits('add_selected', selectedArticle);
}
//для налаштування переходу на сторінку редагування статті
function editArticle(){
  router.push({ name: 'edit_article', params: { id: props.article.id } });

}
function readArticle(){
  router.push({ name: 'read_article', params: { id: props.article.id } });

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
          <div v-if="showEdit">
            <v-tooltip location="top center" origin="end center">
              <template v-slot:activator="{ props }">
                <v-btn icon v-bind="props" @click="addArticleSelect">
                  <v-icon>mdi-bookmark-outline</v-icon>
                </v-btn>
              </template>
              <div>Додати в обране</div>
            </v-tooltip>
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
          Дата: {{ props.article.date_create}}
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
              class="ma-2"
              color="primary-darken-1"
              variant="flat"
              label
          >
            <v-icon start icon="mdi-label"></v-icon>
            Tags
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