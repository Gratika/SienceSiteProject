<script setup lang="ts">
import {ref} from "vue";

const selectedTag = ref<Array<string>>([])//модель для фільтру Теги
const delimiters = ['#',','] //масив рядків, що будуть створювати новий тег при вводі
const sortedValue = ref<number>(0); //параметр за яким відбувається сортування
const props=defineProps({
  tagItems: {
    type:Array<string>,
    default:[],
    required:true
  },
  sortedOptions:{
    type:Array<object>,
    default:[],
    required:true
  }
});

const emits = defineEmits(['choose-tag', 'choose-sorted'])

function tagChoose(focused:boolean){ //по тегу
  if (!focused) {
    //console.log('selectedTag =', selectedTag.value);
    emits('choose-tag',selectedTag.value)
  }
  // articleStore.searchArticlesByParam()
}
function chooseSortParam(){
  emits('choose-sorted',sortedValue.value)
  //console.log("sortedValue=", sortedValue.value)
}
</script>

<template>
  <div class="d-flex">
    <v-combobox
        class="mx-3 w-50"
        label="Теги"
        :items="props.tagItems"
        :delimiters="delimiters"
        v-model="selectedTag"
        multiple
        chips
        @update:focused="tagChoose"
    ></v-combobox>
    <v-select
        class="mx-3 w-50"
        v-model="sortedValue"
        hint="Оберіть параметр сортування"
        :items="props.sortedOptions"
        item-title="value"
        item-value="key"
        label="Впорядкувати"
        @update:modelValue= "chooseSortParam"
    ></v-select>
  </div>
</template>

<style scoped>

</style>