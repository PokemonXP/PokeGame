<script setup lang="ts">
import { TarAvatar } from "logitar-vue3-ui";
import { computed } from "vue";
import { parsingUtils } from "logitar-js";
import { useI18n } from "vue-i18n";

import EditIcon from "@/components/icons/EditIcon.vue";
import ExternalIcon from "@/components/icons/ExternalIcon.vue";
import PokemonGenderIcon from "@/components/icons/PokemonGenderIcon.vue";
import type { Pokemon } from "@/types/pokemon";
import { getSpriteUrl } from "@/helpers/pokemon";

const { parseBoolean } = parsingUtils;
const { t } = useI18n();

const props = withDefaults(
  defineProps<{
    external?: boolean | string;
    level?: boolean | string;
    pokemon: Pokemon;
    size?: number | string;
  }>(),
  {
    size: 40,
  },
);

const isExternal = computed<boolean>(() => parseBoolean(props.external) ?? false);
const showLevel = computed<boolean>(() => parseBoolean(props.level) ?? false);
const target = computed<string | undefined>(() => (isExternal.value ? "_blank" : undefined));
</script>

<template>
  <div class="d-flex">
    <div class="d-flex">
      <div class="align-content-center flex-wrap mx-1">
        <slot name="avatar">
          <RouterLink :to="{ name: 'PokemonEdit', params: { id: pokemon.id } }" :target="target">
            <TarAvatar :display-name="pokemon.nickname ?? pokemon.uniqueName" icon="fas fa-dog" :size="size" :url="getSpriteUrl(pokemon)" />
          </RouterLink>
        </slot>
      </div>
    </div>
    <div>
      <slot name="before"></slot>
      <slot name="contents">
        <RouterLink :to="{ name: 'PokemonEdit', params: { id: pokemon.id } }" :target="target">
          <template v-if="pokemon.nickname">
            <ExternalIcon v-if="isExternal" />
            <EditIcon v-else />
            {{ pokemon.nickname }}
            <template v-if="showLevel">{{ t("pokemon.level.format", { level: pokemon.level }) }}</template>
            <br />
            <PokemonGenderIcon :gender="pokemon.gender" /> {{ pokemon.uniqueName }}
          </template>
          <template v-else>
            <ExternalIcon v-if="isExternal" />
            <EditIcon v-else />
            {{ pokemon.uniqueName }}
            <PokemonGenderIcon :gender="pokemon.gender" />
            <br />
            <template v-if="showLevel">{{ t("pokemon.level.format", { level: pokemon.level }) }}</template>
          </template>
        </RouterLink>
      </slot>
      <slot name="after"></slot>
    </div>
  </div>
</template>
