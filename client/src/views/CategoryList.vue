<script setup lang="ts">
 import {useArticleStore} from "@/stores/articleStore";
 import {onMounted, ref} from "vue";
 import CategoryCard from "@/components/CategoryCard.vue";

 const articleStore = useArticleStore();
 const cnt = ref(0);
 onMounted(()=>{
   articleStore.getScienceList();
   window.scroll(0,0)
 })
</script>

<template>
<v-container>
  <v-row class="justify-sm-center justify-md-start">
    <div class="text-h4 font-weight-bold mt-13 mb-5">
      Доступні категорії
    </div>
  </v-row>
  <v-row>
    <v-col
        v-for="(item, index) in articleStore.sciences"
        :key="index"
        cols="12"
        sm="4"
        md="3"
    >
     <CategoryCard :science="item"/>
    </v-col>
  </v-row>
  <v-row>
    <div class="footer-distance"></div>
  </v-row>
</v-container>
</template>

<style scoped>
.footer-distance{
  min-height: 100px;
}
</style>