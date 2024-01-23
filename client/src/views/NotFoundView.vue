<script setup lang="ts">

</script>

<template>
  <v-container>
    <v-row>
      <v-col xs="12" sm="7" class="py-0">
        <v-img
            aspect-ratio="1/1"
            :width="800"
            cover
            src="notFound.svg"
        ></v-img>
      </v-col>
      <v-col xs="12" sm="5">
        <div class=" mt-10 mb-2 text-sm-h1 font-weight-bold">404</div>
        <div class="mb-16 text-sm-h3 font-weight-medium">
          Упс... Здається, щось пішло не так...
        </div>
        <div class="pt-16">
          <div class="text-h4 font-weight-medium">
            Перезавантажте сторінку
          </div>
          <div class="text-h4 font-weight-medium">
            або перейдіть на головну
          </div>
        </div>

      </v-col>
    </v-row>
  </v-container>

</template>

<style scoped>

</style>