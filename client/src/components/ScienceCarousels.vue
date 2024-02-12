<script setup lang="ts">
import type { IScience} from "@/api/type";
import MyCarousels from "@/components/MyCarousels.vue";
import CategoryCard from "@/components/CategoryCard.vue";
import {computed, onMounted, ref} from "vue";

const props =defineProps({
  science:{
    type:Array<IScience>,
    default:{},
    required:true
  }
})
const title = "Виберіть категорію";

const slideWidth:number = 230;
const myCarousel = ref<Element|undefined>();
const widthContainer = ref(window.innerWidth); // Встановлюємо початкове значення ширини

// Вираховуємо нову ширину при зміні розміру вікна
window.addEventListener('resize', () => {
  widthContainer.value = myCarousel.value?.clientWidth || window.innerWidth;
});

const cntSlide= computed(() => {
  console.log('width=',  widthContainer.value)
  console.log('к-сть слайдів', Math.trunc( widthContainer.value / slideWidth))
  return Math.trunc( widthContainer.value / slideWidth);
})
onMounted(()=>{
  if (myCarousel.value!==undefined){
    console.log('myCarousel.value',myCarousel.value)
    widthContainer.value =  myCarousel.value?.clientWidth
  }
})

</script>

<template>
  <v-row class="justify-center pt-5">
    <v-container class="px-0">
      <div ref="myCarousel" class="d-flex w-100">
        <MyCarousels :title="title" :count-slade='cntSlide' :slade-width='slideWidth' :count-child-element="props.science?.length">
          <CategoryCard
              v-for="item in props.science"
              :key= "item?.id"
              :science = "item"
          />
        </MyCarousels>
      </div>

    </v-container>

  </v-row>
</template>

<style scoped>

</style>