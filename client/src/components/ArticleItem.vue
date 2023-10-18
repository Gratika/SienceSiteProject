<script setup lang="ts">
import type {IArticle} from "@/api/type";

const props = defineProps({
  article: {
    type:Object as () => IArticle,
    required: true,
    default: () => []
  },
})

</script>

<template>

  <v-card class="ma-2" >
    <v-card-title class="d-flex">
      <span class="d-inline font-weight-bold">{{ article.title }}</span>
      <v-spacer></v-spacer>
      <small class="d-inline ">
        <v-icon class="mr-1">mdi-eye</v-icon>
        <span class="subheading mr-2">{{ article.views }}</span>
      </small>


    </v-card-title>

    <v-card-text class="headline">
      {{ article.text }}
    </v-card-text>

    <v-card-actions>
      <span>
        Автор: {{ article.author_id }}
      </span>
      <v-spacer></v-spacer>
      <v-btn icon>
         <v-icon>mdi-star-outline</v-icon>
      </v-btn>
    </v-card-actions>
  </v-card>
</template>

<style scoped>

</style>