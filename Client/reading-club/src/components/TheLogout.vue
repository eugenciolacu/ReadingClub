<template>
  <slot name="logoutLink" :onLogoutClick="onLogoutClick"> </slot>
</template>

<script>
import router from '@/router/index.js'
import { removeToken } from '@/services/AuthenticationService'

export default {
  methods: {
    async onLogoutClick() {
      removeToken();
      router.push({ name: 'LoginPage' });
    }
  }
}
</script>
