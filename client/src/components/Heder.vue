<script setup lang="ts">
import {ref} from "vue";
import {useRouter} from "vue-router";

const router = useRouter();
const searchStr = ref('');
function onSearch(){
  router.push({ name: 'search_article', params: { search: searchStr.value } });
}
</script>

<template>
<div class="header">
  <v-container>
    <v-row class="justify-center">
      <v-col cols="7">
        <div class="text-h3 font-weight-bold mb-7">Discovery Science, Uncover Knowledge with SciFindHub</div>
        <div class="py-7">
          <v-text-field
              label="Пошук за назвою, автором, темою..."
              class="custom-text-field"
              single-line
              hide-details
              v-model="searchStr"

          ></v-text-field>
        </div>

        <v-btn
            class="round-btn px-6"
            @click="onSearch">Пошук</v-btn>
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
    <v-row class="mt-6">
      <v-col cols="12">
        <div class="d-flex flex-row justify-space-between">
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
.round-btn{
  border-radius: 2px;
}
</style>