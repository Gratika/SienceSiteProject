<script setup lang="ts">
import {reactive} from "vue";
import LoginView from "@/views/LoginView.vue";
import RegisterView from "@/views/RegisterView.vue";

const state = reactive({
  tab: null
});

</script>

<template>
  <v-container>
    <v-row class="justify-center mt-7">
      <v-col cols="12" md="10" sm="12">
        <v-card class="pt-10 shadow-auth">
          <v-tabs
              v-model="state.tab"
              color="black"
              fixed-tabs
              grow
          >
            <v-tab value="userLogin"><h2>Увійти</h2></v-tab>
            <v-tab value="userRegister"><h2>Реєстрація</h2></v-tab>
          </v-tabs>

          <v-card-text class="border-card">
            <v-window v-model="state.tab">
              <v-window-item value="userLogin">
                <LoginView/>
              </v-window-item>
              <v-window-item value="userRegister">
                <RegisterView/>
              </v-window-item>
            </v-window>
          </v-card-text>
        </v-card>
      </v-col>
    </v-row>
  </v-container>

</template>

<style scoped>
.border-card{
  border-top: 1px solid rgba(128, 128, 128, 50%);
}
.shadow-auth{
  box-shadow: 0 0 25px 0 rgba(0, 0, 0, 0.25);
}
</style>