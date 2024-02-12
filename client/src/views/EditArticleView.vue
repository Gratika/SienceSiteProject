<script setup lang="ts">
import {onBeforeMount, onMounted, ref} from "vue";
import Swal from 'sweetalert2'
import {useArticleStore} from "@/stores/articleStore";
import RichTextEditor from "@/components/RichTextEditor.vue";
import {useRoute, useRouter} from "vue-router";
import type {IArticle} from "@/api/type";
import type { Ref} from "vue"
import {useField, useForm} from "vee-validate";
import MyLocalStorage from "@/services/myLocalStorage";
import {showErrorMessage} from "@/api/authApi";
import {useLayoutStore} from "@/stores/layoutStore";
import {useAuthStore} from "@/stores/authStore";
 //отримуємо id статті з роута
const  route = useRoute();
const router = useRouter();
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
const layoutStore = useLayoutStore();
const editorReadOnly = false;
const initContent = ref('');
const listFile=ref<string|null>(null);
const draftIsActive = ref(false);
const peopleId = MyLocalStorage.getItem('peopleId');
const isReady = ref(false);
onBeforeMount(()=>{
  if (!restoreLayout()) {
    if (typeof id === 'string') {
      articleStore.getArticle(id, peopleId).then((res) => {
        if (res !== undefined) {
          initialContent(res);
          listFile.value = substrUserFolder(res.path_file);
        }
      })
    }
  }
 });
onMounted(()=>{
  window.scroll(0,0)
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
function initialContent(res:IArticle){
  article.value = res;
  if (res.text != null)
    initContent.value = res.text;
  //console.log('initContent_Edit =', initContent.value)
  title.value.value = res.title;
  tag.value.value = res.tagItems;
  draftIsActive.value = res.isActive;
  isReady.value = true;

}
function restoreLayout():boolean{
  let saveData = layoutStore.restoreLayoutEditArticle();
  if (saveData !==null){
    initialContent(saveData);
    listFile.value = saveData.path_file;
    layoutStore.saveLayoutEditArticle(null);
    return true;
  }
  return false;
}
const validateArticle=()=>{
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
}
function saveLayout(saveData:IArticle){
  layoutStore.saveLayoutEditArticle(saveData);
}
//публікація відредагованної статті
const submitArticle= handleSubmit(()=>{//публікація
  validateArticle();
  article.value.isActive=true;
  //console.log(JSON.stringify(article))
  articleStore.updateArticle(article.value)
      .then((res)=>{
         router.push({ name: 'read_article', params: { id: article.value.id }});
      }).catch((error)=>{
          Swal.fire({
          icon: 'error',
          title: 'Помилка під час публікації статті',
          text: showErrorMessage(error)
          });
         if('name' in error
              && error.name==='AxiosError'
              && error.response?.status===401){
           article.value.isActive=false;
           saveLayout(article.value);
           useAuthStore().delUserData();
           router.push('/login');
         }
      });

})

//збереження відредагованної статті
const saveDraftArticle = handleSubmit(()=>{//зберегти чернетку
  validateArticle();
  //console.log(JSON.stringify(article))
  articleStore.updateArticle(article.value)
      .then((res)=>{
        Swal.fire({
          icon: 'info',
          title: res ? res : 'Статтю успішно збережено',
          text: ''
        });
      }).catch((error)=>{
         Swal.fire({
           icon: 'error',
           title: 'Помилка під час збереження статті',
           text: showErrorMessage(error)
         });
        if('name' in error
             && error.name==='AxiosError'
             && error.response?.status===401){
         saveLayout(article.value);
         useAuthStore().delUserData();
         router.push('/login');
        }
  });
});

//файли
const filesToUpload: Ref<FileList | null> = ref(null);
const handleFileChange = (event: Event & { target: HTMLInputElement & { files: FileList }}) => {
  filesToUpload.value = event.target.files;
  console.log('filesToUpload=',filesToUpload.value)
};

const uploadFiles = () => {
  if (!filesToUpload.value) {
    console.error('Будь ласка, оберіть файли для завантаження');
    return;
  }
  const id:string = article.value.id;
  const formData = new FormData();
  let newFileName:string='';
  formData.append('id', id);
  for (let i = 0; i < filesToUpload.value.length; i++) {
    formData.append('files', filesToUpload.value[i]);
    newFileName = newFileName.concat(filesToUpload.value[i].name,', ')
  }
  articleStore.saveFile(formData).then((res)=>{
    if (listFile.value===null){
      listFile.value=newFileName;
    }else{
      listFile.value=listFile.value+', '+newFileName;
    }
    Swal.fire({
      icon: 'info',
      title: res ? res : 'Файл успішно збережено',
      text: ''
    });
  }).catch((err)=>{
    Swal.fire({
      icon: 'error',
      title: 'Помилка збереження файлу',
      text: showErrorMessage(err)
    });
    if('name' in err
        && err.name==='AxiosError'
        && err.response?.status===401){
      useAuthStore().delUserData();
      router.push('/login');
    }
  })

}
function substrUserFolder(filePatch:string|null):string|null{
  if(filePatch===null || filePatch==='') return null;
  let pos = filePatch.indexOf(',');
  if (pos===-1) return null;
  else return filePatch.substring(pos+1);
}
</script>

<template>
  <v-container>
    <v-row>
      <v-col cols="12" class="my-10">
        <v-overlay :model-value="articleStore.isLoading"
                   class="align-center justify-center">
          <v-progress-circular
              indeterminate
              color="primary"
          ></v-progress-circular>
        </v-overlay>
        <v-card class="px-5 py-6 card-shadow">
          <v-card-title >
            <div class="d-flex justify-center w-100 text-h4 pb-4">
              Редагування статті
            </div>
          </v-card-title>
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
                  <span class="text-h6 d-inline">Текст статті</span>
                  <v-sheet border v-if="isReady" class="mt-2 pa-5 text-h6">
                    <RichTextEditor
                        :initialContent="initContent"
                        :is-read-only="editorReadOnly"
                        @save-content = "saveContent"
                    />
                  </v-sheet>
                </v-col>
              </v-row>
              <v-row v-if="listFile!=null">
                <v-col cols="12" class="pt-0">
                  <div class="text-file-list mt-2 ps-4 py-2 pe-1 border-file">{{listFile}}</div>
                </v-col>
              </v-row>
              <v-row>
                <v-col cols="8" md="5" sm="6">
                  <div class="d-flex justify-start flex-row">
                    <v-file-input
                        clearable
                        density="compact"
                        label="Завантажити документ"
                        show-size
                        variant="outlined"
                        @change="handleFileChange"
                        prepend-icon=''
                    >

                      <template #prepend-inner>
                        <v-icon>mdi-file-word-outline</v-icon>
                      </template>
                    </v-file-input>
                    <v-btn
                        class="ms-3 mt-1"
                        color="primary"
                        density="compact"
                        icon="mdi-check"
                        @click="uploadFiles"/>
                  </div>

                </v-col>
              </v-row>
              <v-row class="d-flex justify-end pa-4">
                <v-btn
                    color="black"
                    class="me-3"
                    :disabled="draftIsActive"
                    variant="outlined"
                    @click="saveDraftArticle"
                >
                  Зберегти чернетку
                </v-btn>
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
.border-file{
  border: solid 1px rgba(var(--v-border-color),var(--v-border-opacity));
  border-radius: 4px;
}
 .card-shadow{
   border-radius: 5px;
   box-shadow: 0 0 5px 0 rgba(0, 0, 0, 0.25);
 }
 .footer-distance{
   min-height: 100px;
 }
.v-file-input {
  flex-direction: row-reverse;

}
.text-file-list{
  font-size: 20px;
}
.ck.ck-toolbar{
  background: white;
}
.ck.ck-editor__main>.ck-editor__editable{
  background: white;
}

</style>