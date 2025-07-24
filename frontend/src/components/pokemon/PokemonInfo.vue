<script setup lang="ts">
import { TarAvatar } from "logitar-vue3-ui";
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import ExperienceBar from "./ExperienceBar.vue";
import PokemonGenderIcon from "@/components/icons/PokemonGenderIcon.vue";
import PokemonSprite from "./PokemonSprite.vue";
import PokemonTypeImage from "./PokemonTypeImage.vue";
import type { ItemSummary, PokemonSummary } from "@/types/game";

const { n, t } = useI18n();

const props = defineProps<{
  pokemon: PokemonSummary;
}>();

const heldItem = computed<ItemSummary | undefined>(() => props.pokemon.heldItem ?? undefined);
const isEgg = computed<boolean>(() => props.pokemon.level < 1);
const number = computed<string>(() => props.pokemon.number.toString().padStart(4, "0"));
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
            <tr>
              <th scope="row">{{ t("pokemon.size.height.label") }}</th>
              <td>
                <span v-if="isEgg" class="text-muted">{{ "—" }}</span>
                <template v-else>{{ n(pokemon.height, "height") }}</template>
              </td>
            </tr>
            <tr>
              <th scope="row">{{ t("pokemon.size.weight.label") }}</th>
              <td>
                <span v-if="isEgg" class="text-muted">{{ "—" }}</span>
                <template v-else>{{ n(pokemon.weight, "weight") }}</template>
              </td>
            </tr>
            <tr>
              <th scope="row">{{ t("pokemon.size.title") }}</th>
              <td>
                <span v-if="isEgg" class="text-muted">{{ "—" }}</span>
                <template v-else>{{ t(`pokemon.size.category.abbreviations.${pokemon.size}`) }}</template>
              </td>
            </tr>
            <tr>
              <th scope="row">{{ t("pokemon.memories.trainer.original") }}</th>
              <td>
                <span v-if="isEgg || !pokemon.originalTrainer" class="text-muted">{{ "—" }}</span>
                <template v-else>
                  <span>{{ pokemon.originalTrainer.name }}</span>
                  <span class="float-end">{{ pokemon.originalTrainer.license }}</span>
                </template>
              </td>
            </tr>
            <tr>
              <th scope="row">{{ t("pokemon.experience.label") }}</th>
              <td>
                <span v-if="isEgg || !pokemon.experience" class="text-muted">{{ "—" }}</span>
                <div v-else>
                  <div class="row">
                    <div class="col">{{ n(pokemon.experience.minimum, "integer") }}</div>
                    <div class="col text-center">
                      <strong>{{ n(pokemon.experience.current, "integer") }}</strong>
                    </div>
                    <div class="col text-end">{{ n(pokemon.experience.maximum, "integer") }}</div>
                  </div>
                  <ExperienceBar :percentage="pokemon.experience.percentage" />
                </div>
              </td>
            </tr>
            <tr>
              <th scope="row">{{ t("pokemon.experience.next.label") }}</th>
              <td>
                <span v-if="isEgg || !pokemon.experience" class="text-muted">{{ "—" }}</span>
                <template v-else>{{ n(pokemon.experience.toNextLevel, "integer") }}</template>
              </td>
            </tr>
            <tr>
              <th scope="row">{{ t("pokemon.item.held") }}</th>
              <td>
                <span v-if="isEgg || !heldItem" class="text-muted">{{ "—" }}</span>
                <template v-else>
                  <div><TarAvatar :display-name="heldItem.name" size="32" icon="fas fa-cart-shopping" :url="heldItem.sprite" /> {{ heldItem.name }}</div>
                  <div v-if="heldItem.description">{{ heldItem.description }}</div>
                </template>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
      <div class="col">
        <h3 class="h5">
          <span>
            <img v-if="pokemon.caughtBallSprite" :src="pokemon.caughtBallSprite" alt="Poké Ball" height="32" />
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
