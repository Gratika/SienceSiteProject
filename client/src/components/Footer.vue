<script setup lang="ts">
import {ref} from "vue";
import {useRouter} from "vue-router";

const router = useRouter();
const support = ref<Element|undefined>()
function openEmailClient() {
  let email='example@example.com'
  if (support.value!==undefined)
      if(support.value?.textContent!==null)  email = support.value.textContent;
  // Відкриваємо поштовий клієнт
  window.location.href = `mailto:${email}`;
}
function aboutUs(){
  router.push({ name: 'developmentPage'});
}
function outBlog(){
  router.push({ name: 'developmentPage'});
}
function goCategoryList(){
  router.push({ name: 'category_list'});
}
function goPopular(){
  router.push({ name: 'developmentPage'});
}

</script>

<template>
<div class="my-footer px-sm-8 px-md-16 py-10">
  <v-row class="h-100 justify-space-between">
    <v-col cols="12" sm="5" md="4" class="d-flex h-100 flex-column justify-space-between">
      <div ref="support" class="support-text cursor-pointer" @click="openEmailClient">support@scifindhub.ua</div>
      <div>
        <div class="text-h4 font-weight-bold mb-3">SciFindHub</div>
        <div class="text-h5">&copy2024 Всі права захищені</div>
      </div>
    </v-col>
    <v-col cols="4" class="d-none menu-text h-100 d-md-flex flex-column justify-space-around align-center">
      <div @click="aboutUs" class="cursor-pointer">Про нас</div>
      <div @click="outBlog" class="cursor-pointer">Блог</div>
      <div @click="goCategoryList" class="cursor-pointer">Категорії</div>
      <div @click="goPopular" class="cursor-pointer">Популярне</div>
    </v-col>
    <v-col cols="12" sm="5" md="3" class="d-flex h-100 flex-column justify-space-between align-end">
      <div class="d-flex flex-row">
        <v-avatar class="me-3" size="50">
          <v-img
              src="telegram.png"
              alt="T"
          ></v-img>
        </v-avatar>
        <v-avatar class="me-3" size="50">
          <v-img
              src="linkedin_1.png"
              alt="L"
          ></v-img>
        </v-avatar>
        <v-avatar class="me-3" size="50">
          <v-img
              src="github.png"
              alt="G"
          ></v-img>
        </v-avatar>
      </div>
      <div class="d-flex justify-end">
          <div class="ps-sm-0 ps-lg-14 text-h5 text-end">Права на всі статті, опубліковані на сайті, належать авторам.</div>

      </div>
    </v-col>

  </v-row>

</div>
</template>

<style scoped>

.cursor-pointer{
  cursor: pointer;
}
.menu-text{
  font-size: 30px;
}
.my-footer{
  background-color: #E2E2E2;
  height: 350px;
  min-height: 250px;
  min-width: 800px;
  width: 100%;
}
.support-text{
  font-size: 26px;
}
</style>