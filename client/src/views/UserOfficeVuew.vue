<script setup lang="ts">

import ProfileHeder from "@/components/ProfileHeder.vue";
import MyArticleView from "@/views/MyArticleView.vue";
import {reactive} from "vue";

const state = reactive({
  tab: null
});
</script>

<template>
  <v-container>
    <ProfileHeder/>
    <v-card>
      <v-tabs
          v-model="state.tab"
          color="deep-purple-accent-4"
          align-tabs="title"
          grow
      >
        <v-tab value="myArticle">Мої статті</v-tab>
        <v-tab value="selected">Збережені</v-tab>
      </v-tabs>

      <v-card-text>
        <v-window v-model="state.tab">
          <v-window-item value="myArticle">
            <MyArticleView/>
          </v-window-item>

          <v-window-item value="selected">
            Selected
          </v-window-item>
        </v-window>
      </v-card-text>
    </v-card>
  </v-container>
</template>

<style scoped>

</style>