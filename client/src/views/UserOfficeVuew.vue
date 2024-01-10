<script setup lang="ts">

import ProfileHeder from "@/components/ProfileHeder.vue";
import MyArticleView from "@/views/MyArticleView.vue";
import {reactive} from "vue";
import MySelectedArticle from "@/views/MySelectedArticle.vue";

const state = reactive({
  tab: null
});
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
      >
        <v-tab value="myArticle">Мої статті</v-tab>
        <v-tab value="selected">Збережені</v-tab>
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
  </v-container>
</template>

<style scoped>
.tab-style{
  background-color: white;
  border-top: 1px solid rgba(128, 128, 128, 20%);
}
</style>