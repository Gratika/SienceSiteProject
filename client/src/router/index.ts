import { createRouter, createWebHistory } from 'vue-router'
import HomeView from '@/views/HomeView.vue'
import MyArticleView from "@/views/MyArticleView.vue";
import MyLocalStorage from "@/services/myLocalStorage";
import AuthView from "@/views/AuthView.vue";
import VerifyEmailView from "@/views/VerifyEmailView.vue";
import CategoryList from "@/views/CategoryList.vue";
import EditArticleView from "@/views/EditArticleView.vue";
import SearchResultView from "@/views/SearchResultView.vue";
import SearchCategoryResultView from "@/views/SearchCategoryResultView.vue";
import ReadArticleView from "@/views/ReadArticleView.vue";
import UserOfficeView from "@/views/UserOfficeView.vue";
import UserProfileView from "@/views/UserProfileView.vue";

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
      component: AuthView, //() => import('@/views/AuthView.vue'),
      /*beforeEnter: (to, from, next) => {
        import('../views/AuthView.vue')
            .then(module => {
              next();
            }).catch(error => {
          console.error('Помилка завантаження модуля LoginView.vue:', error);
          return{name:'nofFound'}
        });
      }*/
    },

    {
      path: '/verify_email',
      name: 'verify_email',
      component: VerifyEmailView ,
    },
    {
      path: '/category-list',
      name: 'category_list',
      component: CategoryList,
    },
    {
      path: '/my_article',
      name: 'my_article',
      component: MyArticleView,
      meta: { requiresAuth: true },
    },
    {
      path: '/edit_article/:id',
      name: 'edit_article',
      component: EditArticleView,
      meta: { requiresAuth: true },

    },
    {
      path: '/search/:search',
      name: 'search_article',
      component: SearchResultView,

    },
    {
      path: '/search_science/:scienceId',
      name: 'search_science_article',
      component: SearchCategoryResultView,

    },
    {
      path: '/read_article/:id',
      name: 'read_article',
      component: ReadArticleView,

    },
    {
      path: '/office',
      name: 'user_office',
      component:UserOfficeView ,
      meta: { requiresAuth: true },

    },
    {
      path: '/profile',
      name: 'profile',
      component: UserProfileView,
      meta: { requiresAuth: true },

    },
    {
      path: '/in_development',
      name: 'developmentPage',
      component: ()=>import('../views/DevelopmentPageView.vue'),

    },
    {
      path: '/test',
      name: 'test',
      component: ()=>import('../views/VerifyEmailView.vue'),

    },
    {/* UserProfileView для непередбачуваних маршрутів*/
      path:"/:pathMatch(.*)*",
      name:'nofFound',
      component:()=>import('../views/NotFoundView.vue')
    }

  ]
})
router.beforeEach((to, from, next) => {
  if (to.matched.some(record => record.meta.requiresAuth)) {
    // Якщо сторінка вимагає авторизації
    if (MyLocalStorage.getItem('isLogin')===true) { // Перевіряємо, чи користувач авторизований.
       next(); // Якщо користувач авторизований, дозволяємо йому перейти на сторінку
    } else {
       next('/login'); // Перенаправлення на сторінку авторизації
    }
  } else {// Якщо сторінка не вимагає авторизації, дозволяємо перейти без перевірки
   next();
  }
})

export default router
