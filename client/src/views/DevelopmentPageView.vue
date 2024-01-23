<script setup lang="ts">

</script>

<template>
<v-container>
  <v-row class="justify-center">
    <div class="d-flex">
      <v-img
          aspect-ratio="1/1"
          :width="531"
          cover
          src="inDev.svg"
      ></v-img>
    </div>
  </v-row>
    <v-row class="justify-center">
      <div class="d-flex">
        <div class="text-h3 font-weight-medium py-3 py-md-5 py-xl-14">
          Вибачте, сторінка на даний момент в розробці :(
        </div>
      </div>
    </v-row>
  <v-row class="justify-center">
    <v-col md="2" sm="3" xl="6">
      <v-btn
          class="d-flex btn-font-size"
          to="/"

      >
        На головну
      </v-btn>
    </v-col>
  </v-row>

</v-container>
</template>

<style scoped>
.btn-font-size{
  font-size: 20px !important;
}

</style>