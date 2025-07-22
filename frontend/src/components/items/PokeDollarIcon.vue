<script setup lang="ts">
withDefaults(
  defineProps<{
    alt?: string;
    height?: number | string;
  }>(),
  {
    alt: "PokéDollar Icon",
  },
);
</script>

<template>
  <img src="@/assets/img/pokedollar.png" :alt="alt" :height="height" />
</template>
