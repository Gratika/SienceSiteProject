<script setup lang="ts">
import {ref, computed, onBeforeMount} from 'vue';
import type { IArticle } from '@/api/type';
import {useRoute} from 'vue-router';
import {useArticleStore} from '@/stores/articleStore';
import MyLocalStorage from '@/services/myLocalStorage';
import {useReactionStore} from '@/stores/reactionStore';
import RatingEmoji from "@/components/RatingEmoji.vue";
import RichTextEditor from "@/components/RichTextEditor.vue";

//отримуємо id статті з роута
const  route = useRoute();
const {id} = route.params;

const editorReadOnly = true;
const article = ref<IArticle>({
  id: '',
  doi: null,
  author_id: '',
  title: '',
  tag: '',
  text: '',
  views: 0,
  date_created: null,
  modified_date: null,
  theory_id: '',
  theory_:null,
  path_file: '',
  author_: {
    id:'',
    surname:'',
    name:'',
    birthday:'',
    path_bucket:'',
    date_create: '',
    modified_date: ''
  },
  tagItems:[],
  reaction: null,
  countLike:0
});
const initContent = ref('');
const articleStore = useArticleStore();
const  peopleId = MyLocalStorage.getItem('peopleId');
const emotionStore = useReactionStore();
const isLoading = ref(true);
onBeforeMount(()=>{
  if(typeof id ==='string'){
    articleStore.getArticle(id, peopleId)//отримуємо статтю з сервера
        .then((data) =>{
          console.log('readArticle=', data)
          isLoading.value=false;
          if (data !== undefined){
            article.value = data;
            initContent.value=data.text;
          }
        })
  }

  articleStore.getPopularArticleList(5);
  emotionStore.getEmojiList();
  /*if (userId){
    emotionStore.getSelectedEmoji(id[0],userId);
  }*/
})
//const url = ' http://localhost:5000/api/Files/GetArchivFiles?id='+article.value.id
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
  articleStore.downloadFiles(article.value.id);
}


</script>
<template>
  <v-row class="title-head align-content-end" >
    <v-container bg-color="background">
      <v-overlay :model-value="isLoading"
                 class="align-center justify-center">
        <v-progress-circular
            indeterminate
            color="primary"
        ></v-progress-circular>
      </v-overlay>
      <div class="d-flex flex-column flex-grow-1">
        <div class="my-title">{{article.title}}</div>
        <div class="d-flex flex-row flex-grow-1">
          <div class="d-lg-flex flex-grow-1">
            <v-chip
                v-for="(item, index) in article.tagItems"
                :key="index"
                class="ma-2 my-chips"
                color="primary-darken-1"
                variant="flat"
            >
              {{ item }}
            </v-chip>
          </div>
          <div>
            <RatingEmoji
                :emoji-list="emotionStore.emojiList"
                :user-reaction-id="emotionStore.selectedEmoji"
            />
          </div>
        </div>

      </div>
    </v-container>
  </v-row>
  <v-row>
    <v-container class=" px-0 px-lg-1">
      <v-row class="flex-grow-1">
        <v-col lg="9" md="7" class="d-flex flex-grow-1 pe-3">
          <RichTextEditor v-if="!isLoading"
              :initialContent="initContent"
              :is-read-only="editorReadOnly"
          />
        </v-col>
        <v-col lg="3" md="5" class="d-none d-md-flex px-0 px-lg-1">
          <div class="d-flex flex-column">
            <v-list bg-color="background" class="auth-list">
              <v-list-item >
                <v-icon class="me-2 " icon="mdi-account-circle-outline"/>
                <v-list-item-title class="d-inline">
                  Автор: {{article.author_?.surname}} {{article.author_?.name}}
                </v-list-item-title>
              </v-list-item>
              <v-list-item>
                <v-icon class="me-2" icon="mdi-calendar-blank-outline"/>
                <v-list-item-title class="d-inline">
                  Дата: {{ article.date_created }}
                </v-list-item-title>
              </v-list-item>
              <v-list-item>
                <v-icon class="me-2" icon="mdi-alpha-a-box-outline"/>
                <v-list-item-title class="d-inline">
                  Мова:
                </v-list-item-title>
              </v-list-item>
              <v-list-item>
                <v-icon class="me-2" icon="mdi-map-marker-radius-outline"/>
                <v-list-item-title class="d-inline">
                  Країна публікації: {{ article.date_created }}
                </v-list-item-title>
              </v-list-item>
            </v-list>
            <v-card
                class="ps-3 mt-6"
            >
              <v-list>
                <v-list-subheader
                    class="text-h5 font-weight-bold text-center"
                >
                  <span class="my-auto">Популярні</span>
                </v-list-subheader>

                <v-list-item
                    v-for="(item, i) in articleStore.articles"
                    :key="i"
                >
                  <v-row>
                    <v-col cols="2" class="pa-0">
                      <span class="text-h4 font-weight-bold me-1">{{ i+1 }}</span>
                    </v-col>
                    <v-col cols="10" class="pa-0">
                      <v-card-text class="text-subtitle-1 ps-1">{{item.title}}</v-card-text>
                    </v-col>
                  </v-row>


                </v-list-item>
              </v-list>
            </v-card>
          </div>
        </v-col>
      </v-row>

    </v-container>
  </v-row>
  <!--v-container>

    <div>
      <div class="d-flex justify-end"-->

        <!--a href="{{url}}"/-->
        <!--v-btn icon="mdi-download" @click="downloadFile"/>
      </div>
      <h1>{{ article.title }}</h1>
      <p v-if="article.author_">
        Автор: {{ article.author_?.surname }} {{ article.author_?.name }}
      </p>
      <div v-html="displayedText"></div>
      <button @click="nextPage" v-if="showPagination">Next Page</button>
    </div>
  </v-container-->

</template>
<style scoped>
.title-head{
  background-color: #E2E2E2;
  height: 240px;
}

.my-title{
  display: flex;
  flex-wrap: wrap;
  font-size: 38px;
  font-style: normal;
  font-weight: 700;
  justify-content: flex-start;
  line-height: normal;
  padding: 10px 5px;
}
.my-chips{
  border-radius: 0;
}
.auth-list{
  font-family: Mariupol;
  font-size: 24px;
  font-style: normal;
  font-weight: 500;
  line-height: normal;
}
.article-list-item{
  font-family: Mariupol;
  font-size: 16px;
  font-style: normal;
  font-weight: 400;
  line-height: normal;
}
.article-list-key{
  font-family: Mariupol;
  font-size: 35px;
  font-style: normal;
  font-weight: 700;
  line-height: normal;
}
.article-list-title{
  font-family: Mariupol;
  font-size: 20px;
  font-style: normal;
  font-weight: 700;
  line-height: normal;
}
</style>


