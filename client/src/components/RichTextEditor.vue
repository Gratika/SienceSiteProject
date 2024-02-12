<script setup lang="ts">
import {onBeforeUpdate, onMounted, onUpdated, ref, watch} from 'vue';
import { ClassicEditor } from '@ckeditor/ckeditor5-editor-classic';
import { Essentials } from '@ckeditor/ckeditor5-essentials';
import { FindAndReplace } from '@ckeditor/ckeditor5-find-and-replace';
import { FontColor } from '@ckeditor/ckeditor5-font';
import { CodeBlock } from '@ckeditor/ckeditor5-code-block';
import { Bold, Italic, Subscript, Superscript, Underline,Code} from '@ckeditor/ckeditor5-basic-styles';
import { Font } from '@ckeditor/ckeditor5-font';
import {Alignment} from "@ckeditor/ckeditor5-alignment";
import {Heading} from "@ckeditor/ckeditor5-heading";
import { HorizontalLine } from '@ckeditor/ckeditor5-horizontal-line';
import {BlockQuote} from "@ckeditor/ckeditor5-block-quote";
import {
  AutoImage,
  Image,
  ImageCaption,
  ImageInsert,
  ImageResize,
  ImageStyle,
  ImageToolbar,
  ImageUpload
} from '@ckeditor/ckeditor5-image';
import { Indent, IndentBlock } from '@ckeditor/ckeditor5-indent';
import { Link, LinkImage } from '@ckeditor/ckeditor5-link';
import { List } from '@ckeditor/ckeditor5-list';
import { Paragraph } from '@ckeditor/ckeditor5-paragraph';
import { PasteFromOffice } from '@ckeditor/ckeditor5-paste-from-office';
import { RemoveFormat } from '@ckeditor/ckeditor5-remove-format';
import { SelectAll } from '@ckeditor/ckeditor5-select-all';
import {
  SpecialCharacters,
  SpecialCharactersArrows,
  SpecialCharactersCurrency,
  SpecialCharactersEssentials,
  SpecialCharactersLatin,
  SpecialCharactersMathematical,
  SpecialCharactersText
} from '@ckeditor/ckeditor5-special-characters';
import {
  Table,
  TableCaption,
  TableCellProperties,
  TableColumnResize,
  TableProperties,
  TableToolbar
} from '@ckeditor/ckeditor5-table';
import { TextTransformation } from '@ckeditor/ckeditor5-typing';
import { Undo } from '@ckeditor/ckeditor5-undo';

import {SimpleUploadAdapter} from "@ckeditor/ckeditor5-upload";
import MyLocalStorage from "@/services/myLocalStorage";
import '@/styles/editorReadonlyCustom.css'

const props = defineProps({
  isReadOnly:{
    type:Boolean,
    required:true
  },
  initialContent: {
    type: String,
    default: '<p>Напишіть тут щось</p>'
  },
});

const emits = defineEmits(['save-content']);
const editorRef = ref<HTMLElement|null>(null);
let editorInstance: ClassicEditor ;
//const editor = ref(null);
const editorData = ref<string>('');
const token = MyLocalStorage.getItem('token');
const editorConfig = {
  plugins:[
    Alignment,
    AutoImage,
    BlockQuote,
    Bold,
    Code,
    CodeBlock,
    Essentials,
    FindAndReplace,
    FontColor,
    Heading,
    HorizontalLine,
    Image,
    ImageCaption,
    ImageInsert,
    ImageResize,
    ImageStyle,
    ImageToolbar,
    ImageUpload,
    Indent,
    IndentBlock,
    Italic,
    Link,
    LinkImage,
    List,
    Paragraph,
    PasteFromOffice,
    RemoveFormat,
    SelectAll,
    SimpleUploadAdapter,
    SpecialCharacters,
    SpecialCharactersArrows,
    SpecialCharactersCurrency,
    SpecialCharactersEssentials,
    SpecialCharactersLatin,
    SpecialCharactersMathematical,
    SpecialCharactersText,
    Subscript,
    Superscript,
    Table,
    TableCaption,
    TableCellProperties,
    TableColumnResize,
    TableProperties,
    TableToolbar,
    TextTransformation,
    Underline,
    Undo
  ],
  toolbar: {
    items: [
      'heading',
      '|',
      'undo',
      'redo',
      'findAndReplace',
      'link',
      'selectAll',
      'removeFormat',
      '|',
      'bold',
      'italic',
      'underline',
      'subscript',
      'superscript',
      'fontColor',
      '|',
      'alignment',
      'outdent',
      'indent',
      'bulletedList',
      'numberedList',
      '|',
      'horizontalLine',
      'blockQuote',
      'insertTable',
      'imageInsert',
      'specialCharacters',
      'codeBlock'
    ]
  },
  language: 'ru',
  image: {
    toolbar: [
      'imageTextAlternative',
      'toggleImageCaption',
      'imageStyle:inline',
      'imageStyle:block',
      'imageStyle:side',
      'linkImage'
    ]
  },
  table: {
    contentToolbar: [
      'tableColumn',
      'tableRow',
      'mergeTableCells',
      'tableCellProperties',
      'tableProperties'
    ]
  },
  simpleUpload: {
    // URL-адреса, на яку завантажуються зображення.
    uploadUrl: '/api/Images/AddImages',
    withCredentials: true,
    // Заголовки, надіслані разом із XMLHttpRequest на сервер завантаження.
    headers: {
      'X-CSRF-TOKEN': 'CSRF-Token',
      Authorization: `Bearer ${token}`,

    }
  },
}

onMounted(()=>{
 editorData.value=props.initialContent;
 console.log('editorData=',editorData.value)
 editorRef.value = document.querySelector('#editor');
 if (editorRef.value){
   ClassicEditor
       .create(editorRef.value, editorConfig)
       .then(editor => {
         editorInstance = editor;
         editorInstance.setData(editorData.value);
         if (editorInstance && editorInstance.ui.view.editable.element){
           editorInstance.ui.view.editable.element.style.border='none';

         }
           if (editorInstance && editorInstance.ui.view.toolbar.element) {
           editorInstance.ui.view.toolbar.element.style.display = props.isReadOnly ? 'none' : 'flex';
           console.log('props.isReadOnly=',props.isReadOnly)
           if (props.isReadOnly) {
             editorInstance.enableReadOnlyMode('editor');
           }
           else{
             editorInstance.disableReadOnlyMode('editor');
           }
         }

         editorInstance.model.document.on('change:data', () => {
           editorData.value=editorInstance.getData();
           emits('save-content', editorData.value);
         });
         //editorInstance.enableReadOnlyMode('editor');
         //editorInstance.model.document.isReadOnly=props.isReadOnly;

       })
       .catch(error => {
         console.error('There was a problem initializing the editor:', error);
       });
 }
})
onUpdated(()=>{
  if (props.isReadOnly===true &&  props.initialContent!==undefined &&props.initialContent!=='') {
    console.log('Update')
    editorData.value = props.initialContent;
    if (editorInstance)
      editorInstance.setData(editorData.value);
  }
})
/*onBeforeUpdate(()=>{
  console.log('onBeforeUpdate')
  editorData.value=props.initialContent;
  if (editorInstance)
    editorInstance.setData(editorData.value);
})*/
/*watch(() => props.initialContent, (newValue, oldValue) => {
  console.log('Значення пропса inputData змінилося з', oldValue, 'на', newValue);

});*/
</script>

<template>
  <main id="sample">
    <div id="editor"   v-html="editorData"/>

  </main>
</template>

<style scoped>
#sample {
  display: flex;
  flex-direction: column;
  flex-grow: 1;
  font-size: 20px;
  min-height: 500px;
  line-height: 1.5rem;

}

.ck.ck-editor__main>.ck-editor__editable{
  background: white;
}
</style>