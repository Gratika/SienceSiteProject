<script setup lang="ts">
import {QuillEditor} from '@vueup/vue-quill'
import '@vueup/vue-quill/dist/vue-quill.snow.css';
import {ref} from "vue";

const toolbarOptions = [
  [{'font': []}], //шрифт
  [{'header': [1, 2, 3, 4, 5, 6, false]}],//заголовки (розкривний список)
  [{ 'size': ['small', false, 'large', 'huge'] }],
  ['bold', 'italic', 'underline'], //кнопки форматування тексту
  [{'script': 'sub'}, {'script': 'super'}], //надстрочні та підстрочні символи
  [{'align': []}, {'list': 'ordered'}, {'list': 'bullet'}], //вирівнювання,списки
  [{'indent': '-1'}, {'indent': '+1'}],          // збільшити/зменшити відступ
  [{'color': []}, {'background': []}],          // розкривне меню з кольорами тексту та фону
  ['blockquote', 'link','image'], //цитата, посилання, зображення
  ['clean']                                         // кнопка Скинути формат
];
console.log(' компонент QuillEditor')
let editorContent = ref(''); //тут буде текст статті
//метод,який буде викликатися при зміні тексту в редакторі
//const onEditorContentUpdate = (e) => {
  //console.log(e);
//};
function onEditorChange({ quill, html, text }: { quill: any; html: string; text: string }) {
  console.log('onEditorChange')
  console.log("text", text);
  console.log("htmlContent", html);
  editorContent.value= quill.getContents();
}
const saveEditorContent = () => { // Відправити editorContent на сервер або зробити іншу обробку
  console.log(editorContent.value);
};
</script>

<template>
  <div>
    <QuillEditor theme="snow" :toolbar="toolbarOptions" v-model:content="editorContent" @change="onEditorChange($event)"/>
    <button @click="saveEditorContent">Зберегти</button>
  </div>

</template>

<style scoped>

</style>