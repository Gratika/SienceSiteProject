<script setup lang="ts">
import {ref} from "vue";
import {useRouter} from "vue-router";

const router = useRouter();
const searchStr = ref('');
function onSearch(){
  router.push({ name: 'search_article', params: { search: searchStr.value } });
}
function handleKeyDown(event:KeyboardEvent) {
  if (event.key === 'Enter') {
    console.log('Key pressed:', event.key);
    onSearch();
  }
}
</script>

<template>
<div class="header">
  <v-container>
    <v-row class="justify-center">
      <v-col cols="7">
        <div class="text-h3 font-weight-bold pb-12 mb-12">Discovery Science, Uncover Knowledge with SciFindHub</div>
        <div class="mb-8">
          <v-text-field
              label="Пошук за назвою, автором, темою..."
              class="custom-text-field"
              single-line
              hide-details
              v-model="searchStr"
              @keydown="handleKeyDown"

          ></v-text-field>
        </div>

        <v-btn
            class="round-btn px-6 text-h6"
            @click="onSearch"
        >Пошук
        </v-btn>
      </v-col>
      <v-col cols="5">
        <v-img
            :width="500"
            aspect-ratio="1/1"
            cover
            src="mainFrame.png"
        ></v-img>
      </v-col>
    </v-row>
    <v-row class="py-15 mb-15" >
      <v-col cols="12">
        <div class="d-flex flex-row justify-space-between total-text-size">
          <div class="d-flex flex-column align-start">
            <div>
              00
            </div>
            <div>
              мов
            </div>
          </div>
          <div class="d-flex flex-column align-start">
            <div>
              00
            </div>
            <div>
              країн
            </div>
          </div>
          <div class="d-flex flex-column align-start">
            <div>
              00
            </div>
            <div>
              статей
            </div>
          </div>
          <div class="d-flex flex-column align-start">
            <div>
              00
            </div>
            <div>
              наукових статей
            </div>
          </div>
        </div>
      </v-col>
    </v-row>
  </v-container>
</div>
</template>

<style scoped>

.custom-text-field {
  border: 1px solid black; /* Додати границю 1px чорного кольору */
  border-radius: 0; /* Скруглення = 0 */
}
.header{
  padding: 100px 0;
}
.total-text-size{
  font-size: 32px!important;
  font-style: normal;
  font-weight: 400;
  line-height: normal;
  letter-spacing: 1.02px;
}

.round-btn{
  border-radius: 2px;
}
</style>