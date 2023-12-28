<script setup lang="ts">
import {computed, onMounted, ref} from "vue";
import {useRoute, useRouter} from "vue-router";
import MyLocalStorage from "@/services/myLocalStorage";


const router = useRouter();
const showLogin = ref(true);
const searchStr = ref('');

onMounted(()=>{
  showLogin.value = MyLocalStorage.getItem('isLogin')===false;

})
function goToUserOffice(){
  router.push({ name: 'user_office'});
}
const route = useRoute();
// Обчислюємо, чи поточний шлях є шляхом '/search/:search'
const isSearchResultPage = computed(() => {
  return route.path.startsWith('/search/');
});

</script>

<template>

      <v-toolbar class="pa-2" density="compact">
        <v-container class="my-container">
          <div class="w-25 d-inline-flex justify-start flex-wrap flex-row">
            <v-avatar   size="32" class="me-3 ms-6" image="Icon.png"></v-avatar>
            <!--v-avatar size="32" class="me-3 ms-6" >
              <v-img src="src/assets/images/icon.png" alt="Логотип"></v-img>
            </v-avatar-->
            <RouterLink to="/" >
              <v-toolbar-title class="text-decoration-underline">
                SciFindHub
              </v-toolbar-title>
            </RouterLink>
          </div>

          <div class="w-75 d-inline-flex align-center justify-end flex-wrap flex-row">
            <div v-if="isSearchResultPage" class="new-element">
            <input v-if="isSearchResultPage"
                   class="search-input"
                   v-model="searchStr"
            >
            <!--v-text-field
                class="my-1 me-6 rounded-0"
                label="Пошук за назвою, автором, темою..."
                single-line
                hide-details
                v-model="searchStr"
            ></v-text-field-->
            </div>
            <v-btn v-else
                   class="mx-1"
                   prepend-icon="mdi-magnify"
                   variant="text"
                   size="small">
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
                   variant="outlined"
                   size="small"
            >
              Вхід
            </v-btn>
            <v-avatar v-else image="avatar.png" size="small" @click="goToUserOffice"></v-avatar>
          </div>
        </v-container>
        <!--div class="my-toolbar"-->



        <!--/div-->
      </v-toolbar>

</template>

<style scoped>
.new-element{
  display: flex;
  justify-content: center;
  margin-left: auto;
  margin-right: auto;
  width: 60%;

}
.my-container{
  align-content: center;
  display: flex;
  flex-wrap: wrap;
}
.search-input{
  height: 35px;
  border-radius: 2px;
  background: #D9D9D9;
}


</style>