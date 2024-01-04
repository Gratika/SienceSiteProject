<script setup lang="ts">
import { ref } from 'vue';

const props = defineProps<{
  title:String
}>();
const currentIndex = ref(0);
const prev = () => {
  if (currentIndex.value > 0) {
    currentIndex.value -= 1;
  }
};
const next = () => {
  // Отримання кількості дочірніх елементів
  const childrenCount = document.querySelectorAll('.slide-group .content > *').length;
  if (currentIndex.value < childrenCount - 1) {
    currentIndex.value += 1;
  }
};

</script>
<template>
  <div class="slide-group">
    <div class="header">
      <h2>{{ title }}</h2>
      <div class="navigation">
        <v-btn icon="mdi-chevron-left" size="small" @click="prev"/>
        <v-btn icon="mdi-chevron-right" size="small" @click="next"/>
      </div>
    </div>
    <div class="content">
      <slot></slot>
    </div>
  </div>
</template>

<style>
.slide-group {
  /* Стилі для контейнера */
}

.header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  /* Стилі для заголовка та навігації */
}

.navigation {
  display: flex;
  gap: 10px;
  /* Стилі для блоку з кнопками навігації */
}

.content {
  display: flex;
  flex-direction: row;
  flex-grow: 1;
  flex-wrap: wrap;
  overflow-wrap: break-word;
  overflow-x: hidden;

}
</style>
