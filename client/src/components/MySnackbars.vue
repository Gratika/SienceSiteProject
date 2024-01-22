<script setup lang="ts">


const props = defineProps({
  showMessage:Boolean,
  message: String
})
const emits = defineEmits(['close-message'])
function closeMessage(){
  emits('close-message', false)
}
</script>

<template>
  <v-snackbar
      v-model="props.showMessage"
      multi-line
  >

    <p>{{props.message}}</p>

    <template v-slot:actions>
      <v-btn
          color="indigo"
          variant="text"
          @click="closeMessage"
      >
        Close
      </v-btn>
    </template>
  </v-snackbar>
</template>

<style scoped>

</style>