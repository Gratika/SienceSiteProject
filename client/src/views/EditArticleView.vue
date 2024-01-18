<script setup lang="ts">
import {isReadonly, onBeforeMount, ref} from "vue";
import {useArticleStore} from "@/stores/articleStore";
import RichTextEditor from "@/components/RichTextEditor.vue";
import {useRoute} from "vue-router";
import type {IArticle} from "@/api/type";
import type { Ref} from "vue"
import {useField, useForm} from "vee-validate";
import MyLocalStorage from "@/services/myLocalStorage";
 //отримуємо id статті з роута
const  route = useRoute();
const {id} = route.params;
//модель статті
const article = ref<IArticle>({
  id: '',
  doi: null,
  author_id: '',
  title: '',
  tag: '',
  text: '',
  views: 0,
  date_created:null,
  modified_date: null,
  theory_id: '',
  theory_:null,
  path_file: '',
  author_: null,
  tagItems:[],
  reaction: null,
  countLike:0,
  selected:false,
  isActive:false,
})
const delimiters = ['#',','] //масив рядків, що будуть створювати новий тег при вводі
const articleStore = useArticleStore();
 const editorReadOnly = false;
 const initContent = ref('');
 onBeforeMount(()=>{
   const data = articleStore.articles.find(art => art.id ===id);
   if (data){
     article.value = data
     if(data.text!=null)
      initContent.value=data.text;
     console.log('initContent_Edit =',initContent.value)
     title.value.value = data.title;
     tag.value.value= data.tagItems;
   }
 })

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
    }
  },
})
const title= useField('title');
const  tag = useField('tag');
function saveContent(articleText: string){
  article.value.text = articleText

}
//збереження відредагованної статті
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
  article.value.isActive=true;
  console.log(JSON.stringify(article))
  articleStore.updateArticle(article.value);

})
const saveDraftArticle = handleSubmit(()=>{});

//файли
const filesToUpload: Ref<FileList | null> = ref(null);
const handleFileChange = (event: Event & { target: HTMLInputElement & { files: FileList }}) => {
  filesToUpload.value = event.target.files;
};

const uploadFiles = () => {
  if (!filesToUpload.value) {
    console.error('Будь ласка, оберіть файли для завантаження');
    return;
  }
  const id:string = article.value.id;
  const formData = new FormData();
  formData.append('id', id);
  for (let i = 0; i < filesToUpload.value.length; i++) {
    formData.append('files', filesToUpload.value[i]);
  }
  articleStore.saveFile(formData);

}
</script>

<template>
  <v-container>
    <v-row>
      <v-col cols="12" class="my-10">
        <v-card class="px-5 py-4 card-shadow">
          <v-card-text>
            <v-form @submit.prevent="submitArticle">
              <v-row >
                <v-col cols="12" class="pt-0">
                  <span class="text-h6 d-inline">Назва</span><h3 class="d-inline text-red"><sup>*</sup></h3>
                  <v-text-field
                      class="mt-1"
                      clearable
                      label="Назва статті"
                      id="title"
                      v-model="title.value.value"
                      :error-messages="title.errorMessage.value"
                  />
                  <span class="text-h6">DOI-ідентифікатор</span>
                  <v-text-field
                      class="mt-2"
                      clearable
                      label="DOI"
                      id="DOI_article"
                      v-model="article.doi"
                  />
                  <span class="text-h6 d-inline">Теги</span><h3 class="d-inline text-red"><sup>*</sup></h3>
                  <v-combobox
                      class="mt-2"
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
                  <v-sheet border>
                    <RichTextEditor
                        :initialContent="initContent"
                        :is-read-only="editorReadOnly"
                        @save-content = "saveContent"
                    />
                  </v-sheet>
                </v-col>
              </v-row>
              <v-row>
                <v-col cols="6">
                  <div class="d-flex align-content-center">
                    <v-file-input
                        clearable
                        density="compact"
                        label="Завантажити документ"
                        show-size
                        variant="outlined"
                        @change="handleFileChange"
                        :prepend-icon=undefined
                    >

                      <template #prepend-inner>
                        <v-icon>mdi-file</v-icon>
                      </template>
                    </v-file-input>
                    <v-btn class="ms-3" color="black" variant="tonal" density="compact" icon="mdi-check" @click="uploadFiles"/>
                  </div>

                </v-col>
              </v-row>
              <v-row class="d-flex justify-end pa-4">
                <v-btn @click="saveDraftArticle" variant="outlined" color="black" class="me-3">Зберегти чернетку</v-btn>
                <v-btn type="submit">Опублікувати</v-btn>

              </v-row>
            </v-form>
          </v-card-text>
        </v-card>
      </v-col>
    </v-row>
    <v-row>
      <div class="footer-distance"></div>
    </v-row>
  </v-container>
</template>

<style scoped>
 .card-shadow{
   border-radius: 5px;
   box-shadow: 0 0 5px 0 rgba(0, 0, 0, 0.25);
 }
 .footer-distance{
   min-height: 100px;
 }
</style>