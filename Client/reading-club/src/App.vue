<template>
  <the-navigator />
  <router-view />
</template>

<script>
import TheNavigation from '@/components/TheNavigation.vue'

export default {
  name: 'App',

  components: {
    TheNavigator: TheNavigation
  }
}
</script>

<style></style>
