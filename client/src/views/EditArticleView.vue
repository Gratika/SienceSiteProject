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
  countLike:0
})
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
     tag.value.value= data.tag;
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
  if (typeof tag.value.value === "string") {
    article.value.tag = tag.value.value;
  }
  console.log(JSON.stringify(article))
  articleStore.updateArticle(article.value);

})

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
        <v-col cols="5" >
          <v-text-field
              clearable
              label="Теги"
              variant="solo"
              id="teg_article"
              v-model="tag.value.value"
              :error-messages="tag.errorMessage.value"
          />
        </v-col>
        <v-col cols="5">
          <v-text-field
              clearable
              label="DOI"
              variant="solo"
              id="DOI_article"
              v-model="article.doi"
          />
        </v-col>
        <v-col cols="2">
          <v-file-input
              clearable
              label="Завантажити файли"
              variant="solo-inverted"
              @change="handleFileChange"
          ></v-file-input>
          <v-btn density="compact" icon="mdi-check" @click="uploadFiles"/>
        </v-col>
      </v-row>
      <v-row class="justify-center">
        <v-col cols="10" class="justify-center align-center">
          <RichTextEditor
              :initialContent="initContent"
              :is-read-only="editorReadOnly"
              @save-content = "saveContent"
          />
        </v-col>
      </v-row>
      <v-row>
        <v-btn>Відмінити</v-btn>
        <v-btn type="submit">Зберегти</v-btn>
      </v-row>
    </v-form>
  </v-container>
</template>

<style scoped>

</style>