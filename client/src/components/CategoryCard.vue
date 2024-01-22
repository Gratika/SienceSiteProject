<script setup lang="ts">
  import type {IScience} from "@/api/type";
  import {useRouter} from "vue-router";

  const props=defineProps<{
    science: IScience
  }>()

  const router = useRouter();
  function searchScienceCategoryArticle(){
    router.push({ name: 'search_science_article', params: { scienceId: props.science.id} });
  }
</script>

<template>
  <div class="card-box" @click="searchScienceCategoryArticle">
    <v-sheet
        height="180"
        width="190"
        class="py-4 d-flex flex-column card-shadow">
      <v-img
          class="ma-auto"
          :width="120"
          :src="props.science?.note"
      ></v-img>
      <div class="text-center py-1 text-h5 font-weight-medium">{{props.science?.name}}</div>
    </v-sheet>

  </div>

</template>

<style scoped>
.card-box{
  background-color: transparent;
  cursor: pointer;
  height: 200px;
  margin: 0 10px;
  padding: 10px;
  width: 230px;
}

.card-shadow{
  background: #FFF;
  border-radius: 5px;
  box-shadow: 0 0 10px 0 rgba(0, 0, 0, 0.25);
}
</style>