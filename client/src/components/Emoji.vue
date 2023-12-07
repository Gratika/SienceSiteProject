<script setup lang="ts">
import type {IEmotion} from "@/api/type";

const props = defineProps<{
  emoji: IEmotion,
  selectedEmojiId: string
}>()
function selectedEmoji () {
  console.log(`Selected emoji: ${props.emoji.Emoji}`);
}
</script>

<template>
  <span @click="selectedEmoji">{{props.emoji.Emoji}}</span>
</template>

<style scoped>

</style>