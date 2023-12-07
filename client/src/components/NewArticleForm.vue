<script setup lang="ts">
import type {IArticle, IScience, IScientificTheory} from "@/api/type";
import {ref, watch} from "vue";
import {useField, useForm} from "vee-validate";
import {useArticleStore} from "@/stores/articleStore";
import MyLocalStorage from "@/services/myLocalStorage";

const props = defineProps<{
  scienceList:IScience[],
  scienceSectionList:IScientificTheory[]
  }>()
const articleStore = useArticleStore();
const emits = defineEmits(['close']);
const dialogShow = ref(false);//відображення діалогового вікна
const scienceId = ref('');//id обраної наукової сфери
const  isEditing = ref(false); //autocomplete з розділами наукової сфери недоступний
let scienceSectionList_: IScientificTheory[] //відфільтрований масив з розділами наукової сфери
//нова стаття
const article = ref<IArticle>({
  id: '',
  DOI: null,
  author_id: getAuthorId(),
  title: '',
  tag: '',
  text: null,
  views: 0,
  date_create:new Date(),
  modified_date: new Date(),
  theory_id: '',
  path_file: '',
  author_: null,
});
//отримуємо значення author_id  з localStorage
function  getAuthorId():string|null {
  let peopleId = MyLocalStorage.getItem('peopleId');
  if (peopleId!=null)  return peopleId.trim();
  return peopleId;
}
//фільтруємо список з розділами залежно від обраної науки
watch(scienceId,(newValue)=>{
  console.log("scienceId=",scienceId,' newValue=',newValue);
  if (newValue!=null && newValue!=''){
    isEditing.value = true;
    scienceSectionList_ = props.scienceSectionList.filter(s=>s.science_id.trim()===newValue.trim())
  }
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
    },
    theory_id (value:string) {
      if (value?.length > 0) return true

      return 'Оберіть тематику вашої статті'
    },
  },
})
const title= useField('title');
const  tag = useField('tag');
const  theory_id = useField('theory_id');

const submitArticle= handleSubmit(()=>{
  if (typeof title.value.value === "string") {
    article.value.title = title.value.value;
  }
  if (typeof tag.value.value === "string") {
    article.value.tag = tag.value.value;
  }
  if (typeof theory_id.value.value === "string") {
    article.value.theory_id = theory_id.value.value;
  }
  articleStore.saveArticle(article.value);
  emits('close', dialogShow.value);
})
function closeDialog() {
  emits('close', dialogShow.value);
}
</script>


<template>
  <v-card>
    <v-card-title>
      <span class="text-h5">Нова стаття</span>
    </v-card-title>
    <v-card-text>
      <v-form @submit.prevent="submitArticle">
        <v-row>
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
          <v-col cols="4" >
            <v-autocomplete
                label="Наукова сфера"
                :items="scienceList"
                variant="solo-inverted"
                v-model="scienceId"
                item-title="name"
                item-value="id"
            > </v-autocomplete>
            <v-autocomplete
                :disabled="!isEditing"
                item-title="name"
                item-value="id"
                :items="scienceSectionList_"
                label="Розділ"
                variant="solo-filled"
                v-model="theory_id.value.value"
                :error-messages="theory_id.errorMessage.value"
            ></v-autocomplete>
          </v-col>
          <v-col cols="8">
            <v-text-field
                clearable
                label="Теги"
                variant="solo"
                id="teg_article"
                v-model="tag.value.value"
                :error-messages="tag.errorMessage.value"
            />
            <v-text-field
                clearable
                label="DOI"
                variant="solo"
                id="DOI_article"
                v-model="article.DOI"
            />
          </v-col>
        </v-row>
        <v-row class="d-flex justify-end pa-4">
          <v-btn type="submit">Зберегти</v-btn>
          <v-btn @click="closeDialog">Скасувати</v-btn>
        </v-row>
      </v-form>
    </v-card-text>
  </v-card>

</template>

<style scoped>

</style>