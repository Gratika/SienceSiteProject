<script setup lang="ts">
import type {IArticle} from "@/api/type";
import SmallArticleItem from "@/components/SmallArticleItem.vue";
import MyCarousels from "@/components/MyCarousels.vue";
import {computed, onMounted} from "vue";
import {ref} from "vue";


const props =defineProps({
  articles:{
    type:Array<IArticle>,
    default:{},
    required:true
  },
  title: {
    type:String,
    default:'',
    required:true
  }
})


const slideWidth:number = 390;
const myCarousel = ref<Element|undefined>();
const widthContainer = ref(window.innerWidth); // Встановлюємо початкове значення ширини

// Вираховуємо нову ширину при зміні розміру вікна
window.addEventListener('resize', () => {
  widthContainer.value = myCarousel.value?.clientWidth || window.innerWidth;
});

const cntSlide= computed(() => {
 // console.log('width=',  widthContainer.value)
 // console.log('к-сть слайдів', Math.trunc( widthContainer.value / slideWidth))
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
  <v-row class="justify-center carousel-color">
    <v-container class="px-0">
      <div ref="myCarousel" class="d-flex w-100">
        <MyCarousels :title="props.title" :count-slade='cntSlide' :slade-width='slideWidth' :count-child-element="props.articles?.length">
          <SmallArticleItem
              v-for="item in props.articles"
              :key="item?.id"
              :article="item"
          />

        </MyCarousels>
      </div>

    </v-container>

  </v-row>

</template>

<style scoped>
.carousel-color{
  background-color: transparent;
}
</style>