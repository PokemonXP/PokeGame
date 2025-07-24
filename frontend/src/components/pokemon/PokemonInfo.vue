<script setup lang="ts">
import { TarAvatar } from "logitar-vue3-ui";
import { useI18n } from "vue-i18n";

import PokemonGenderIcon from "@/components/icons/PokemonGenderIcon.vue";
import PokemonSprite from "./PokemonSprite.vue";
import PokemonTypeImage from "./PokemonTypeImage.vue";
import type { ItemSummary, PokemonSummary } from "@/types/pokemon/game";
import { computed } from "vue";

const { n, t } = useI18n();

const props = defineProps<{
  pokemon: PokemonSummary;
}>();

const heldItem = computed<ItemSummary | undefined>(() => props.pokemon.heldItem ?? undefined);
const isEgg = computed<boolean>(() => props.pokemon.level < 1);
const number = computed<string>(() => props.pokemon.number.toString().padStart(4, "0"));
const pokeBall = computed<ItemSummary>(() => props.pokemon.pokeBall);
</script>

<template>
  <section>
    <div class="row">
      <div class="col">
        <table class="table table-striped">
          <tbody>
            <tr>
              <th scope="row">{{ t("pokemon.pokedex.number") }}</th>
              <td>
                <span v-if="isEgg" class="text-muted">{{ "—" }}</span>
                <template v-else>{{ number }}</template>
              </td>
            </tr>
            <tr>
              <th scope="row">{{ t("name.label") }}</th>
              <td>
                <span v-if="isEgg" class="text-muted">{{ "—" }}</span>
                <template v-else>{{ pokemon.name }}</template>
              </td>
            </tr>
            <tr>
              <th scope="row">{{ t("pokemon.type.label") }}</th>
              <td>
                <span v-if="isEgg" class="text-muted">{{ "—" }}</span>
                <template v-else>
                  <PokemonTypeImage :type="pokemon.primaryType" height="32" />
                  <PokemonTypeImage v-if="pokemon.secondaryType" :type="pokemon.secondaryType" class="ms-2" height="32" />
                </template>
              </td>
            </tr>
            <tr>
              <th scope="row">{{ t("pokemon.type.tera") }}</th>
              <td>
                <span v-if="isEgg" class="text-muted">{{ "—" }}</span>
                <template v-else>
                  <PokemonTypeImage tera :type="pokemon.teraType" height="32" />
                </template>
              </td>
            </tr>
            <!-- TODO(fpion): Height, Weight, Size -->
            <tr>
              <th scope="row">{{ t("pokemon.memories.trainer.original") }}</th>
              <td>
                <span v-if="isEgg || !pokemon.originalTrainer" class="text-muted">{{ "—" }}</span>
                <template v-else>
                  <span>{{ pokemon.originalTrainer.name }}</span>
                  <!-- TODO(fpion): license instead of sprite -->
                  <span class="float-end">{{ pokemon.originalTrainer.sprite }}</span>
                </template>
              </td>
            </tr>
            <tr>
              <th scope="row">{{ t("pokemon.experience.label") }}</th>
              <td>
                <span v-if="isEgg" class="text-muted">{{ "—" }}</span>
                <template v-else>{{ n(pokemon.experience, "integer") }}</template>
              </td>
            </tr>
            <!-- TODO(fpion): to next level, progress with percent, min./max., next -->
            <tr>
              <th scope="row">{{ t("pokemon.item.held") }}</th>
              <td>
                <span v-if="isEgg || !heldItem" class="text-muted">{{ "—" }}</span>
                <template v-else>
                  <TarAvatar :display-name="heldItem.name" size="32" icon="fas fa-cart-shopping" :url="heldItem.sprite" /> {{ heldItem.name }}
                  <!-- TODO(fpion): missing description -->
                </template>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
      <div class="col">
        <h3 class="h5">
          <span>
            <img v-if="pokeBall.sprite" :src="pokeBall.sprite" :alt="t('sprite.alt', { name: pokeBall.name })" height="32" />
            {{ isEgg ? t("pokemon.egg.label") : (pokemon.nickname ?? pokemon.name) }}
          </span>
          <span v-if="!isEgg" class="float-end">
            {{ t("pokemon.level.format", { level: pokemon.level }) }} <PokemonGenderIcon :gender="pokemon.gender" />
          </span>
        </h3>
        <PokemonSprite :pokemon="pokemon" />
      </div>
    </div>
  </section>
</template>
