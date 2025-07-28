<script setup lang="ts">
import { computed } from "vue";
import { parsingUtils } from "logitar-js";
import { useI18n } from "vue-i18n";

import EditIcon from "@/components/icons/EditIcon.vue";
import EvolutionTriggerIcon from "./EvolutionTriggerIcon.vue";
import FriendshipIcon from "@/components/icons/FriendshipIcon.vue";
import ItemBlock from "@/components/items/ItemBlock.vue";
import MoveIcon from "@/components/icons/MoveIcon.vue";
import PokemonFormBlock from "@/components/pokemon/forms/PokemonFormBlock.vue";
import PokemonGenderIcon from "@/components/icons/PokemonGenderIcon.vue";
import StatusBlock from "@/components/shared/StatusBlock.vue";
import TimeOfDayIcon from "@/components/icons/TimeOfDayIcon.vue";
import type { Evolution } from "@/types/evolutions";
import { formatItem, formatMove } from "@/helpers/format";

const { parseBoolean } = parsingUtils;
const { t } = useI18n();

const props = defineProps<{
  evolutions: Evolution[];
  incoming?: boolean | string;
  outgoing?: boolean | string;
}>();

const isIncoming = computed<boolean>(() => parseBoolean(props.incoming) ?? false);
const isOutgoing = computed<boolean>(() => parseBoolean(props.outgoing) ?? false);

function hasRequirements(evolution: Evolution): boolean {
  return Boolean(
    evolution.level || evolution.friendship || evolution.gender || evolution.heldItem || evolution.knownMove || evolution.location || evolution.timeOfDay,
  );
}
</script>

<template>
  <table class="table table-striped">
    <thead>
      <tr>
        <th scope="col"></th>
        <th v-if="!isOutgoing" scope="col">{{ t("evolutions.source") }}</th>
        <th v-if="!isIncoming" scope="col">{{ t("evolutions.target") }}</th>
        <th scope="col">{{ t("evolutions.trigger.label") }}</th>
        <th scope="col">{{ t("evolutions.requirements.title") }}</th>
        <th scope="col">{{ t("evolutions.sort.options.UpdatedOn") }}</th>
      </tr>
    </thead>
    <tbody>
      <tr v-for="evolution in evolutions" :key="evolution.id">
        <td>
          <RouterLink :to="{ name: 'EvolutionEdit', params: { id: evolution.id } }"><EditIcon /> {{ t("actions.edit") }}</RouterLink>
        </td>
        <td v-if="!isOutgoing">
          <PokemonFormBlock :form="evolution.source" />
        </td>
        <td v-if="!isIncoming">
          <PokemonFormBlock :form="evolution.target" />
        </td>
        <td>
          <ItemBlock v-if="evolution.item" :item="evolution.item" />
          <template v-else>
            <EvolutionTriggerIcon :trigger="evolution.trigger" />
            {{ t(`evolutions.trigger.options.${evolution.trigger}`) }}
          </template>
        </td>
        <td>
          <template v-if="hasRequirements(evolution)">
            <div v-if="evolution.level">
              <font-awesome-icon icon="fas fa-arrow-turn-up" /> {{ t("evolutions.requirements.level", { level: evolution.level }) }}
            </div>
            <div v-if="evolution.friendship"><FriendshipIcon /> {{ t("evolutions.requirements.friendship") }}</div>
            <div v-if="evolution.gender"><PokemonGenderIcon :gender="evolution.gender" /> {{ t(`pokemon.gender.select.options.${evolution.gender}`) }}</div>
            <div>
              <RouterLink v-if="evolution.heldItem" :to="{ name: 'ItemEdit', params: { id: evolution.heldItem.id } }">
                <img
                  v-if="evolution.heldItem.sprite"
                  :src="evolution.heldItem.sprite"
                  :alt="t('sprite.alt', { name: formatItem(evolution.heldItem) })"
                  height="20"
                />
                {{ formatItem(evolution.heldItem) }}
              </RouterLink>
            </div>
            <div>
              <RouterLink v-if="evolution.knownMove" :to="{ name: 'MoveEdit', params: { id: evolution.knownMove.id } }">
                <MoveIcon /> {{ formatMove(evolution.knownMove) }}
              </RouterLink>
            </div>
            <div v-if="evolution.location"><font-awesome-icon icon="fas fa-location-dot" /> {{ evolution.location }}</div>
            <div v-if="evolution.timeOfDay"><TimeOfDayIcon :time="evolution.timeOfDay" /> {{ t(`evolutions.timeOfDay.options.${evolution.timeOfDay}`) }}</div>
          </template>
          <span v-else class="text-muted">{{ "—" }}</span>
        </td>
        <td><StatusBlock :actor="evolution.updatedBy" :date="evolution.updatedOn" /></td>
      </tr>
    </tbody>
  </table>
</template>
