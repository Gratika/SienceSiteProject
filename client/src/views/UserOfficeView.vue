<script setup lang="ts">

import ProfileHeder from "@/components/ProfileHeder.vue";
import MyArticleView from "@/views/MyArticleView.vue";
import {onMounted, reactive, ref} from "vue";
import MySelectedArticle from "@/views/MySelectedArticle.vue";
import {useArticleStore} from "@/stores/articleStore";
import MyLocalStorage from "@/services/myLocalStorage";
import {useAuthStore} from "@/stores/authStore";
import type {IPeople} from "@/api/type";
import moment from "moment/moment";

const state = reactive({
  tab: null
});
const authStore = useAuthStore();
const articleStore = useArticleStore();
const peopleId = MyLocalStorage.getItem('peopleId');
let people_ = ref<IPeople>(
    {
      id:'',
      surname:'',
      name:'',
      birthday:'',
      path_bucket:'',
      date_create: '',
      modified_date:''
    }
);
const email = ref('');
onMounted(()=>{
  articleStore.searchArticlesByParam(0,undefined,undefined,
      undefined,undefined,undefined,peopleId);//список моїх статей
  articleStore.getMySelectedArticleList(peopleId,0); //список моїх обраних статей
  articleStore.getScienceList(); //отримуємо список наукових сфер (для формування списку тегів)
  articleStore.getScienceSectionList(); //отримуємо список підкатегорій
  authStore.getPeople()
      .then((res:IPeople|undefined)=> {
        if(res!==undefined) {
          people_.value = res
          people_.value.birthday = formatDate(people_.value.birthday);
        }
      });
  if(authStore.authUser)
    email.value = authStore.authUser.email;
  window.scroll(0,0);
});
function formatDate(date: null | string): string {
  console.log('date=',date)
  console.log('typeof date=',typeof date)
  if (date == null) return '';
  const formattedDate: Date = new Date(date);
  return (moment(formattedDate)).format('DD.MM.YYYY')
}
function getPageData(){
  console.log('state.tab=',state.tab);
  console.log('typeof',typeof state.tab)
  if(state.tab==='myArticle') //articleStore.getMyArticleList(peopleId);
    articleStore.searchArticlesByParam(0,undefined,undefined,
        undefined,undefined,undefined,peopleId);//список моїх статей
  if(state.tab==='selected') articleStore.getMySelectedArticleList(peopleId,0); //список моїх статей
}
</script>

<template>
  <v-container>
    <ProfileHeder :email="email" :people="people_"/>
    <v-card color="background" variant="flat">
      <v-tabs
          class="tab-style"
          v-model="state.tab"
          color="black"
          fixed-tabs
          grow
          @update:modelValue="getPageData"
      >
        <v-tab class="text-h5 py-6" value="myArticle">Мої статті</v-tab>
        <v-tab class="text-h5 py-6" value="selected">Збережені</v-tab>
      </v-tabs>

      <v-card-text class="px-0">
        <v-window v-model="state.tab">
          <v-window-item value="myArticle">
            <MyArticleView/>
          </v-window-item>

          <v-window-item value="selected">
            <MySelectedArticle/>
          </v-window-item>
        </v-window>
      </v-card-text>
    </v-card>
    <div class="footer-distance"></div>
  </v-container>
</template>

<style scoped>
.tab-style{
  background-color: white;
  border-top: 1px solid rgba(128, 128, 128, 20%);
}
.footer-distance{
  min-height: 100px;
}
.v-chip--variant-outlined{
  border: 1px solid black!important;
}
</style>