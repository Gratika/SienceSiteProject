<script setup lang="ts">
import {onMounted, ref} from "vue";
import {useArticleStore} from "@/stores/articleStore";
import RichTextEditor from "@/components/RichTextEditor.vue";

 const articleStore = useArticleStore();
 const isUpdating = ref(false);
 onMounted(()=>{
   articleStore.getScienceList();
   articleStore.getScienceSectionList();
})
function getScienceSection(){
   articleStore.selectedScienceId=parseInt(search.value,10);
   //зробити запит на сервер чи відфільтрувати список?
}

const search = ref('')
const search2 = ref('')
</script>

<template>
  <v-row class="justify-center">
    <v-col cols="8" class="justify-center align-center">
      <RichTextEditor/>
    </v-col>
    <v-col cols="3">
 <v-form >
       <v-autocomplete
           label="Наукова сфера"
           :items="articleStore.sciences"
           :disabled = "isUpdating"
           variant="solo-inverted"
           v-model="search"
           item-title="name"
           item-value="id"
           @change="getScienceSection"
       > </v-autocomplete>
       <v-autocomplete
           label="Розділ"
           :items="articleStore.scientificSections"
           variant="solo-filled"
           :disabled = "isUpdating"
           v-model="search2"
           item-title="name"
           item-value="id"
       ></v-autocomplete>
   <v-text-field
       clearable
       label="DOI"
       variant="solo"
       id="DOI_article"
   />
   <v-textarea
       clearable
       label="Назва статті"
       variant="solo"
       id="title_article"
   />
   <v-textarea
       clearable
       label="Теги"
       variant="solo"
       id="teg_article"
   />


 </v-form>
    </v-col>
  </v-row>

</template>

<style scoped>

</style>