<script setup lang="ts">
 import {ref} from "vue";

 const props = defineProps<{is_active:boolean}>();
 const dialog = ref(props.is_active);
 const emits = defineEmits(['close']);
 function closeDialog(){
   dialog.value=false;
   emits('close', dialog.value);
 }
</script>

<template>
  <v-dialog
      v-model="dialog"
      width="auto"
  >
 <h1>this_Dialog</h1>
    <v-btn @click="closeDialog">Close</v-btn>
  </v-dialog>


</template>

<style scoped>

</style>