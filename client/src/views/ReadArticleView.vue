<script setup lang="ts">
import {ref, computed, onBeforeMount, watch} from 'vue';
import type {IArticle, IReaction} from '@/api/type';
import {useRoute} from 'vue-router';
import {useArticleStore} from '@/stores/articleStore';
import MyLocalStorage from '@/services/myLocalStorage';
import {useReactionStore} from '@/stores/reactionStore';
import RatingEmoji from "@/components/RatingEmoji.vue";
import RichTextEditor from "@/components/RichTextEditor.vue";
import moment from "moment";
import ItemListArticle from "@/components/ItemListArticle.vue";
import Like from "@/components/Like.vue";
import SmallArticleItem from "@/components/SmallArticleItem.vue";
import {createToast} from "mosha-vue-toastify";

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
  countLike:0,
  selected:false,
  isActive:false,
});
const articleLikeTag=ref<Array<IArticle>>([]);
const initContent = ref('');
let isSelected = ref(false);
let countLike = ref(0);
const articleStore = useArticleStore();
const  peopleId = MyLocalStorage.getItem('peopleId');

const emotionStore = useReactionStore();
const isLoading = ref(true);
let setReaction = ref(false);
onBeforeMount(()=>{
  if(typeof id ==='string'){
      articleStore.getArticle(id, peopleId)//отримуємо статтю з сервера
      .then((data) =>{
        //console.log('readArticle=', data);
        if (data !== undefined){
           article.value = data;
           initContent.value=data.text;
           isSelected.value=article.value.selected;
           setReaction.value = article.value.reaction!==null;
           countLike.value = article.value.countLike;

          let tags = getTags(article.value.tag);
          console.log('Tag=',tags)
          if (tags!==undefined)
            articleStore.searchArticlesByParam(1,undefined,undefined,
                  undefined,undefined,tags).then(()=>{
                    articleLikeTag.value=articleStore.articles?.filter(item=>
                      item.id!==article.value.id
                    )
            });

        }
      }).catch((error)=>{
         console.log(error);
      }).finally(()=>{ isLoading.value = false;})

  }
  articleStore.getPopularArticleList(5);

});

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
  })

}
}

function downloadFile(){
  articleStore.downloadFiles(article.value.id);
}
function  saveUserReaction(){
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
      createToast(error, {
        position: 'top-right',
        type: 'danger',
      });
    })
  }
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
       <v-col sm="12" md="10"  lg="11">
           <div class="d-flex flex-wrap justify-start py-4 px-1 text-h4 font-weight-bold">
             {{article.title}}
           </div>
       </v-col>
       <v-col md="2" lg="1" class="d-none d-md-block">
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
       </v-col>
     </v-row>
      <!--рядок з тегами та значком лайк-->
      <v-row class="align-center">
        <v-col sm="12" md="10"  lg="11">
            <div class="d-flex">
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
        </v-col>
        <v-col md="2" lg="1" class="d-none d-md-flex">
            <div class="d-flex justify-space-between">
              <Like :is-selected="setReaction" @click="saveUserReaction"/>
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
        <v-col lg="9" md="7" class="d-flex flex-grow-1 pe-3">
          <RichTextEditor v-if="!isLoading"
              :initialContent="initContent"
              :is-read-only="editorReadOnly"
          />
        </v-col>
        <!--стовпчик що являє собою бічну панель зі списками-->
        <v-col lg="3" md="5" class="d-none d-md-flex px-0 px-lg-1">
          <div class="d-flex flex-column ps-3">
            <v-list bg-color="background" class="text-h5 pb-6">
              <v-list-item >
                <v-icon class="me-2 " icon="mdi-account-circle-outline"/>
                <v-list-item-title class="d-inline">
                  Автор: {{article.author_?.surname}} {{article.author_?.name}}
                </v-list-item-title>
              </v-list-item>
              <v-list-item>
                <v-icon class="me-2" icon="mdi-calendar-blank-outline"/>
                <v-list-item-title class="d-inline">
                  Дата: {{ formatDate(article.date_created) }}
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
                  Країна публікації:
                </v-list-item-title>
              </v-list-item>
            </v-list>
            <ItemListArticle
                :articles="articleStore.popularArticles"
            />

          </div>
        </v-col>
      </v-row>

      <v-row v-if="articleLikeTag?.length>0">
        <div class="text-h4 font-weight-bold mt-10 pt-10 pb-10"> Схожі за тегами</div>
      </v-row>
      <v-row >
        <v-col
            v-for="(item, index) in articleLikeTag?.slice(0, 4)"
            :key="index"
            cols="12"
            sm="6"
            md="3"
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
.title-head{
  background-color: #E2E2E2;
  height: 240px;
}

.footer-distance{
  min-height: 100px;
}



</style>


