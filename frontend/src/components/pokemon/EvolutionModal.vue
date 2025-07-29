<script setup lang="ts">
import { TarButton, TarCheckbox, TarModal } from "logitar-vue3-ui";
import { computed, ref } from "vue";
import { useI18n } from "vue-i18n";

import EvolutionTriggerIcon from "@/components/evolutions/EvolutionTriggerIcon.vue";
import FriendshipIcon from "@/components/icons/FriendshipIcon.vue";
import LevelIcon from "@/components/icons/LevelIcon.vue";
import LocationIcon from "@/components/icons/LocationIcon.vue";
import MoveIcon from "@/components/icons/MoveIcon.vue";
import NoLabel from "@/components/shared/NoLabel.vue";
import PokemonFormSelect from "./forms/PokemonFormSelect.vue";
import PokemonGenderIcon from "@/components/icons/PokemonGenderIcon.vue";
import TimeOfDayIcon from "@/components/icons/TimeOfDayIcon.vue";
import YesLabel from "@/components/shared/YesLabel.vue";
import type { Pokemon } from "@/types/pokemon";
import { EVOLUTION_FRIENDSHIP, type Evolution } from "@/types/evolutions";
import { formatItem, formatMove } from "@/helpers/format";
import { useForm } from "@/forms";
import { evolvePokemon } from "@/api/pokemon";

const { t } = useI18n();

const props = withDefaults(
  defineProps<{
    evolution: Evolution;
    id?: string;
    pokemon: Pokemon;
  }>(),
  {
    id: "evolve-pokemon",
  },
);

const isLoading = ref<boolean>(false);
const location = ref<string>("");
const modalRef = ref<InstanceType<typeof TarModal> | null>(null);
const timeOfDay = ref<string>("");

const isFriendship = computed<boolean>(() => !props.evolution.friendship || props.pokemon.friendship >= EVOLUTION_FRIENDSHIP);
const isGender = computed<boolean>(() => !props.evolution.gender || props.evolution.gender === props.pokemon.gender);
const isItemHeld = computed<boolean>(() => !props.evolution.heldItem || props.evolution.heldItem.id === props.pokemon.heldItem?.id);
const isLevel = computed<boolean>(() => props.evolution.level < 1 || props.pokemon.level >= props.evolution.level);
const isLocation = computed<boolean>(() => !Boolean(props.evolution.location) || props.evolution.location === location.value);
const isMoveKnown = computed<boolean>(() =>
  props.evolution.knownMove ? props.pokemon.moves.some((item) => item.move.id === props.evolution.knownMove?.id) : true,
);
const isTimeOfDay = computed<boolean>(() => !Boolean(props.evolution.timeOfDay) || props.evolution.timeOfDay === timeOfDay.value);
const isTraded = computed<boolean>(
  () =>
    props.evolution.trigger !== "Trade" ||
    Boolean(props.pokemon.ownership && props.pokemon.ownership.currentTrainer.id !== props.pokemon.ownership.originalTrainer.id),
);

const canEvolve = computed<boolean>(
  () =>
    !isLoading.value &&
    isFriendship.value &&
    isGender.value &&
    isItemHeld.value &&
    isLevel.value &&
    isLocation.value &&
    isMoveKnown.value &&
    isTimeOfDay.value &&
    isTraded.value,
);

const emit = defineEmits<{
  (e: "error", error: unknown): void;
  (e: "evolved", pokemon: Pokemon): void;
}>();

function hide(): void {
  modalRef.value?.hide();
}
function show(): void {
  modalRef.value?.show();
}

function cancel(): void {
  location.value = "";
  timeOfDay.value = "";
  hide();
}

function setLocation(): void {
  if (location.value === props.evolution.location) {
    location.value = "";
  } else {
    location.value = props.evolution.location ?? "";
  }
}
function setTimeOfDay(): void {
  if (timeOfDay.value === props.evolution.timeOfDay) {
    timeOfDay.value = "";
  } else {
    timeOfDay.value = props.evolution.timeOfDay ?? "";
  }
}

useForm();
async function submit(): Promise<void> {
  if (canEvolve.value) {
    isLoading.value = true;
    try {
      const pokemon: Pokemon = await evolvePokemon(props.pokemon.id, props.evolution.id);
      emit("evolved", pokemon);
      hide();
    } catch (e: unknown) {
      emit("error", e);
    } finally {
      isLoading.value = false;
    }
  }
}

defineExpose({ show });
</script>

<template>
  <TarModal :close="t('actions.close')" :id="id" ref="modalRef" size="large" :title="t('pokemon.evolve.title')">
    <form @submit.prevent="submit">
      <PokemonFormSelect disabled :forms="[evolution.target]" id="target" label="evolutions.target" :model-value="evolution.target.id" />
      <table class="table table-striped">
        <thead>
          <tr>
            <th scope="col">{{ t("evolutions.requirements.title") }}</th>
            <th scope="col">{{ t("evolutions.assessment") }}</th>
          </tr>
        </thead>
        <tbody>
          <tr v-if="evolution.trigger === 'Trade'">
            <td><EvolutionTriggerIcon trigger="Trade" /> {{ t("evolutions.trigger.options.Trade") }}</td>
            <td>
              <YesLabel v-if="isTraded" class="text-success" />
              <NoLabel v-else class="text-danger" />
            </td>
          </tr>
          <tr v-if="evolution.level">
            <td>
              <LevelIcon />
              {{ t("evolutions.requirements.level", { level: evolution.level }) }}
            </td>
            <td>
              <span v-if="isLevel" class="text-success"><font-awesome-icon icon="fas fa-check" /> {{ pokemon.level }}</span>
              <span v-else class="text-danger"><font-awesome-icon icon="fas fa-times" /> {{ pokemon.level }}</span>
            </td>
          </tr>
          <tr v-if="evolution.friendship">
            <td><FriendshipIcon /> {{ t("evolutions.requirements.friendship") }}</td>
            <td>
              <span v-if="isFriendship" class="text-success"><font-awesome-icon icon="fas fa-check" /> {{ pokemon.friendship }}</span>
              <span v-else class="text-danger"><font-awesome-icon icon="fas fa-times" /> {{ pokemon.friendship }}</span>
            </td>
          </tr>
          <tr v-if="evolution.gender">
            <td><PokemonGenderIcon :gender="evolution.gender" /> {{ t(`pokemon.gender.select.options.${evolution.gender}`) }}</td>
            <td>
              <span v-if="isGender" class="text-success">
                <PokemonGenderIcon :gender="pokemon.gender" /> {{ t(`pokemon.gender.select.options.${pokemon.gender}`) }}
              </span>
              <span v-else class="text-danger"><PokemonGenderIcon :gender="pokemon.gender" /> {{ t(`pokemon.gender.select.options.${pokemon.gender}`) }}</span>
            </td>
          </tr>
          <tr v-if="evolution.heldItem">
            <td>
              <RouterLink v-if="evolution.heldItem" :to="{ name: 'ItemEdit', params: { id: evolution.heldItem.id } }" target="_blank">
                <img
                  v-if="evolution.heldItem.sprite"
                  :src="evolution.heldItem.sprite"
                  :alt="t('sprite.alt', { name: formatItem(evolution.heldItem) })"
                  height="20"
                />
                {{ formatItem(evolution.heldItem) }}
              </RouterLink>
            </td>
            <td>
              <RouterLink
                v-if="isItemHeld && pokemon.heldItem"
                :to="{ name: 'ItemEdit', params: { id: evolution.heldItem.id } }"
                class="text-success"
                target="_blank"
              >
                <img v-if="pokemon.heldItem.sprite" :src="pokemon.heldItem.sprite" :alt="t('sprite.alt', { name: formatItem(pokemon.heldItem) })" height="20" />
                {{ formatItem(pokemon.heldItem) }}
              </RouterLink>
              <RouterLink v-else-if="pokemon.heldItem" :to="{ name: 'ItemEdit', params: { id: evolution.heldItem.id } }" class="text-danger" target="_blank">
                <img v-if="pokemon.heldItem.sprite" :src="pokemon.heldItem.sprite" :alt="t('sprite.alt', { name: formatItem(pokemon.heldItem) })" height="20" />
                {{ formatItem(pokemon.heldItem) }}
              </RouterLink>
              <NoLabel v-else class="text-danger" />
            </td>
          </tr>
          <tr v-if="evolution.knownMove">
            <td>
              <RouterLink v-if="evolution.knownMove" :to="{ name: 'MoveEdit', params: { id: evolution.knownMove.id } }" target="_blank">
                <MoveIcon /> {{ formatMove(evolution.knownMove) }}
              </RouterLink>
            </td>
            <td>
              <span v-if="isMoveKnown" class="text-success"><font-awesome-icon icon="fas fa-check" /> {{ t("yes") }}</span>
              <NoLabel v-else class="text-danger" />
            </td>
          </tr>
          <tr v-if="evolution.location">
            <td><LocationIcon /> {{ evolution.location }}</td>
            <td>
              <TarCheckbox id="location" :model-value="isLocation" required @update:model-value="setLocation">
                <template #label-override>
                  <label class="form-check-label" for="location">
                    <span class="text-success" v-if="isLocation">{{ t("yes") }}</span>
                    <span class="text-danger" v-else>{{ t("no") }}</span>
                  </label>
                </template>
              </TarCheckbox>
            </td>
          </tr>
          <tr v-if="evolution.timeOfDay">
            <td><TimeOfDayIcon :time="evolution.timeOfDay" /> {{ t(`evolutions.timeOfDay.options.${evolution.timeOfDay}`) }}</td>
            <td>
              <TarCheckbox id="time-of-day" :model-value="isTimeOfDay" required @update:model-value="setTimeOfDay">
                <template #label-override>
                  <label class="form-check-label" for="time-of-day">
                    <span class="text-success" v-if="isTimeOfDay">{{ t("yes") }}</span>
                    <span class="text-danger" v-else>{{ t("no") }}</span>
                  </label>
                </template>
              </TarCheckbox>
            </td>
          </tr>
        </tbody>
      </table>
    </form>
    <template #footer>
      <TarButton icon="fas fa-ban" :text="t('actions.cancel')" variant="secondary" @click="cancel" />
      <TarButton
        :disabled="!canEvolve"
        icon="fas fa-dna"
        :loading="isLoading"
        :status="t('loading')"
        :text="t('pokemon.evolve.submit')"
        variant="primary"
        @click="submit"
      />
    </template>
  </TarModal>
</template>
