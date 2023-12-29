<script setup lang="ts">
import {computed, onMounted, ref, watch} from "vue";
import {useRoute, useRouter} from "vue-router";
import MyLocalStorage from "@/services/myLocalStorage";


const router = useRouter();
const showLogin = ref(true);
const showSearchStr = ref(false);
const searchStr = ref('');

onMounted(()=>{
  showLogin.value = MyLocalStorage.getItem('isLogin')===false;
})
function goToUserOffice(){
  router.push({ name: 'user_office'});
}

const route = useRoute();
// Обчислюємо, чи поточний шлях є шляхом '/search/:search'
//потрібно для відображення/приховування пошукового рядка
const isSearchResultPage = computed(() => {
  return route.path.startsWith('/search/');
});
//отримуємо пошуковий запит для відображення у рядку пошуку
const searchParam = computed(() => route.params.search || '');
// Слідкування за змінами параметра маршруту 'search'
watch(() => route.params.search, (newValue) => {
  if (typeof newValue === "string")  searchStr.value = newValue || '';
});

function onSearch(){
  console.log('searchStr=', searchStr.value)
  router.push({ name: 'search_article', params: { search: searchStr.value} });
}
function onShowSearchStr(){
  showSearchStr.value=true;
}
</script>

<template>

      <v-toolbar class="pa-2" density="compact">
        <v-container class="my-container">
          <div class="d-inline-flex justify-start align-center flex-wrap flex-row">
            <v-avatar   size="32" class="me-3" image="Icon.png"></v-avatar>
            <RouterLink to="/" >
              <v-toolbar-title class="me-3">
                SciFindHub
              </v-toolbar-title>
            </RouterLink>
          </div>
          <div class="new-element flex-grow-1">
            <v-text-field v-if="isSearchResultPage || showSearchStr"
                class="search-input flex-grow-0"
                density="compact"
                append-inner-icon="mdi-magnify"
                single-line
                hide-details
                v-model="searchStr"
                @click:append-inner="onSearch"
            ></v-text-field>
          </div>

          <div class="d-inline-flex align-center justify-end flex-wrap flex-row">
            <v-btn v-if="!isSearchResultPage && !showSearchStr"
                   class="mx-3"
                   prepend-icon="mdi-magnify"
                   variant="text"
                   size="small"
                   @click="onShowSearchStr"
            >
              Пошук
            </v-btn>
            <v-btn
                class="mx-1"
                variant="text"
                size="small"
                disabled
            >
              Блог
            </v-btn>
            <v-btn
                class="mx-1"
                variant="text"
                size="small">
              Про нас
            </v-btn>

            <v-btn v-if="showLogin"
                   class="mx-1"
                   to="/login"
                   size="small"
            >
              Вхід
            </v-btn>
            <v-avatar v-else image="avatar.png" size="small" class="mx-1" @click="goToUserOffice"></v-avatar>
          </div>
        </v-container>
      </v-toolbar>

</template>

<style scoped>
.new-element{
  display: flex;
  justify-content: center;

}
.my-container{
  align-content: center;
  display: flex;
  flex-wrap: wrap;
}
.search-input{
  border-radius: 2px;
  background: #D9D9D9;
  margin-left: auto;
  margin-right: auto;
  width: 75%;
}


</style>