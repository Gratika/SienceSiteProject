<script setup lang="ts">
import type {IArticle} from "@/api/type";
import SmallArticleItem from "@/components/SmallArticleItem.vue";
import MyCarousels from "@/components/MyCarousels.vue";

const props =defineProps({
  articles:{
    type:Array<IArticle>,
    default:{},
    required:true
  }
})
const title = "Останні опубліковані статті";
</script>

<template>
  <v-row class="justify-center carousel-color">
    <v-container>
      <MyCarousels :title="title" :count-slade=3 :slade-width=390 :count-child-element="props.articles?.length">
        <SmallArticleItem
            v-for="item in props.articles"
            :key="item?.id"
            :article="item"
        />

      </MyCarousels>
    </v-container>

  </v-row>

</template>

<style scoped>
.carousel-color{
  background-color: #E2E2E2;
}
</style>