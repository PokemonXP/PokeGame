<script setup lang="ts">
import { TarAvatar } from "logitar-vue3-ui";

import EditIcon from "@/components/icons/EditIcon.vue";
import PokemonGenderIcon from "@/components/icons/PokemonGenderIcon.vue";
import type { Pokemon } from "@/types/pokemon";
import { getSpriteUrl } from "@/helpers/pokemon";

defineProps<{
  pokemon: Pokemon;
}>();
</script>

<template>
  <div class="d-flex">
    <div class="d-flex">
      <div class="align-content-center flex-wrap mx-1">
        <RouterLink :to="{ name: 'PokemonEdit', params: { id: pokemon.id } }">
          <TarAvatar :display-name="pokemon.nickname ?? pokemon.uniqueName" icon="fas fa-dog" size="40" :url="getSpriteUrl(pokemon)" />
        </RouterLink>
      </div>
    </div>
    <div>
      <RouterLink :to="{ name: 'PokemonEdit', params: { id: pokemon.id } }">
        <template v-if="pokemon.nickname">
          <EditIcon /> {{ pokemon.nickname }}
          <br />
          <PokemonGenderIcon :gender="pokemon.gender" /> {{ pokemon.uniqueName }}
        </template>
        <template v-else>
          <EditIcon /> {{ pokemon.nickname }}
          {{ pokemon.uniqueName }}
          <PokemonGenderIcon :gender="pokemon.gender" />
        </template>
      </RouterLink>
    </div>
  </div>
</template>
