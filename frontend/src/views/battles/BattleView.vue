<script setup lang="ts">
import { arrayUtils } from "logitar-js";
import { computed, inject, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import AbilityIcon from "@/components/icons/AbilityIcon.vue";
import AdminBreadcrumb from "@/components/admin/AdminBreadcrumb.vue";
import ExternalLink from "@/components/battle/ExternalLink.vue";
import ItemBlock from "@/components/items/ItemBlock.vue";
import PokemonBlock from "@/components/pokemon/PokemonBlock.vue";
import StaminaBar from "@/components/pokemon/StaminaBar.vue";
import StartBattle from "@/components/battle/StartBattle.vue";
import StatusConditionIcon from "@/components/pokemon/StatusConditionIcon.vue";
import TrainerBlock from "@/components/trainers/TrainerBlock.vue";
import VitalityBar from "@/components/pokemon/VitalityBar.vue";
import type { Ability } from "@/types/abilities";
import type { Battle, Battler } from "@/types/battle";
import type { Breadcrumb } from "@/types/components";
import { StatusCodes, type ApiFailure } from "@/types/api";
import { getAbility, getUrl } from "@/helpers/pokemon";
import { handleErrorKey } from "@/inject";
import { readBattle } from "@/api/battle";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const { orderByDescending } = arrayUtils;
const { t } = useI18n();

type BattlerInfo = Battler & {
  order: number;
  ability: Ability;
  url?: string;
};

const battle = ref<Battle>();
const isLoading = ref<boolean>(false);

const activeBattlers = computed<BattlerInfo[]>(() =>
  orderByDescending(
    battle.value?.battlers
      .filter(({ isActive }) => isActive)
      .map((battler) => ({
        ...battler,
        order: battler.pokemon.statistics.speed.value,
        ability: getAbility(battler.pokemon),
        url: getUrl(battler.pokemon),
      })) ?? [],
    "order",
  ),
);
const breadcrumb = computed<Breadcrumb[]>(() => {
  const breadcrumb: Breadcrumb[] = [{ to: { name: "BattleList" }, text: t("battle.title") }];
  if (battle.value) {
    breadcrumb.push({ to: { name: "BattleEdit", params: { id: battle.value.id } }, text: battle.value.name });
  }
  return breadcrumb;
});

onMounted(async () => {
  isLoading.value = true;
  try {
    const id = route.params.id as string;
    battle.value = await readBattle(id);
  } catch (e: unknown) {
    const { status } = e as ApiFailure;
    if (status === StatusCodes.NotFound) {
      router.push("/not-found");
    } else {
      handleError(e);
    }
  } finally {
    isLoading.value = false;
  }
});
</script>

<template>
  <main class="container-fluid">
    <template v-if="battle">
      <h1 class="text-center">{{ battle.name }}</h1>
      <div class="d-flex justify-content-center">
        <AdminBreadcrumb :current="t('battle.action')" :parent="breadcrumb" />
      </div>
      <div class="mb-3 d-flex gap-2">
        <RouterLink :to="{ name: 'BattleEdit', params: { id: battle.id } }" class="btn btn-secondary">
          <font-awesome-icon icon="fas fa-door-closed" /> {{ t("battle.leave") }}
        </RouterLink>
        <StartBattle v-if="battle.status === 'Created'" :battle="battle" @error="handleError" @started="battle = $event" />
      </div>
      <table v-if="battle.status === 'Started'" class="table table-striped">
        <thead>
          <tr>
            <th scope="col">{{ t("pokemon.statistic.battle.Speed") }}</th>
            <th scope="col">{{ t("pokemon.title") }}</th>
            <th scope="col">{{ t("abilities.label") }}</th>
            <th scope="col">{{ t("items.held.label") }}</th>
            <th scope="col">{{ t("pokemon.status.label") }}</th>
            <th scope="col">{{ t("trainers.select.label") }}</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="battler in activeBattlers" :key="battler.pokemon.id">
            <td>{{ battler.pokemon.statistics.speed.value }}</td>
            <td>
              <PokemonBlock external level :pokemon="battler.pokemon" :size="battler.url ? 60 : undefined">
                <template v-if="battler.url" #after>
                  <br />
                  <ExternalLink :href="battler.url" />
                </template>
              </PokemonBlock>
            </td>
            <td>
              <RouterLink :to="{ name: 'AbilityEdit', params: { id: battler.ability.id } }" target="_blank">
                <AbilityIcon /> {{ battler.ability.displayName ?? battler.ability.uniqueName }}
                <template v-if="battler.ability.displayName">
                  <br />
                  {{ battler.ability.uniqueName }}
                </template>
              </RouterLink>
              <template v-if="battler.ability.url">
                <br />
                <ExternalLink :href="battler.ability.url" />
              </template>
            </td>
            <td>
              <ItemBlock v-if="battler.pokemon.heldItem" :item="battler.pokemon.heldItem" external :size="battler.pokemon.heldItem.url ? 60 : undefined">
                <template v-if="battler.pokemon.heldItem.url" #after>
                  <br />
                  <ExternalLink :href="battler.pokemon.heldItem.url" />
                </template>
              </ItemBlock>
              <span v-else class="text-muted">{{ "—" }}</span>
            </td>
            <td>
              <div class="d-flex align-items-center">
                <span class="text-danger me-2 text-nowrap"> {{ battler.pokemon.vitality }}/{{ battler.pokemon.statistics.hp.value }} </span>
                <div class="flex-grow-1">
                  <VitalityBar :current="battler.pokemon.vitality" :maximum="battler.pokemon.statistics.hp.value" />
                </div>
              </div>
              <div class="d-flex align-items-center">
                <span class="text-primary me-2 text-nowrap"> {{ battler.pokemon.stamina }}/{{ battler.pokemon.statistics.hp.value }} </span>
                <div class="flex-grow-1">
                  <StaminaBar :current="battler.pokemon.stamina" :maximum="battler.pokemon.statistics.hp.value" />
                </div>
              </div>
              <StatusConditionIcon v-if="battler.pokemon.statusCondition" :status="battler.pokemon.statusCondition" height="20" />
            </td>
            <td>
              <TrainerBlock v-if="battler.pokemon.ownership" external :size="battler.url ? 60 : undefined" :trainer="battler.pokemon.ownership.currentTrainer">
                <template v-if="battler.pokemon.ownership.currentTrainer.url" #after>
                  <br />
                  <ExternalLink :href="battler.pokemon.ownership.currentTrainer.url" />
                </template>
              </TrainerBlock>
              <span v-else class="text-muted">{{ t("pokemon.wild") }}</span>
            </td>
          </tr>
        </tbody>
      </table>
      <p v-else-if="battle.status === 'Created'">{{ t("battle.notStarted") }}</p>
      <p v-else>{{ t("battle.ended") }}</p>
    </template>
  </main>
</template>
