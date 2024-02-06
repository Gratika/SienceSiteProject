<script setup lang="ts">
import {ref, onBeforeMount, watch, onBeforeUnmount} from 'vue';
import type {IArticle, IReaction} from '@/api/type';
import {useRoute, useRouter} from 'vue-router';
import {useArticleStore} from '@/stores/articleStore';
import MyLocalStorage from '@/services/myLocalStorage';
import RichTextEditor from "@/components/RichTextEditor.vue";
import moment from "moment";
import ItemListArticle from "@/components/ItemListArticle.vue";
import Like from "@/components/Like.vue";
import SmallArticleItem from "@/components/SmallArticleItem.vue";
import {useReactionStore} from "@/stores/reactionStore";

//отримуємо id статті з роута
const  route = useRoute();
const {id} = route.params;

const router = useRouter();

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
  countLike:0,
  selected:false,
  isActive:false,
});
const articleLikeTag=ref<Array<IArticle>>([]);
const initContent = ref('');
let isSelected = ref(false);
let countLike = ref(0);
let isLogin = ref(false);
let showDownload = ref(false);
const articleStore = useArticleStore();

const emotionStore = useReactionStore();
const isLoading = ref(true);
let setReaction = ref(false);
let cntPopulateArticle = ref(0);
onBeforeMount(()=>{
  const isLoginString = MyLocalStorage.getItem('isLogin');
  if (isLoginString!=null){
    isLogin.value = isLoginString;
  }
  if(typeof id ==='string'){
    getData(id);
  }
  articleStore.getPopularArticleList(0).then(()=>{
    cntPopulateArticle.value=articleStore.popularArticles.length
  });
});

watch(
    () => route.params,
    (params) => {
      const { id } = params;
      if (typeof id === 'string') {
        getData(id);
        window.scroll(0,0)
      }

    }
);
function getData(id: string){
  const  peopleId =MyLocalStorage.getItem('peopleId');
  articleStore.getArticle(id, peopleId)//отримуємо статтю з сервера
      .then((data) =>{
        //console.log('readArticle=', data);
        if (data !== undefined){
          article.value = data;
          initContent.value=data.text;
          isSelected.value=article.value.selected;
          setReaction.value = article.value.reaction!==null;
          countLike.value = article.value.countLike;
          showDownload.value = article.value.path_file?.length > 0

          let tags = getTags(article.value.tag);
          console.log('Tag=',tags)
          if (tags!==undefined)
            articleStore.searchArticlesByParam(0,undefined,undefined,
                undefined,0,tags).then(()=>{
              articleLikeTag.value=articleStore.articles?.filter(item=>
                  item.id!==article.value.id
              )
            });

        }
      }).catch((error)=>{
        routerGoBack();
        console.log(error);
      }).finally(()=>{ isLoading.value = false;})
}
function routerGoBack() {
  // Перевірка, чи існує метод $router та чи існує попередня сторінка в історії
  if (typeof router !== 'undefined' && router.currentRoute.value.meta?.backEnabled) {
    // Перейти на попередню сторінку
    router.go(-1);
  } else {
    // Якщо історія недоступна або попередня сторінка не визначена, можна використовувати інші методи навігації, наприклад, redirectToHome()
    redirectToHome();
  }
}

function redirectToHome() {
  // Перейти на домашню сторінку або іншу сторінку за замовчуванням
  router.push('/');
}
function getTags(data:string|null):string|undefined{
  if(data!==null){
    if (data.endsWith(',')||data.endsWith('#'))
      return data.substring(0,data.length-1);
    else return data;
  }

}

//додати до обраного
function changeArticleSelect(){
  let article_id= article.value.id as string;
  if(!isSelected.value) {
    articleStore.addToFavorites(article_id).then(()=>{
      isSelected.value = true;
    }).catch((err)=>{
      console.log(err)
    })
  }else {
    articleStore.deleteFromFavorites(article_id)
        .then(()=>{
          isSelected.value = false;
        }).catch((err)=>{
          console.log(err)
    })
  }
}

function downloadFile(){
  articleStore.downloadFiles(article.value.id);
}
function  saveUserReaction(){
  if(isLogin.value){
    if (!setReaction.value) {
      let userReaction: IReaction = {
        id: '',
        people_id: MyLocalStorage.getItem("peopleId"),
        article_id: article.value.id,
        reaction_id: '1',
        date_create: (new Date()).toISOString(),
        modified_date: (new Date()).toISOString(),
      }
      emotionStore.saveUserReaction(userReaction)
          .then(() => {
            setReaction.value = true;
            countLike.value++
          }).catch((error)=>{
        console.log(error)
      })
    }
  }

}

function formatDate(date: null | string): string {
  //console.log('date=',date)
  // console.log('typeof date=',typeof date)
  if (date == null) return '';
  const formattedDate: Date = new Date(date);
  return (moment(formattedDate)).format('DD.MM.YYYY')
}

onBeforeUnmount(()=>{
  articleStore.incArticleView(article.value.id)
})



</script>
<template>
  <!--Рядок заголовку-->
  <v-row class="title-head align-content-end" >
    <v-container style="background-color:#F9F9F9">
      <v-overlay :model-value="isLoading"
                 class="align-center justify-center">
        <v-progress-circular
            indeterminate
            color="primary"
        ></v-progress-circular>
      </v-overlay>
      <!--рядок з назвою статті та значком закладки-->
      <v-row class="align-center">
       <v-col cols="12">
           <div class="d-flex flex-wrap justify-start py-4 px-1 text-h4 font-weight-bold">
             {{article.title}}
           </div>
       </v-col>

     </v-row>
      <!--рядок з тегами та значком лайк-->
      <v-row class="align-center">
        <v-col sm="12" md="10"  lg="11">
            <div class="d-flex flex-wrap">
              <v-chip
                  v-for="(item, index) in article.tagItems"
                  :key="index"
                  class="me-2 mb-2  chip-text-size"
                  color="primary-darken-1"
                  variant="flat"
              >
                {{ item }}
              </v-chip>
            </div>
        </v-col>
        <v-col md="2" lg="1" class="d-none d-md-flex">
            <div class="d-flex justify-space-between">
              <Like :is-selected="setReaction" @click="saveUserReaction" :style="{ cursor: isLogin ? 'pointer' : 'not-allowed' }"/>
              <span class="mx-3">{{countLike}}</span>
            </div>
        </v-col>
      </v-row>
    </v-container>
  </v-row>
  <!--рядок з текстом статті та боковою панелю-->
  <v-row>
    <v-container class=" px-0 pt-6 px-lg-1">
      <v-row class="flex-grow-1">
        <!--стовпчик з текстом статті-->
        <v-col lg="8" md="6" class="d-flex flex-grow-1 pe-3 text-h6">
          <RichTextEditor v-if="!isLoading"
              :initialContent="initContent"
              :is-read-only="editorReadOnly"
          />
        </v-col>
        <!--стовпчик що являє собою бічну панель зі списками-->
        <v-col lg="4" md="6" class="d-none d-md-flex px-0 px-lg-1">
          <div class="d-flex flex-column ps-3">
            <v-list bg-color="background" class="pb-3 mb-3">
              <v-list-item >
                <v-icon class="me-2 text-h5" icon="mdi-account-circle-outline"/>
                <v-list-item-title class="text-h6 d-inline">
                  Автор: {{article.author_?.surname}} {{article.author_?.name}}
                </v-list-item-title>
              </v-list-item>
              <v-list-item>
                <v-icon class="me-2 text-h5" icon="mdi-calendar-blank-outline"/>
                <v-list-item-title class="d-inline text-h6">
                  Дата: {{ formatDate(article.date_created) }}
                </v-list-item-title>
              </v-list-item>
              <v-list-item>
                <v-icon class="me-2 text-h5" icon="mdi-alpha-a-box-outline"/>
                <v-list-item-title class="d-inline text-h6">
                  Мова: українська
                </v-list-item-title>
              </v-list-item>
              <v-list-item>
                <v-icon class="me-2 text-h5" icon="mdi-map-marker-radius-outline"/>
                <v-list-item-title class="d-inline text-h6">
                  Країна публікації: Україна
                </v-list-item-title>
              </v-list-item>
            </v-list>
            <!--блок з кнопками Додати в обране, Скачати файл-->
            <div class="d-flex flex-column justify-start  ps-4 pb-3 mb-3">
              <div v-if="isLogin" class="d-block mb-3 ms-3" >
                <v-icon
                        class="flex-grow-1"
                        @click="changeArticleSelect"
                >
                  <svg
                      xmlns="http://www.w3.org/2000/svg"
                      width="30"
                      height="40"
                      viewBox="0 0 40 40"
                      :fill="isSelected ? '#778FD2' : '#FFFFFF'"
                      stroke="#778FD2"
                      stroke-width="2"
                      stroke-linecap="round"
                      stroke-linejoin="round"
                  >
                    <path d="M30.8574 37.6429L20.0002 26.7857L9.14307 37.6429V5.07146C9.14307 4.35158 9.42904 3.6612 9.93805 3.15217C10.4471 2.64315 11.1375 2.35718 11.8574 2.35718H28.1431C28.8629 2.35718 29.5534 2.64315 30.0623 3.15217C30.5713 3.6612 30.8574 4.35158 30.8574 5.07146V37.6429Z" />
                  </svg>
                </v-icon>
                <span class="ms-2 text-h6 color-text">
                  Додати у збережені
                </span>
              </div>
              <div v-if="showDownload"
                   class="d-block align-center text-h6 color-text ms-3"
                   @click="downloadFile"
                   :style="{ cursor: 'pointer'}"
              >
                <v-icon
                    class="flex-grow-1"
                    icon="mdi-arrow-down-bold-box-outline"
                />
                <span class="ms-2">
                  Завантажити файл
                </span>
              </div>
            </div>
            <ItemListArticle v-if="cntPopulateArticle>0"
                :articles="articleStore.popularArticles.slice(0,5)"
            />

          </div>
        </v-col>
      </v-row>

      <v-row v-if="articleLikeTag?.length>0">
        <div class="text-h4 font-weight-bold mt-10 pt-10 pb-10"> Схожі за тегами</div>
      </v-row>
      <v-row >
        <v-col
            v-for="(item, index) in articleLikeTag?.slice(0, 3)"
            :key="index"
            cols="12"
            sm="6"
            md="4"
        >
          <SmallArticleItem :article="item" />
        </v-col>
      </v-row>
      <v-row>
        <div class="footer-distance"></div>
      </v-row>
    </v-container>
  </v-row>

</template>
<style scoped>

.chip-text-size{
  font-size: 16px!important;
}
.title-head{
  background-color: #E2E2E2;
  height: 240px;
}

.footer-distance{
  min-height: 100px;
}
.color-text{
  color: #778FD2;
}



</style>


