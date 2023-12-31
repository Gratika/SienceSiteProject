<script setup lang="ts">
import type {IArticle, IEmotion, IPeople, IScience, IScientificTheory, IUser} from "@/api/type";
import {ref, watch} from "vue";
import {useField, useForm} from "vee-validate";
import {useArticleStore} from "@/stores/articleStore";
import MyLocalStorage from "@/services/myLocalStorage";
import {object} from "zod";

const props = defineProps<{
  scienceList:IScience[],
  scienceSectionList:IScientificTheory[]
  }>()
const articleStore = useArticleStore();
const emits = defineEmits(['close']);
const dialogShow = ref(false);//відображення діалогового вікна
const scienceId = ref('');//id обраної наукової сфери
const  isEditing = ref(false); //autocomplete з розділами наукової сфери недоступний
const delimiters = ['#',','] //масив рядків, що будуть створювати новий тег при вводі
let scienceSectionList_: IScientificTheory[] //відфільтрований масив з розділами наукової сфери
//нова стаття
const article = ref<IArticle>({
  id: '',
  doi: null,
  author_id: getAuthorId(),
  title: '',
  tag: '',
  text: '',
  views: 0,
  date_created:new Date(),
  modified_date: new Date(),
  theory_id: '',
  theory_:null,
  path_file: '',
  author_: getAuthor(),
  tagItems:[],
  reaction: null,
  countLike:0
});
let scienceTheory = ref<IScientificTheory|undefined>({
  id: '',
  science_id:'',
  science_: null,
  name: '',
  note: ''
})
//отримуємо значення author_id  з localStorage
function  getAuthorId():string|null {
  let peopleId = MyLocalStorage.getItem('peopleId');
  if (peopleId!=null)  return peopleId.trim();
  return peopleId;
}
function  getAuthor():IPeople|null {
  let userStr:string|null = MyLocalStorage.getItem('user');
  console.log('userStr=',userStr)
  let res: null|IPeople = null;
  if(userStr !=null){
    let user:IUser =  MyLocalStorage.getItem('user')
    res = user.people_;
  }
  return res;
}
/*валідація форм*/
const { handleSubmit, handleReset } = useForm({
  validationSchema: {
    title (value:string) {
      if (value?.length > 0) return true

      return 'Введіть назву статті'
    },
    tag (value:Array<string>) {
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
//фільтруємо список з розділами залежно від обраної науки
watch([scienceId,theory_id],([newScienceId,newTheory_id])=>{
  console.log("scienceId=",scienceId,' newValue=',newScienceId);
  if (newScienceId!=null && newScienceId!=''){
    isEditing.value = true;
    scienceSectionList_ = props.scienceSectionList.filter(s=>s.science_id.trim()===newScienceId.trim())
  }
  if (newTheory_id && newTheory_id.value.value!=null && newTheory_id.value.value!=''){
    scienceTheory.value = props.scienceSectionList.find(s=>s.id===newTheory_id.value.value)
  }
})
const submitArticle= handleSubmit(()=>{
  if (typeof title.value.value === "string") {
    article.value.title = title.value.value;
  }
  let tagString='';
 // console.log('tag',tag.value.value);
  (tag.value.value as string[]).forEach((item:string)=>{
    if (!tagString.includes(item+',')){
      tagString = tagString.concat(item+',');
    }
  })
  article.value.tag = tagString;
  /*if (typeof tag.value.value === "string") {
    article.value.tag = tag.value.value;
  }*/
  if (typeof theory_id.value.value === "string") {
    article.value.theory_id = theory_id.value.value;
  }
  if (scienceTheory.value)  article.value.theory_=scienceTheory.value;
  console.log('article=', article.value);
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
            <!--v-text-field
                clearable
                chips
                label="Теги"
                variant="solo"
                id="teg_article"
                v-model="tag.value.value"
                :error-messages="tag.errorMessage.value"
            /-->
            <v-combobox
                clearable
                label="Теги"
                :items="articleStore.tagItems"
                :delimiters="delimiters"
                v-model="tag.value.value as string[]"
                :error-messages="tag.errorMessage.value"
                multiple
                chips
                variant="outlined"
            ></v-combobox>
            <v-text-field
                clearable
                label="DOI"
                variant="solo"
                id="DOI_article"
                v-model="article.doi"
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