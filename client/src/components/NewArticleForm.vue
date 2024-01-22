<script setup lang="ts">
import type {IArticleResponse, IArticle, IPeople, IScience, IScientificTheory, IUser} from "@/api/type";
import {onMounted, ref, watch} from "vue";
import {useField, useForm} from "vee-validate";
import {useArticleStore} from "@/stores/articleStore";
import MyLocalStorage from "@/services/myLocalStorage";
import {createToast} from "mosha-vue-toastify";
import MySnackbars from "@/components/MySnackbars.vue";


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
let scienceSectionList_=ref <Array<IScientificTheory>>([]) //відфільтрований масив з розділами наукової сфери
//нова стаття
const article = ref<IArticle>({
  id: '',
  doi: null,
  author_id: getAuthorId(),
  title: '',
  tag: '',
  text: '',
  views: 0,
  date_created:'',
  modified_date: '',
  theory_id: '',
  theory_:null,
  path_file: '',
  author_: getAuthor(),
  tagItems:[],
  reaction: null,
  countLike:0,
  selected:false,
  isActive: false,
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
  let res: null|IPeople = null;
  let user =  MyLocalStorage.getItem('user')
  if(user!==null && user.people_!==undefined){
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
watch([scienceId],([newScienceId])=>{
  console.log("scienceId=",scienceId,' newValue=',newScienceId);
  if (newScienceId!=null && newScienceId!=''){
    isEditing.value = true;
    console.log('props.scienceSectionList=',props.scienceSectionList)
    scienceSectionList_.value = props.scienceSectionList.filter(s=>s.science_id.trim()==newScienceId.trim())
    console.log('scienceSectionList_=',scienceSectionList_.value)
  }else isEditing.value = false;
})
watch([theory_id],([newTheory_id])=>{
  if (newTheory_id && newTheory_id.value.value!=null && newTheory_id.value.value!=''){
    console.log('newTheory_id', newTheory_id.value.value)
    scienceTheory.value = props.scienceSectionList.find(s=>s.id==newTheory_id.value.value)
    console.log('scienceTheory=',scienceTheory.value)
  }
})
//збереження статті
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
  });

  article.value.tag = processTags(tagString);//прибираємо в кінці рядка можливі розділові знаки

  if (typeof theory_id.value.value === "string") {
    article.value.theory_id = theory_id.value.value;
  }
  if (scienceTheory.value)  article.value.theory_=scienceTheory.value;
  article.value.date_created= (new Date()).toISOString();
  article.value.modified_date= (new Date()).toISOString();
  console.log('article=', article.value);
  articleStore.saveArticle(article.value)
      .then((res:IArticleResponse|undefined)=>{
        emits('close', dialogShow.value, res?.articleId);
      })
      .catch((err)=>console.log('Axios Error: ', err));

})
function processTags(data:string|null):string|null {
  if (data !== null) {
    if (data.endsWith(',') || data.endsWith('#'))
      return data.substring(0, data.length - 1);
    else return data;
  }
  return null
}
function closeDialog() {
  emits('close', dialogShow.value);
}
</script>


<template>
  <v-container>
    <v-row>
      <v-col cols="12" class="py-0">
        <v-card class="px-5 py-2">
          <v-card-text>
            <v-form @submit.prevent="submitArticle">
              <v-row class="pt-3 pb-2">
                <span class="text-h6 d-inline">Наукові сфера та розділ</span><h1 class="d-inline text-red"><sup>*</sup></h1>
              </v-row>
              <v-row>
                <v-col cols="6" >
                  <v-autocomplete
                      density="comfortable"
                      label="Наукова сфера"
                      :items="scienceList"
                      v-model="scienceId"
                      item-title="name"
                      item-value="id"
                      variant="outlined"
                  > </v-autocomplete>
                </v-col>
                <v-col cols="6">
                  <v-autocomplete
                      density="comfortable"
                      :disabled="!isEditing"
                      item-title="name"
                      item-value="id"
                      :items="scienceSectionList_"
                      label="Розділ"
                      variant="outlined"
                      v-model="theory_id.value.value"
                      :error-messages="theory_id.errorMessage.value"
                  ></v-autocomplete>
                </v-col>
              </v-row>
              <v-row >
                <v-col cols="12" class="pt-0">
                  <span class="text-h6 d-inline">Назва</span><h1 class="d-inline text-red"><sup>*</sup></h1>
                  <v-text-field
                      class = "my-3"
                      clearable
                      label="Назва статті"
                      id="title"
                      v-model="title.value.value"
                      :error-messages="title.errorMessage.value"
                  />
                  <span class="text-h6">DOI-ідентифікатор</span>
                  <v-text-field
                      class="my-3"
                      clearable
                      label="DOI"
                      id="DOI_article"
                      v-model="article.doi"
                  />
                  <span class="text-h6 d-inline">Теги</span><h1 class="d-inline text-red"><sup>*</sup></h1>
                  <v-combobox
                      class="my-3"
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
                </v-col>
              </v-row>
              <v-row class="d-flex justify-end pa-4">
                <v-btn @click="closeDialog" variant="outlined" color="black" class="me-3">Скасувати</v-btn>
                <v-btn type="submit">Зберегти</v-btn>

              </v-row>
            </v-form>
          </v-card-text>
        </v-card>
      </v-col>
    </v-row>

  </v-container>



</template>

<style scoped>

</style>