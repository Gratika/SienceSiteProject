
import { createApp } from 'vue'
import { createPinia } from 'pinia'

import App from './App.vue'
import router from './router'


// Vuetify
import '@mdi/font/css/materialdesignicons.css'
import 'vuetify/styles'
import {createVuetify } from 'vuetify'
import * as components from 'vuetify/components'
import * as directives from 'vuetify/directives'
import CKEditor from '@ckeditor/ckeditor5-vue';
import type {ThemeDefinition} from 'vuetify'

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
    icons: {
        defaultSet: 'mdi', // This is already the default value - only for display purposes
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
