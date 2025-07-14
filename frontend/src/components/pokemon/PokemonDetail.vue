<script setup lang="ts">
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import AbilityIcon from "@/components/icons/AbilityIcon.vue";
import FormIcon from "@/components/icons/FormIcon.vue";
import ItemIcon from "@/components/icons/items/ItemIcon.vue";
import PokemonTypeImage from "./PokemonTypeImage.vue";
import SpeciesIcon from "@/components/icons/SpeciesIcon.vue";
import VarietyIcon from "@/components/icons/VarietyIcon.vue";
import type { Ability, Form, Pokemon, PokemonSize, Species, Variety } from "@/types/pokemon";
import { calculateSize, getMaximumExperience } from "@/helpers/pokemon";
import { formatAbility, formatForm, formatItem, formatSpecies, formatVariety } from "@/helpers/format";
import { getAbilityUrl, getFormUrl, getSpeciesUrl, getVarietyUrl } from "@/helpers/cms";

const { n, t } = useI18n();

const props = defineProps<{
  pokemon: Pokemon;
}>();

const ability = computed<Ability>(() => {
  const { abilitySlot, form } = props.pokemon;
  let ability: Ability | undefined = form.abilities.primary;
  if (abilitySlot === "Secondary" && form.abilities.secondary) {
    ability = form.abilities.secondary;
  } else if (abilitySlot === "Hidden" && form.abilities.hidden) {
    ability = form.abilities.hidden;
  }
  return ability;
});
const form = computed<Form>(() => props.pokemon.form);
const size = computed<PokemonSize>(() => calculateSize(props.pokemon.form, props.pokemon.size));
const species = computed<Species>(() => props.pokemon.form.variety.species);
const variety = computed<Variety>(() => props.pokemon.form.variety);

const minimumExperience = computed<number>(() => (props.pokemon.level === 1 ? 0 : getMaximumExperience(species.value.growthRate, props.pokemon.level - 1)));
const experiencePercentage = computed<number>(() =>
  props.pokemon.maximumExperience === minimumExperience.value
    ? 1
    : (props.pokemon.experience - minimumExperience.value) / (props.pokemon.maximumExperience - minimumExperience.value),
);
</script>

<template>
  <table class="table table-striped">
    <tbody>
      <tr>
        <th scope="row">
          {{ t("pokemon.species.select.label") }}
          {{ " / " }}
          {{ t("pokemon.variety.select.label") }}
          {{ " / " }}
          {{ t("pokemon.form.select.label") }}
        </th>
        <td>
          <a :href="getSpeciesUrl(species)" target="_blank"><SpeciesIcon /> {{ formatSpecies(species) }}</a>
        </td>
        <td>
          <a :href="getVarietyUrl(variety)" target="_blank"><VarietyIcon /> {{ formatVariety(variety) }}</a>
        </td>
        <td>
          <a :href="getFormUrl(form)" target="_blank"><FormIcon /> {{ formatForm(form) }}</a>
        </td>
      </tr>
      <tr>
        <th scope="row">{{ t("pokemon.type.types") }} / {{ t("pokemon.type.tera") }}</th>
        <td>
          <PokemonTypeImage :type="form.types.primary" />
        </td>
        <td>
          <PokemonTypeImage v-if="form.types.secondary" :type="form.types.secondary" class="ms-2" />
          <span v-else class="text-muted">{{ "—" }}</span>
        </td>
        <td>
          <PokemonTypeImage :type="pokemon.teraType" />
        </td>
      </tr>
      <tr>
        <th scope="row">
          {{ t("pokemon.level.label") }}
          {{ " / " }}
          {{ t("pokemon.experience.label") }}
        </th>
        <td>{{ pokemon.level }}</td>
        <td>{{ pokemon.experience }} / {{ pokemon.maximumExperience }} ({{ n(experiencePercentage, "integer_percent") }})</td>
        <td>{{ t("pokemon.experience.next.format", { next: pokemon.toNextLevel }) }}</td>
      </tr>
      <tr>
        <th scope="row">
          {{ t("pokemon.size.height.label") }}
          {{ " / " }}
          {{ t("pokemon.size.weight.label") }}
          {{ " / " }}
          {{ t("pokemon.size.category.label") }}
        </th>
        <td>{{ n(size.height, "height") }} ({{ pokemon.size.height }})</td>
        <td>{{ n(size.weight, "weight") }} ({{ pokemon.size.weight }})</td>
        <td>{{ t(`pokemon.size.category.options.${size.category}`) }}</td>
      </tr>
      <tr>
        <th scope="row">{{ t("pokemon.nature.select.label") }}</th>
        <td>{{ pokemon.nature.name }}</td>
        <td>
          <template v-if="pokemon.nature.increasedStatistic && pokemon.nature.decreasedStatistic">
            <span class="text-primary">
              <font-awesome-icon icon="fas fa-plus" /> {{ t(`pokemon.statistic.select.options.${pokemon.nature.increasedStatistic}`) }}
            </span>
            {{ " / " }}
            <span class="text-danger">
              <font-awesome-icon icon="fas fa-minus" /> {{ t(`pokemon.statistic.select.options.${pokemon.nature.decreasedStatistic}`) }}
            </span>
          </template>
          <span v-else class="text-muted">{{ "—" }}</span>
        </td>
        <td>
          <template v-if="pokemon.nature.favoriteFlavor && pokemon.nature.dislikedFlavor">
            <span class="text-success">
              <font-awesome-icon icon="fas fa-face-smile" /> {{ t(`pokemon.flavor.options.${pokemon.nature.favoriteFlavor}`) }}
            </span>
            {{ " / " }}
            <span class="text-danger"> <font-awesome-icon icon="fas fa-face-frown" /> {{ t(`pokemon.flavor.options.${pokemon.nature.dislikedFlavor}`) }} </span>
          </template>
          <span v-else class="text-muted">{{ "—" }}</span>
        </td>
      </tr>
      <tr>
        <th scope="row">{{ t("pokemon.characteristic.label") }}</th>
        <!-- TODO(fpion): should not take an entire row -->
        <td colspan="3">{{ pokemon.characteristic }}.</td>
      </tr>
      <tr>
        <th scope="row">{{ t("pokemon.friendship.label") }}</th>
        <!-- TODO(fpion): should not take an entire row; should be editable -->
        <td colspan="3">{{ pokemon.friendship }}</td>
      </tr>
      <tr>
        <th scope="row">{{ t("pokemon.ability.title") }}</th>
        <!-- TODO(fpion): should not take an entire row -->
        <td colspan="3">
          <a :href="getAbilityUrl(ability)" target="_blank"><AbilityIcon /> {{ formatAbility(ability) }}</a>
        </td>
      </tr>
      <tr>
        <th scope="row">{{ t("pokemon.growthRate.select.label") }}</th>
        <!-- TODO(fpion): should not take an entire row; show experience table modal -->
        <td colspan="3">{{ t(`pokemon.growthRate.select.options.${species.growthRate}`) }}</td>
      </tr>
      <tr>
        <th scope="row">{{ t("pokemon.item.held") }}</th>
        <!-- TODO(fpion): should not take an entire row; should be editable (update, give from inventory, take to inventory) -->
        <td colspan="3">
          <a v-if="pokemon.heldItem" href="#" target="_blank"><ItemIcon /> {{ formatItem(pokemon.heldItem) }}</a>
          <span v-else class="text-muted">{{ "—" }}</span>
        </td>
      </tr>
    </tbody>
  </table>
</template>
