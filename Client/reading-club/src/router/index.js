/* eslint-disable no-constant-condition */
import { createRouter, createWebHistory } from 'vue-router'

import { checkIfUserIsAuthenticated } from '@/services/AuthenticationService'

const routes = [
  {
    path: '/',
    redirect: '/home',
    meta: { requiresAuth: true }
  },

  {
    path: '/admin/:page',
    query: {
      title: null,
      authors: null,
      isbn: null,
      orderBy: null,
      sort: null,
      items: null
    },
    name: 'AdminPage',
    component: () => import(/* webpackChunkName: "AdminPage" */ '@/views/AdminPage.vue'),
    meta: { 
      requiresAuth: true,
      defaultQuery: {
        title: '',
        authors: '',
        isbn: '',
        orderBy: 'Title',
        sort: 'Ascending',
        items: '5'
      },
      queryOptions: {
        orderBy: ['Title', 'Authors', 'ISBN'],
        sort: ['Ascending', 'Descending'],
        items: ['5', '10', '20', '50']
      }
    },
    beforeEnter: (to, from, next) => {
      clearUpdateAndNext(to, from, next)
    },
    children: [
      {
        path: 'book/:id/details',
        name: 'BookDetailsForAdminPage',
        component: () =>
          import(/* webpackChunkName: "BookDetailsForAdminPage" */ '@/views/BookDetailsPage.vue'),
        meta: { requiresAuth: true }
      }
    ]
  },

  {
    path: '/home',
    name: 'HomePage',
    component: () => import(/* webpackChunkName: "HomePage" */ '@/views/HomePage.vue'),
    meta: { requiresAuth: true }
  },

  {
    path: '/login',
    name: 'LoginPage',
    component: () => import(/* webpackChunkName: "LoginPage" */ '@/views/LoginPage.vue')
  },

  {
    path: '/profile',
    name: 'ProfilePage',
    component: () => import(/* webpackChunkName: "ProfilePage" */ '@/views/ProfilePage.vue'),
    meta: { requiresAuth: true }
  },

  {
    path: '/readinglist/:page',
    query: {
      title: null,
      authors: null,
      isbn: null,
      orderBy: null,
      sort: null,
      items: null,
      isRead: null
    },
    name: 'ReadingListPage',
    component: () =>
      import(/* webpackChunkName: "ReadingListPage" */ '@/views/ReadingListPage.vue'),
    meta: {
      requiresAuth: true,
      defaultQuery: {
        title: '',
        authors: '',
        isbn: '',
        orderBy: 'Title',
        sort: 'Ascending',
        items: '5',
        isRead: 'All'
      },
      queryOptions: {
        orderBy: ['Title', 'Authors', 'ISBN'],
        sort: ['Ascending', 'Descending'],
        items: ['5', '10', '20', '50'],
        isRead: ['All', 'Read', 'Not read']
      }
    },
    beforeEnter: (to, from, next) => {
      clearUpdateAndNext(to, from, next)
    },
    children: [
      {
        path: 'book/:id/details',
        name: 'BookDetailsForReadingListPage',
        component: () =>
          import(
            /* webpackChunkName: "BookDetailsForReadingListPage" */ '@/views/BookDetailsPage.vue'
          ),
        meta: { requiresAuth: true }
      }
    ]
  },

  {
    path: '/register',
    name: 'RegisterPage',
    component: () => import(/* webpackChunkName: "RegisterPage" */ '@/views/RegisterPage.vue')
  },

  {
    path: '/searchforbooks/:page',
    query: {
      title: null,
      authors: null,
      isbn: null,
      orderBy: null,
      sort: null,
      items: null
    },
    name: 'SearchForBooksPage',
    component: () =>
      import(/* webpackChunkName: "SearchForBooksPage" */ '@/views/SearchForBooksPage.vue'),
    meta: {
      requiresAuth: true,
      defaultQuery: {
        title: '',
        authors: '',
        isbn: '',
        orderBy: 'Title',
        sort: 'Ascending',
        items: '5'
      },
      queryOptions: {
        orderBy: ['Title', 'Authors', 'ISBN'],
        sort: ['Ascending', 'Descending'],
        items: ['5', '10', '20', '50']
      }
    },
    beforeEnter: (to, from, next) => {
      clearUpdateAndNext(to, from, next)
    },
    children: [
      {
        path: 'book/:id/details',
        name: 'BookDetailsForSearchPage',
        component: () =>
          import(/* webpackChunkName: "BookDetailsForSearchPage" */ '@/views/BookDetailsPage.vue'),
        meta: { requiresAuth: true }
      }
    ]
  },

  {
    path: '/:notFound(.*)',
    component: () => import(/* webpackChunkName: "NotFound" */ '@/views/NotFound.vue')
  }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

router.beforeEach((to, from, next) => {
  if (to.matched.some((route) => route.meta.requiresAuth)) {
    checkIfUserIsAuthenticated().then((isAuthenticated) => {
      if (isAuthenticated) {
        next()
      } else {
        next('/login')
      }
    })
  } else {
    next()
  }
})

function clearUpdateAndNext(to, from, next) {
  let count = 0

  for (const key in to.query) {
    if (to.meta.defaultQuery[key] === undefined || to.query[key] === to.meta.defaultQuery[key]) {
      delete to.query[key]
      count++
    } else {
      if (key in to.meta.queryOptions) {
        if (to.meta.queryOptions[key].includes(to.query[key]) === false) {
          to.query[key] = to.meta.defaultQuery[key]
          count++
        }
      }
    }
  }

  if (to.params.page < 1) {
    to.params.page = '' + 1
    next({ name: to.name, params: to.params, query: to.query })
  } else if (count > 0) {
    next({ name: to.name, params: to.params, query: to.query })
  } else {
    next()
  }
}

export default router
