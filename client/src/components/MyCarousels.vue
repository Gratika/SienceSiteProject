<script setup lang="ts">
import {ref, onMounted, computed, getCurrentInstance} from 'vue';

const props = defineProps<{
  title:String
}>();
const index = ref(0);
let slides = ref([]);
const slideDirection = ref('');

const slidesLength = computed(() => slides.value.length);

function next() {
  index.value++;
  if (index.value >= slidesLength.value) {
    index.value = 0;
  }
  slideDirection.value = 'slide-right';
}

function prev() {
  index.value--;
  if (index.value < 0) {
    index.value = slidesLength.value - 1;
  }
  slideDirection.value = 'slide-left';
}


//const instance = getCurrentInstance();
//slides = instance?.proxy?.$children;
/*onMounted(() => {
  slides.value = $children;
  slides.value.forEach((slide, idx) => {
    slide.index = idx;
  });
});*/
</script>
<!--script setup lang="ts">
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

</script-->
<template>
  <div class="slide-group">
    <div class="header">
      <h2>{{ props.title }}</h2>
      <div class="navigation">
        <v-btn icon="mdi-chevron-left" size="small" @click="prev"/>
        <v-btn icon="mdi-chevron-right" size="small" @click="next"/>
      </div>
    </div>
    <div class="wrapper">
      <div class="content">
        <slot></slot>
      </div>
    </div>

  </div>
</template>

<style>
.slide-group {
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
}
.wrapper{
  max-width: 400px;
  overflow: hidden;
}
</style>
