<script setup lang="ts">
import Emoji from "@/components/Emoji.vue";
import type {IEmotion} from "@/api/type";

const props = defineProps<{
  emojiList: IEmotion[],
  userReactionId: string
}>();

</script>

<template>
 <Emoji
     v-for="emoji in props.emojiList"
     :key="emoji.id"
     :emoji="emoji"
     :selectedEmojiId="props.userReactionId"
 />
</template>

<style scoped>

</style>