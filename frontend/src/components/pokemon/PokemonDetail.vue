<script setup lang="ts">
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import AbilityIcon from "@/components/icons/AbilityIcon.vue";
import type { Ability, Pokemon } from "@/types/pokemon";
import { formatAbility } from "@/helpers/format";
import { getAbilityUrl } from "@/helpers/cms";

const { t } = useI18n();

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
</script>

<template>
  <table class="table table-striped">
    <tbody>
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
        <th scope="row">{{ t("pokemon.ability.title") }}</th>
        <!-- TODO(fpion): should not take an entire row -->
        <td colspan="3">
          <a :href="getAbilityUrl(ability)" target="_blank"><AbilityIcon /> {{ formatAbility(ability) }}</a>
        </td>
      </tr>
      <tr>
        <th scope="row">{{ t("pokemon.characteristic.label") }}</th>
        <!-- TODO(fpion): should not take an entire row -->
        <td colspan="3">{{ pokemon.characteristic }}.</td>
      </tr>
    </tbody>
  </table>
</template>
