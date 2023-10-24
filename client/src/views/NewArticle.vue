<script setup lang="ts">
import type {IArticle} from "@/api/type";
 import type {Ref} from "vue";
import {onMounted, ref} from "vue";
import {useArticleStore} from "@/stores/articleStore";
import RichTextEditor from "@/components/RichTextEditor.vue";

 /*const newArticle:Ref<IArticle>=ref({
   id:undefined,
   DOI: '',
   author_id: undefined,
   title: '',
   tag: '',
   text: '',
   views: 0,
   date_create: Date.now(),
   modified_date: Date.now(),
   theory_id: undefined,
   path_file: '',
   author_: null
 })*/
 const articleStore = useArticleStore();
 const isUpdating = ref(true);
 onMounted(()=>{
   articleStore.getScienceList();
   articleStore.getScienceSectionList();
})

const search = ref('')
</script>

<template>
  <v-row class="justify-center">
    <v-col cols="5" class="justify-center align-center">
 <v-form >
   <span>{{search}}</span>
   <v-autocomplete
       label="Наукова сфера"
       :items="articleStore.sciences"
       :disabled = "isUpdating"
       variant="solo-filled"
       v-model="search"
       item-title="name"
       item-value="id"
   >
     <!--template v-slot:item="{  props, item }">
       {{item.raw.name}}
       <v-list-item
           v-bind="props"
           :title="item.raw.name"
       ></v-list-item>
     </template-->
   </v-autocomplete>
   <!--v-autocomplete
       label="Розділ"
       :items="articleStore.filteredScientificSections"
       variant="solo-filled"
   ></v-autocomplete-->
 </v-form>
    </v-col>
    <v-col cols="7">
      <RichTextEditor/>
    </v-col>
  </v-row>

</template>

<style scoped>

</style>