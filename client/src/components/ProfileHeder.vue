<script setup lang="ts">

</script>

<template>
<v-row>
  <v-col cols="12">
    <div class="gradient">
      <div class="avatar-overlay">
        <v-avatar image="avatar.png" size="x-large"></v-avatar>
      </div>
    </div>
    <div class="general-size content-zone">
      <div class="btn-zone general-size">
        <v-chip>
          Редагувати профіль
        </v-chip>
        <v-chip variant="outlined">
          Налаштування
        </v-chip>
      </div>
      <div class="general-size info-align">Прізвище ім'я</div>
      <div class="general-size info-align">Київ, Україна</div>
    </div>

  </v-col>
</v-row>
</template>

<style scoped>

 .avatar-overlay {
   position: absolute;
   top: 100px;
   left: 60px; /*  відступ від лівого краю */
   transform: translate(-50%, -50%);
   z-index: 1; /* Підняти над content-zone */
 }

 .btn-zone{
   flex-direction: row;
   justify-content: flex-end;

 }
 .content-zone{
   flex-direction:column;
   z-index: 0;
 }
 .general-size{
   display: flex;
   width: 100%;
   padding-top: 10px;
 }
 .gradient{
   background: linear-gradient(to bottom right, #ead9ff, #c3f2ff);
   max-height: 100px;
   min-height: 100px;
   position: relative;
   width: 100%;
 }

 .info-align{
   justify-content: flex-start;
 }
</style>