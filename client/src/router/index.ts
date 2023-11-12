import { createRouter, createWebHistory } from 'vue-router'
import HomeView from '../views/HomeView.vue'

const LoginView = () => import('../views/LoginView.vue');
const VerifyEmailView = () => import('../views/VerifyEmailView.vue');


const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'home',
      component: HomeView
    },
    {
      path: '/login',
      name: 'login',
      component: LoginView,
      beforeEnter: (to, from, next) => {
        import('../views/LoginView.vue').then(module => {
          next();
        }).catch(error => {
          console.error('Помилка завантаження модуля LoginView.vue:', error);
          // Тут ви можете обробити помилку
          next(false); // Відмова від переходу
        });
      }
    },
    {
      path: '/register',
      name: 'register',
      component: () => import('../views/RegisterView.vue')
    },
    {
      path: '/verify_email',
      name: 'verify_email',
      component: VerifyEmailView,
      beforeEnter: (to, from, next) => {
       import('../views/VerifyEmailView.vue').then(module => {
          next();
        }).catch(error => {
          console.error('Помилка завантаження модуля VerifyEmailView.vue:', error);
          // Тут ви можете обробити помилку
          next(false); // Відмова від переходу
        });
      }
    },
    {
      path: '/my_article',
      name: 'my_article',
      component: ()=>import('../views/MyArticleView.vue'),
      meta: { requiresAuth: true },
    },
    {
      path: '/new_article',
      name: 'new_article',
      component: ()=>import('../views/NewArticle.vue'),

    },

  ]
})
/*router.beforeEach((to, from, next) => {
  if (to.matched.some(record => record.meta.requiresAuth)) {
    // Якщо сторінка вимагає авторизації
    if (localStorage.getItem('isLogin')!=='true') { // Перевіряємо, чи користувач авторизований. Якщо ні
     next('/login'); // Перенаправлення на сторінку авторизації
    } else {// Якщо користувач авторизований, дозволяємо йому перейти на сторінку
      next();
    }
  } else {// Якщо сторінка не вимагає авторизації, дозволяємо перейти без перевірки
   next();
  }
})*/

export default router
