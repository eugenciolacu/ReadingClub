<template>
  <div>
    <nav class="navbar navbar-expand-md navbar-dark bg-dark mb-4">
      <div class="container-sm">
        <!-- class="container-fluid" -->
        <div class="navbar-brand">Book Club</div>
        <button v-if="toDisplay" class="navbar-toggler collapsed" type="button" data-bs-toggle="collapse"
          data-bs-target="#navbarCollapse" aria-controls="navbarCollapse" aria-expanded="false"
          aria-label="Toggle navigation">
          <span class="navbar-toggler-icon"></span>
        </button>
        <div v-if="toDisplay" class="navbar-collapse collapse" id="navbarCollapse" style="">
          <ul class="navbar-nav me-auto">
            <li class="nav-item">
              <router-link :to="{ name: 'HomePage' }" class="nav-link" active-class="active">
                Home
              </router-link>
            </li>
            <li class="nav-item">
              <router-link :to="{
                name: 'ReadingListPage',
                params: { page: 1 },
              }" class="nav-link"
                :class="{ 'active': $route.name === 'ReadingListPage' || $route.name === 'BookDetailsForReadingListPage' }">
                Reading List
              </router-link>
            </li>
            <li class="nav-item">
              <router-link :to="{
                name: 'SearchForBooksPage',
                params: { page: 1 },
              }" class="nav-link"
                :class="{ 'active': $route.name === 'SearchForBooksPage' || $route.name === 'BookDetailsForSearchPage' }">
                Search
              </router-link>
            </li>
            <li class="nav-item">
              <router-link :to="{ 
                name: 'AdminPage', 
                params: { page: 1 }  
              }" class="nav-link"
                :class="{ 'active': $route.name === 'AdminPage' || $route.name === 'BookDetailsForAdminPage' }">
                Admin Page
              </router-link>
            </li>
          </ul>
          <div v-if="toDisplay" class="d-flex">
            <ul class="navbar-nav me-auto">
              <li class="nav-item">
                <router-link :to="{ name: 'ProfilePage' }" class="nav-link" active-class="active">
                  Profile
                </router-link>
              </li>
              <li class="nav-item">
                <the-logout>
                  <template v-slot:logoutLink="{ onLogoutClick }">
                    <a href="#" @click="onLogoutClick" class="nav-link" active-class="active">
                      Logout
                    </a>
                  </template>
                </the-logout>
              </li>
            </ul>
          </div>
        </div>
      </div>
    </nav>
  </div>
</template>

<script>
import TheLogout from '@/components/TheLogout.vue'

export default {
  name: 'TheNavigation',

  components: {
    TheLogout: TheLogout
  },

  computed: {
    toDisplay() {
      let route = this.$route.name

      if (route === undefined || route === 'LoginPage' || route === 'RegisterPage') {
        return false
      }

      return true
    }
  }
}
</script>

<style scoped>
.navbar {
  margin-bottom: 20px;
}
</style>
