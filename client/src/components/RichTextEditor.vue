<script setup lang="ts">
import { ref } from 'vue';
import { ClassicEditor } from '@ckeditor/ckeditor5-editor-classic';
import { Essentials } from '@ckeditor/ckeditor5-essentials';
import { Bold, Italic} from '@ckeditor/ckeditor5-basic-styles';
import { Link } from '@ckeditor/ckeditor5-link';
import { Paragraph } from '@ckeditor/ckeditor5-paragraph';
import { Font } from '@ckeditor/ckeditor5-font';
import {Alignment} from "@ckeditor/ckeditor5-alignment";
import {Heading} from "@ckeditor/ckeditor5-heading";
import {GeneralHtmlSupport} from "@ckeditor/ckeditor5-html-support";

const editor = ClassicEditor;
const editorData = ref<string>('<p>Content of the editor.</p>');
const editorConfig = {
  plugins: [Essentials, GeneralHtmlSupport,Bold, Italic, Font, Link, Paragraph,Alignment,Heading],
  fontSize: {
    options: [8,10,12,'default', 16, 18, 20],
    supportAllValues: true
  },
  toolbar: {
    items: ['bold', 'italic', 'link', 'undo', 'redo',
      'fontSize', 'fontFamily', 'fontColor','alignment','heading'],
  },
};
</script>

<template>
  <main id="sample">
    <ckeditor  class="ck-editor__editable_inline" :editor="editor" v-model="editorData" :config="editorConfig"></ckeditor>
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