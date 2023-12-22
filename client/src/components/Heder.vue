<script setup lang="ts">
import {ref} from "vue";
import {useRouter} from "vue-router";

const router = useRouter();
const searchStr = ref('');
function onSearch(){
  router.push({ name: 'search_article', params: { search: searchStr.value } });
}
</script>

<template>
<div class="header">
  <v-row class="justify-center">
    <v-col cols="7">
      <h1>Discovery Science, Uncover Knowledge with SciFindHub</h1>
      <v-text-field
          class="my-1 me-6 rounded-0"
          density="compact"
          variant="outlined"
          label="Пошук за назвою, автором, темою..."
          single-line
          hide-details
          v-model="searchStr"
      ></v-text-field>
      <v-btn variant="outlined" @click="onSearch">Пошук</v-btn>
    </v-col>
    <v-col cols="5">
      <v-img
          :width="400"
          aspect-ratio="1/1"
          cover
          src="mainFrame.png"
      ></v-img>
    </v-col>
  </v-row>
</div>
</template>

<style scoped>
.header{
  height: 891px;
}
</style>