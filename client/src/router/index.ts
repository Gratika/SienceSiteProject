import { createRouter, createWebHistory } from 'vue-router'
import HomeView from '@/views/HomeView.vue'
import MyArticleView from "@/views/MyArticleView.vue";

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
      component: () => import('@/views/AuthView.vue'),
      beforeEnter: (to, from, next) => {
        import('../views/AuthView.vue')
            .then(module => {
              next();
            }).catch(error => {
          console.error('Помилка завантаження модуля LoginView.vue:', error);
          next({ name: 'notFound', params: { pathMatch: to.path.substring(1).split('/') } });
        });
      }
    },
    /*{
      path: '/register',
      name: 'register',
      component: () => import('../views/RegisterView.vue')
    },*/
    {
      path: '/verify_email',
      name: 'verify_email',
      component: () => import('../views/VerifyEmailView.vue'),
      beforeEnter: (to, from, next) => {
       import('../views/VerifyEmailView.vue').then(module => {
          next();
        }).catch(error => {
          console.error('Помилка завантаження модуля VerifyEmailView.vue:', error);
         next({ name: 'notFound', params: { pathMatch: to.path.substring(1).split('/') } });
        });
      }
    },
    {
      path: '/my_article',
      name: 'my_article',
      component: () => import('../views/MyArticleView.vue'),
      meta: { requiresAuth: true },
      beforeEnter: (to, from, next) => {
        import('../views/MyArticleView.vue').then(module => {
          next();
        }).catch(error => {
          console.error('Помилка завантаження модуля MyArticleView.vue:', error);
          next({ name: 'notFound', params: { pathMatch: to.path.substring(1).split('/') } });
        });
      }
    },
    {
      path: '/edit_article/:id',
      name: 'edit_article',
      component: ()=>import('../views/EditArticleView.vue'),
      meta: { requiresAuth: true },
      beforeEnter: (to, from, next) => {
        import('../views/EditArticleView.vue').then(module => {
          next();
        }).catch(error => {
          console.error('Помилка завантаження модуля EditArticleView.vue:', error);
          next({ name: 'notFound', params: { pathMatch: to.path.substring(1).split('/') } });
        });
      }

    },
    {
      path: '/search/:search',
      name: 'search_article',
      component: ()=>import('../views/SearchResultView.vue'),
      beforeEnter: (to, from, next) => {
        import('../views/SearchResultView.vue').then(module => {
          next();
        }).catch(error => {
          console.error('Помилка завантаження модуля SearchResultView.vue:', error);
          next({ name: 'notFound', params: { pathMatch: to.path.substring(1).split('/') } });
        });
      }

    },
    {
      path: '/search_science/:scienceId',
      name: 'search_science_article',
      component: ()=>import('../views/SearchCategoryResultView.vue'),
      beforeEnter: (to, from, next) => {
        import('../views/SearchCategoryResultView.vue').then(module => {
          next();
        }).catch(error => {
          console.error('Помилка завантаження модуля SearchCategoryResultView.vue:', error);
          next({ name: 'notFound', params: { pathMatch: to.path.substring(1).split('/') } });
        });
      }

    },
    {
      path: '/read_article/:id',
      name: 'read_article',
      component: ()=>import('../views/ReadArticleView.vue'),
      beforeEnter: (to, from, next) => {
        import('../views/ReadArticleView.vue').then(module => {
          next();
        }).catch(error => {
          console.error('Помилка завантаження модуля ReadArticleView.vue:', error);
          next({ name: 'notFound', params: { pathMatch: to.path.substring(1).split('/') } });
        });
      }

    },
    {
      path: '/office',
      name: 'user_office',
      component: ()=>import('../views/UserOfficeView.vue'),
      meta: { requiresAuth: true },
      beforeEnter: (to, from, next) => {
        import('../views/UserOfficeView.vue').then(module => {
          next();
        }).catch(error => {
          console.error('Помилка завантаження модуля UserOfficeView.vue:', error);
          next({ name: 'notFound', params: { pathMatch: to.path.substring(1).split('/') } });
        });
      }

    },
    {
      path: '/profile',
      name: 'profile',
      component: ()=>import('../views/UserProfileView.vue'),
      meta: { requiresAuth: true },
      beforeEnter: (to, from, next) => {
        import('../views/UserProfileView.vue').then(module => {
          next();
        }).catch(error => {
          console.error('Помилка завантаження модуля UserProfileView.vue:', error);
          next({ name: 'notFound', params: { pathMatch: to.path.substring(1).split('/') } });
        });
      }

    },
    {
      path: '/in_development',
      name: 'developmentPage',
      component: ()=>import('../views/DevelopmentPageView.vue'),

    },
    {
      path: '/test',
      name: 'test',
      component: ()=>import('../views/NotFoundView.vue'),

    },
    {/* UserProfileView для непередбачуваних маршрутів*/
      path:"/:pathMatch(.*)*",
      name:'nofFound',
      component:()=>import('../views/NotFoundView.vue')
    }

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
