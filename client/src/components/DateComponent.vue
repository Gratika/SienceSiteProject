<script setup lang="ts">
import { ref } from 'vue';


const selectedDate = ref(undefined);

const localeOptions = {
  firstDayOfWeek: 1,
  weekdays: ['Нд', 'Пн', 'Вт', 'Ср', 'Чт', 'Пт', 'Сб'],
  months: [
    'Січень',
    'Лютий',
    'Березень',
    'Квітень',
    'Травень',
    'Червень',
    'Липень',
    'Серпень',
    'Вересень',
    'Жовтень',
    'Листопад',
    'Грудень'
  ],
  format: 'YYYY-MM-DD',
  titleDateFormat: 'MMMM YYYY',
  mask: '__.__.____'
};

const landscapeMode = ref(false);

const allowedDates = {
  from: new Date(new Date().getFullYear(), new Date().getMonth(), 1),
  to: new Date(new Date().getFullYear(), new Date().getMonth() + 1, 0)
};

const onDateSelected = (selectedDate: Date) => {
  console.log('Обрана дата:', selectedDate);
};
</script>

<template>
  <v-date-picker
      v-model="selectedDate"
      color="primary"
      :locale="localeOptions"
      scrollable
      :landscape="landscapeMode"
      :allowed-dates="allowedDates"
      @input="onDateSelected"
  ></v-date-picker>
</template>
