<script setup lang="ts">
import { onMounted, ref} from 'vue';
import { ClassicEditor } from '@ckeditor/ckeditor5-editor-classic';
import { Essentials } from '@ckeditor/ckeditor5-essentials';
import { Bold, Italic, Subscript, Superscript, Underline} from '@ckeditor/ckeditor5-basic-styles';
import { Link } from '@ckeditor/ckeditor5-link';
import { Paragraph } from '@ckeditor/ckeditor5-paragraph';
import { Font } from '@ckeditor/ckeditor5-font';
import {Alignment} from "@ckeditor/ckeditor5-alignment";
import {Heading} from "@ckeditor/ckeditor5-heading";
import {GeneralHtmlSupport} from "@ckeditor/ckeditor5-html-support";
import {List, ListProperties} from "@ckeditor/ckeditor5-list";
import {Table, TableToolbar} from "@ckeditor/ckeditor5-table";
import {BlockQuote} from "@ckeditor/ckeditor5-block-quote";
import {Image,  ImageResizeEditing, ImageResizeHandles, ImageInsert, AutoImage} from "@ckeditor/ckeditor5-image";
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
    default: '<p>Default initial content</p>'
  },
});

const emits = defineEmits(['save-content']);
const editorRef = ref<HTMLElement|null>(null);
let editorInstance: ClassicEditor ;
//const editor = ref(null);
const editorData = ref<string>(props.initialContent);
const token = MyLocalStorage.getItem('token');

onMounted(()=>{
 editorRef.value = document.querySelector('#editor');
 if (editorRef.value){
   ClassicEditor
       .create(editorRef.value, {
         plugins: [Essentials, GeneralHtmlSupport,Bold, Italic,Subscript, Superscript, Underline,
           BlockQuote, Font, Link, Paragraph,Alignment,Heading, List, ListProperties,SimpleUploadAdapter,
           Table, TableToolbar,Image, ImageResizeEditing, ImageResizeHandles, ImageInsert, AutoImage],
         fontSize: {
           options: [8,10,12,'default', 16, 18, 20],
           supportAllValues: true
         },
         list: {
           properties: {
             styles: true,
             startIndex: true,
             reversed: false
           }
         },
         table: {
           contentToolbar: [ 'tableColumn', 'tableRow', 'mergeTableCells' ]
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
         toolbar: {
           items: ['undo', 'redo','|','heading','|','bold', 'italic','underline','subscript', 'superscript', '|',
             'link', 'blockQuote','insertImage','insertTable','|',
             'fontFamily', 'fontSize', 'fontColor','|','alignment','bulletedList', 'numberedList','|'],
         }
       })
       .then(editor => {
         editorInstance = editor;
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
//плагін для налаштування редактора. Зобороняємо вкладати в комірку таблиці іншу таблицю
/*function DisallowNestingTables( editor:ClassicEditor ) {
  editor.model.schema.addChildCheck( ( context, childDefinition ) => {
    if ( childDefinition.name == 'table' && Array.from( context.getNames() ).includes( 'table' ) ) {
      return false;
    }
  } );
}
//заборона вкладати цитату в цитату
function DisallowNestingBlockQuotes( editor:ClassicEditor  ) {
  editor.model.schema.addChildCheck( ( context, childDefinition ) => {
    if ( context.endsWith( 'blockQuote' ) && childDefinition.name == 'blockQuote' ) {
      return false;
    }
  } );
}
const editorConfig = {
  //extraPlugins: [ DisallowNestingTables, DisallowNestingBlockQuotes ],
  plugins: [Essentials, GeneralHtmlSupport,Bold, Italic,Subscript, Superscript, Underline,
            BlockQuote, Font, Link, Paragraph,Alignment,Heading, List, ListProperties,SimpleUploadAdapter,
           Table, TableToolbar,Image, ImageResizeEditing, ImageResizeHandles, ImageInsert, AutoImage],
  fontSize: {
    options: [8,10,12,'default', 16, 18, 20],
    supportAllValues: true
  },
  list: {
    properties: {
      styles: true,
      startIndex: true,
      reversed: false
    }
  },
  table: {
    contentToolbar: [ 'tableColumn', 'tableRow', 'mergeTableCells' ]
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
  toolbar: {
    items: ['undo', 'redo','|','heading','|','bold', 'italic','underline','subscript', 'superscript', '|',
            'link', 'blockQuote','insertImage','insertTable','|',
            'fontFamily', 'fontSize', 'fontColor','|','alignment','bulletedList', 'numberedList','|'],
  },

}
*/


</script>

<template>
  <main id="sample">
    <div id="editor" class="ck-editor__editable_inline" v-html="editorData"/>


  </main>
</template>

<style scoped>
#sample {
  display: flex;
  flex-direction: column;
  flex-grow: 1;

}
</style>