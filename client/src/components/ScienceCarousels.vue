<script setup lang="ts">
import type { IScience} from "@/api/type";
import MyCarousels from "@/components/MyCarousels.vue";
import CategoryCard from "@/components/CategoryCard.vue";

const props =defineProps({
  science:{
    type:Array<IScience>,
    default:{},
    required:true
  }
})
const title = "Виберіть категорію";

</script>

<template>
  <v-row class="justify-center pt-5">
    <v-container>
      <MyCarousels :title="title" :count-slade=5 :slade-width=230 :count-child-element="props.science?.length">
        <CategoryCard
            v-for="item in props.science"
            :key= "item?.id"
            :science = "item"
        />
      </MyCarousels>
    </v-container>

  </v-row>
</template>

<style scoped>

</style>