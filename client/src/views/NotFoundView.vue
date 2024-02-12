<script setup lang="ts">
import { useRouter } from 'vue-router';

const router = useRouter();

const reloadPage = () => {
  router.go(0); // Викликає перезавантаження поточного маршруту
};
const goHome=() =>{
  router.push('/');
}
</script>

<template>
  <v-container>
    <v-row>
      <v-col sm="12" md="7" class="py-0">
        <div class="d-flex d-md-none mx-10 ">
          <v-img
              aspect-ratio="1/1"
              :width="700"
              cover
              src="notFound.svg"
          ></v-img>
        </div>
        <div class="d-none d-md-flex">
          <v-img
              aspect-ratio="1/1"
              :width="800"
              cover
              src="notFound.svg"
          ></v-img>
        </div>

      </v-col>
      <v-col sm="12" md="5">
        <div class="d-flex mx-10 mx-md-0 flex-column" >
          <div class=" mt-10 mb-2 text-sm-h1 font-weight-bold">404</div>
          <div class="md-2 mb-md-16 text-sm-h3 font-weight-medium">
            Упс... Здається, щось пішло не так...
          </div>
          <div class="pt-sm-2 pt-md-16">
            <div class="text-h4 font-weight-medium">
              <a @click="reloadPage"> Перезавантажте сторінку</a>
            </div>
            <div class="text-h4 font-weight-medium">
              або <a @click="goHome">перейдіть на головну</a>
            </div>
          </div>
        </div>

      </v-col>
    </v-row>
  </v-container>

</template>

<style scoped>
a{
  color: #5E8D64;
  cursor: pointer;
}

</style>