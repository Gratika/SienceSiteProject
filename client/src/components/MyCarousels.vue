<script setup lang="ts">
import {ref, onMounted, computed, getCurrentInstance} from 'vue';

const props = defineProps<{
  title:String,
  sladeWidth:number;
  countSlade:number;
  countChildElement:number;
}>();
const currentSlideIndex = ref(0);
//const currentSlideIndex = ref(props.countSlade);
const isWhiteL = ref(true);
const isWhiteR = ref(false);
/*const prev = () => {
  if (currentSlideIndex.value > 0) {
    isWhiteR.value = false;
    currentSlideIndex.value -= 1;
    if (currentSlideIndex.value == 0) isWhiteL.value = true;
  }
  console.log('currentSlideIndex=',currentSlideIndex.value)
};
const next = () => {
  // Отримання кількості дочірніх елементів каруселі
  const childrenCount = document.querySelectorAll('.wrapper .content > *').length;
  console.log('childrenCount=',childrenCount)

  if (currentSlideIndex.value < childrenCount/props.countSlade) {
    currentSlideIndex.value += 1;
    isWhiteL.value = false;
    if (currentSlideIndex.value == childrenCount/props.countSlade) isWhiteR.value = true;
  }
  console.log('currentSlideIndex=',currentSlideIndex.value)
};*/
const prev = () => {
  if (currentSlideIndex.value > 0) {
    isWhiteR.value = false;
    currentSlideIndex.value -= 1;
    if (currentSlideIndex.value == 0) isWhiteL.value = true;
  }
  console.log('currentSlideIndex=',currentSlideIndex.value)
};
const next = () => {
  // Отримання кількості дочірніх елементів каруселі

  console.log('childrenCount=',props.countChildElement)

  if (currentSlideIndex.value < props.countChildElement-props.countSlade) {
    currentSlideIndex.value += 1;
    isWhiteL.value = false;
    if (currentSlideIndex.value == props.countChildElement-props.countSlade) isWhiteR.value = true;
  }
  console.log('currentSlideIndex=',currentSlideIndex.value)
};
</script>

<template>
  <div class="slide-group">
    <div class="header">
      <div class="text-h4 font-weight-bold">{{ props.title }}</div>
      <div class="navigation">
        <v-btn
            icon="mdi-chevron-left"
            :color="isWhiteL ? 'white' : 'black'"
            size="small"
            @click="prev"
        />
        <v-btn
            icon="mdi-chevron-right"
            :color="isWhiteR ? 'white' : 'black'"
            size="small"
            @click="next"/>
      </div>
    </div>
    <div class="wrapper" :style="{'max-width': (sladeWidth*countSlade)+'px'}">
      <div
          class="content"
          :style="{'margin-left':'-'+(100/countSlade*currentSlideIndex)+'%'}"
      >
        <slot></slot>
      </div>
    </div>

  </div>
</template>

<style>
.slide-group {
  padding: 25px 0;
}

.header {
  align-items: center;
  display: flex;
  justify-content: space-between;
  padding: 10px 0;
}

.navigation {
  /* Стилі для блоку з кнопками навігації */
  display: flex;
  gap: 10px;

}

.content {
  display: flex;
  margin-top: 20px;
  transition: all ease .5s;
}
.wrapper{
  overflow: hidden;
  margin: 20px auto;
  padding: 20px 0;
}
</style>
