<script setup lang="ts">
import {ref, computed, onBeforeMount} from 'vue';
import type { IArticle } from '@/api/type';
import {useRoute} from 'vue-router';
import {useArticleStore} from '@/stores/articleStore';
import MyLocalStorage from '@/services/myLocalStorage';
import {useReactionStore} from '@/stores/reactionStore';
import RatingEmoji from "@/components/RatingEmoji.vue";

//отримуємо id статті з роута
const  route = useRoute();
const {id} = route.params;

const article = ref<IArticle>({
  id: '',
  DOI: null,
  author_id: '',
  title: '',
  tag: '',
  text: null,
  views: 0,
  date_create: null,
  modified_date: null,
  theory_id: '',
  Scientific_theories:null,
  path_file: '',
  author_: null,
});
const articleStore = useArticleStore();
const  userId = MyLocalStorage.getItem('userId');
const emotionStore = useReactionStore();
onBeforeMount(()=>{
  const data = articleStore.newArticles.find(art => art.id ===id);//тимчасово. Потім замінити
  if (data){
    article.value = data
  }
  emotionStore.getEmojiList();
  if (userId){
    emotionStore.getSelectedEmoji(id[0],userId);
  }
})

const currentPage = ref<number>(0);
const pageSize = 500; // Розмір сторінки для відображення тексту статті
const displayedText = computed(() => {
  const startIndex = currentPage.value * pageSize;
  return article.value.text ? article.value.text.substring(startIndex, startIndex + pageSize) : '';
});

function nextPage() {
  currentPage.value++;
}

const showPagination = computed(() => {
  return !!article.value.text && article.value.text.length > (currentPage.value + 1) * pageSize;
});
function downloadFile(){
  articleStore.downloadFiles();
}


</script>
<template>
  <div>
    <v-row>
      <v-col cols="4">
        <RatingEmoji
            :emoji-list="emotionStore.emojiList"
            :user-reaction-id="emotionStore.selectedEmoji"
        />
      </v-col>
      <v-col cols="2">
        <v-btn icon="mdi-download" @click="downloadFile"/>
      </v-col>
    </v-row>
    <h1>{{ article.title }}</h1>
    <p v-if="article.author_">
      Автор: {{ article.author_?.surname }} {{ article.author_?.name }}
    </p>
    <div v-html="displayedText"></div>
    <button @click="nextPage" v-if="showPagination">Next Page</button>
  </div>
</template>


