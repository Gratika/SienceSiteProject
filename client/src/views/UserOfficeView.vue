<script setup lang="ts">

import ProfileHeder from "@/components/ProfileHeder.vue";
import MyArticleView from "@/views/MyArticleView.vue";
import {onMounted, reactive} from "vue";
import MySelectedArticle from "@/views/MySelectedArticle.vue";
import {useArticleStore} from "@/stores/articleStore";
import MyLocalStorage from "@/services/myLocalStorage";

const state = reactive({
  tab: null
});
const articleStore = useArticleStore();
const peopleId = MyLocalStorage.getItem('peopleId');
onMounted(()=>{
  //articleStore.getMyArticleList(peopleId); //список моїх статей
  articleStore.searchArticlesByParam(0,undefined,undefined,
      undefined,undefined,undefined,peopleId);//список моїх статей
  articleStore.getMySelectedArticleList(peopleId); //список моїх статей
  articleStore.getScienceList(); //отримуємо список наукових сфер (для формування списку тегів)
  articleStore.getScienceSectionList(); //отримуємо список підкатегорій
})
function getPageData(){
  console.log('state.tab=',state.tab);
  console.log('typeof',typeof state.tab)
  if(state.tab==='myArticle') //articleStore.getMyArticleList(peopleId);
    articleStore.searchArticlesByParam(0,undefined,undefined,
        undefined,undefined,undefined,peopleId);//список моїх статей
  if(state.tab==='selected') articleStore.getMySelectedArticleList(peopleId); //список моїх статей
}
</script>

<template>
  <v-container>
    <ProfileHeder/>
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
</style>