<script setup lang="ts">
const callApi=() => {
  fetch('/api/Emotion')
      .then(res=>{
        console.log(res)
      }).catch(err =>{
    console.log('ERR');
    console.log(err);
  })
}
</script>
<template>
  <div class="about">
   <button @click="callApi">Get All</button>
  </div>
</template>

<style>
@media (min-width: 1024px) {
  .about {
    min-height: 100vh;
    display: flex;
    align-items: center;
  }
}
</style>
