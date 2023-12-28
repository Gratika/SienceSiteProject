
import { createApp } from 'vue'
import { createPinia } from 'pinia'

import App from './App.vue'
import router from './router'
import '@/assets/styles/main.scss'


// Vuetify
import '@mdi/font/css/materialdesignicons.css'
import 'vuetify/styles'
import {createVuetify } from 'vuetify'
import * as components from 'vuetify/components'
import * as directives from 'vuetify/directives'
import type {ThemeDefinition} from 'vuetify'

import CKEditor from '@ckeditor/ckeditor5-vue';

const myCustomLightTheme: ThemeDefinition = {
    dark: false,
    colors: {
        background: '#FFFFFF',
        surface: '#FFFFFF',
        primary: '#5E8D64',
        'primary-darken-1': '#385D3D',
        secondary: '#778FD2',
        'secondary-darken-1': '#2A3759',
        error: '#B00020',
        info: '#2196F3',
        success: '#4CAF50',
        warning: '#FB8C00',
        'my-dark':'#607D8B',
        'my-accent':'#F44336'
    },
}
const vuetify = createVuetify({
    components,
    directives,
    defaults: {
        VBtn: {
            variant:'text',
            style: 'border-radius: 0',
        },
        VCombobox:{
            variant:'outlined',
            density:'compact',
            style: 'border-radius: 2px;',
        },
        VSelect:{
            variant:'outlined',
            density:'compact',
            style: 'border-radius: 2px;',
        },
        VTextField:{
            variant:'outlined',
            density:'compact',
            style: 'border-radius: 2px;',
        }
    },
    icons: {
        defaultSet: 'mdi',
    },
    theme: {
        defaultTheme: 'myCustomLightTheme',
        themes: {
            myCustomLightTheme
        },
    },
})
const app = createApp(App)

app.use(createPinia())
app.use(router)
app.use(vuetify)
app.use( CKEditor )

app.mount('#app')
