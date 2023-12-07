<script setup lang="ts">
import {onBeforeMount, onMounted, ref} from "vue";
import {useArticleStore} from "@/stores/articleStore";
import RichTextEditor from "@/components/RichTextEditor.vue";
import {useRoute} from "vue-router";
import type {IArticle} from "@/api/type";
import {useField, useForm} from "vee-validate";
 //отримуємо id статті з роута
const  route = useRoute();
const {id} = route.params;
//модель статті
const article = ref<IArticle>({
  id: '',
  DOI: null,
  author_id: '',
  title: '',
  tag: '',
  text: null,
  views: 0,
  date_create:null,
  modified_date: null,
  theory_id: '',
  path_file: '',
  author_: null,
})
 const articleStore = useArticleStore();
 const isUpdating = ref(false);
 onBeforeMount(()=>{
   const data = articleStore.myArticles.find(art => art.id ===id);
   if (data){article.value = data}
 })

/*валідація форм*/
const { handleSubmit, handleReset } = useForm({
  validationSchema: {
    title (value:string) {
      if (value?.length > 0) return true

      return 'Введіть назву статті'
    },
    tag (value:string) {
      if (value?.length > 0) return true

      return 'Вкажіть теги для вашої статті'
    }
  },
})
const title= useField('title');
const  tag = useField('tag');
//збереження відредагованної статті
const submitArticle= handleSubmit(()=>{
  if (typeof title.value.value === "string") {
    article.value.title = title.value.value;
  }
  if (typeof tag.value.value === "string") {
    article.value.tag = tag.value.value;
  }
  console.log(JSON.stringify(article))
  //articleStore.saveArticle(article.value);

})
</script>

<template>
  <v-form @submit.prevent="submitArticle">
    <v-row class="justify-center">
      <v-col cols="12">
        <v-text-field
            clearable
            label="Назва статті"
            variant="solo"
            id="title"
            v-model="title.value.value"
            :error-messages="title.errorMessage.value"
        />
      </v-col>
    </v-row>
    <v-row>
      <v-col cols="6" >
        <v-text-field
            clearable
            label="Теги"
            variant="solo"
            id="teg_article"
            v-model="tag.value.value"
            :error-messages="tag.errorMessage.value"
        />
      </v-col>
      <v-col cols="6">
        <v-text-field
            clearable
            label="DOI"
            variant="solo"
            id="DOI_article"
            v-model="article.DOI"
        />
      </v-col>
    </v-row>
    <v-row class="justify-center">
      <v-col cols="8" class="justify-center align-center">
        <RichTextEditor/>
      </v-col>
    </v-row>
  </v-form>


</template>

<style scoped>

</style>