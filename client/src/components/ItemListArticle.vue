<script setup lang="ts">
import type {IArticle} from "@/api/type";
import {computed} from "vue";

const props = defineProps({
  articles: {
    type:Array<IArticle>,
    default:[],
    required:true
  }
})

</script>

<template>
  <v-card
      class="ps-3 pt-3 mt-6 card-shadow"
  >
    <v-list>
      <div class="text-h5 font-weight-bold d-flex justify-center">
        <span>Популярні</span>
      </div>

      <v-list-item
          v-for="(item, i) in props.articles"
          :key="i"
      >
        <div class="d-flex flex-row align-center pa-0">
          <span class="text-h4 font-weight-bold me-2 d-inline">{{ i+1 }}</span>
          <v-card-text class="text-subtitle-1 ps-1 d-inline">{{item.title}}</v-card-text>
        </div>
        <v-divider v-if="i+1<props.articles?.length" :thickness="2"></v-divider>
      </v-list-item>
    </v-list>
  </v-card>
</template>

<style scoped>
 .card-shadow{
   border-radius: 5px;
   box-shadow: 0 0 10px 0 rgba(0, 0, 0, 0.25);
 }
</style>