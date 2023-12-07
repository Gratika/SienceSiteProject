<script setup lang="ts">
import { ref } from 'vue';
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

const props = defineProps({
  initialContent: {
    type: String,
    default: '<p>Default initial content</p>'
  },
});
const emits = defineEmits(['save-content']);
const editorRef = ref();//посилання на DOM-елемент CKEditor для подальшого доступу до нього
const editor = ClassicEditor;
const editorData = ref<string>(props.initialContent);
//плагін для налаштування редактора. Зобороняємо вкладати в комірку таблиці іншу таблицю
function DisallowNestingTables( editor:ClassicEditor ) {
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
  //extraPlugins: [ 'DisallowNestingTables', 'DisallowNestingBlockQuotes' ],
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
    uploadUrl: 'http://example.com',
    withCredentials: true,
    // Заголовки, надіслані разом із XMLHttpRequest на сервер завантаження.
    headers: {
      'X-CSRF-TOKEN': 'CSRF-Token',
      Authorization: 'Bearer <JSON Web Token>'
    }
  },
  toolbar: {
    items: ['undo', 'redo','|','heading','|','bold', 'italic','underline','subscript', 'superscript', '|',
            'link', 'blockQuote','insertImage','insertTable','|',
            'fontFamily', 'fontSize', 'fontColor','|','alignment','bulletedList', 'numberedList','|'],
  },
};
// Метод для отримання вмісту редактора при втраті фокусу
function onEditorBlur (editor:ClassicEditor) {
  console.log('Вміст редактора при втраті фокусу:', editorData);
  emits('save-content', editorData.value);
  //const editorInstance = editorRef.value;
  /*if (editor) {
    const editorContent = editor.getData();
    console.log('Вміст редактора при втраті фокусу:', editorContent);
    emits('save-content', editorContent)
    // Тут ви можете використовувати editorContent по вашому вибору
  }*/
};
</script>

<template>
  <main id="sample">
    <ckeditor
        ref="editorRef"
        class="ck-editor__editable_inline"
        :editor="editor"
        v-model="editorData"
        :config="editorConfig"
        @blur="onEditorBlur"
    ></ckeditor>
  </main>
</template>

<style scoped>
#sample {
  display: flex;
  flex-direction: column;
  place-items: center;
  width: 100%;
}

.ck-editor__editable_inline {
  width: 100%;
  min-height: 300px; /* змініть потрібну висоту редактора */
  border: 1px solid #ccc; /* додайте рамку, якщо потрібно */
}
</style>