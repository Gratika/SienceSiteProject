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
const title = "Популярні статті";
</script>

<template>
  <v-row class="justify-center carousel-color">
    <v-container>
      <MyCarousels :title="title">
        <SmallArticleItem
            v-for="item in props.articles"
            :key="item?.id"
            :article="item"
        />
      </MyCarousels>
      <!--v-slide-group
          prev-icon="mdi-arrow-left-drop-circle-outline"
          next-icon="mdi-arrow-right-drop-circle-outline"
          selected-class="bg-primary"
          show-arrows
      >
        <v-slide-group-item
            v-for="item in props.articles"
            :key="item?.id"
            v-slot="{ isSelected, toggle }"
        >
           <SmallArticleItem :article="item" @click="toggle"/>

        </v-slide-group-item>
      </v-slide-group-->
    </v-container>

  </v-row>

</template>

<style scoped>
.carousel-color{
  background-color: #E2E2E2;
}
</style>