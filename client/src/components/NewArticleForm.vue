<script setup lang="ts">
import type {IArticle, IScience, IScientificTheory} from "@/api/type";
import {ref, watch} from "vue";

const props = defineProps<{
  scienceList:IScience[],
  scienceSectionList:IScientificTheory[],
  article:IArticle
  }>()

const scienceId = ref('');
const filteredScienceSections = ref([]);
const article_ = ref(props.article);
watch(scienceId,()=>{
  return props.scienceSectionList.filter(s=>s.science_id===parseInt(scienceId.value))
    }
)
</script>


<template>
  <v-form >
    <v-row>
      <v-col cols="4" >
        <v-autocomplete
            label="Наукова сфера"
            :items="scienceList"
            variant="solo-inverted"
            v-model="scienceId"
            item-title="name"
            item-value="id"
            > </v-autocomplete>
        <v-autocomplete
            label="Розділ"
            :items="scienceSectionList"
            variant="solo-filled"
            v-model="article_.theory_id"
            item-title="name"
            item-value="id"
        ></v-autocomplete>
        <v-text-field
            clearable
            label="Теги"
            variant="solo"
            id="teg_article"
            v-model="article_.tag"
        />
      </v-col>
      <v-col cols="8">
        <v-text-field
            clearable
            label="Назва статті"
            variant="solo"
            id="title_article"
            v-model="article_.title"
        />
        <v-textarea
            clearable
            variant="solo"
            clear-icon="mdi-close-circle"
            label="Напишіть коротку анатацію до статті"
            id="annotation"
            v-model="article_.text"
        ></v-textarea>
        <v-text-field
            clearable
            label="DOI"
            variant="solo"
            id="DOI_article"
            v-model="article_.DOI"
        />
      </v-col>
    </v-row>
    <v-row class="d-flex justify-end">
       <v-btn type="submit">Зберегти</v-btn>
      <v-btn>Скасувати</v-btn>
    </v-row>


  </v-form>
</template>

<style scoped>

</style>